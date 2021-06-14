using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class ServerAccess {

	public async Task<List<string>> GetLevelsAsync() {
		HttpWebRequest request = WebRequest.CreateHttp(new Uri("http://192.168.88.5:5000/Levels"));

		try {
			using WebResponse response = await request.GetResponseAsync();
			using StreamReader s = new StreamReader(response.GetResponseStream());
			return JsonUtility.FromJson<List<string>>(await s.ReadToEndAsync());
		}
		catch (Exception e) {
			UnityEngine.Debug.Log(e);
		}
		return null;
	}

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public async Task<string> DownloadAsync(string fileName, bool temp = false) {
		string tempPath;
		string persistentPath;
		if (GameEnvironment.IsAndroid) {
			tempPath = Application.temporaryCachePath;
			persistentPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
		}
		else {
			tempPath = Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
			persistentPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
		}

		string hostname = "http://192.168.88.5:5000/Level?name=" + fileName;

		HttpWebRequest req = WebRequest.CreateHttp(hostname);

		try {
			WebResponse response = await req.GetResponseAsync();
			UnityEngine.Debug.Log(fileName);
			response = (FtpWebResponse)response;

			using Stream s = response.GetResponseStream();

			string inputFilePath;
			if (temp) {
				inputFilePath = tempPath + fileName;
			}
			else {
				inputFilePath = persistentPath + fileName;
			}
			using FileStream file = File.Create(inputFilePath);
			byte[] buffer = new byte[4096];
			int readCount = await s.ReadAsync(buffer, 0, buffer.Length);
			while (readCount > 0) {
				await file.WriteAsync(buffer, 0, readCount);
				readCount = await s.ReadAsync(buffer, 0, buffer.Length);
			}
			UnityEngine.Debug.Log("Successfully downloaded file" + file.Name + "  - Async");
			return file.Name;
		}
		catch (Exception e) {
			UnityEngine.Debug.LogError(e);
			return "";
		}
	}

	/// <summary>
	/// Uploads file to server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void UploadLevel(string fileName) {

#if (UNITY_ANDROID || UNITY_IOS)
		string persistentPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#else
		string persistentPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#endif

		string inputFilePath = persistentPath + fileName;
		string hostname = "http://192.168.88.5:5000/UploadLevel?name=" + fileName;

		HttpWebRequest req = WebRequest.CreateHttp(hostname);
		byte[] data = File.ReadAllBytes(inputFilePath);
		req.GetRequestStream().Write(data, 0, data.Length);
		req.GetResponse();
	}
}
