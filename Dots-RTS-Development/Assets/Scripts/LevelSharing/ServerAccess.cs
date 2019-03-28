using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class ServerAccess {

	/// <summary>
	/// Connects to the FTP server and reads its contents
	/// </summary>

	public async Task<List<string>> GetContentsAsync() {
		List<string> contents = new List<string>();

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://kocicka.endora.cz/"));
		request.Credentials = new NetworkCredential("phage", "Abcd123");
		request.Method = WebRequestMethods.Ftp.ListDirectory;

		try {
			Stopwatch stop = Stopwatch.StartNew();
			WebResponse response = await request.GetResponseAsync();
			UnityEngine.Debug.Log(stop.Elapsed);
			response = (FtpWebResponse)response;
			StreamReader s = new StreamReader(response.GetResponseStream());
			string line = s.ReadLine();
			while (!string.IsNullOrEmpty(line)) {
				if (line != "." && line != "..") {
					if (line.Contains(".phage")) {
						contents.Add(line);
					}
				}
				line = s.ReadLine();
			}
			s.Close();
			response.Close();
		}
		catch (Exception e) {
			UnityEngine.Debug.Log(e);
		}

		//t.text += "Returned value : ";
		for (int i = 0; i < contents.Count; i++) {
			//t.text += contents[i] + ", ";
		}

		return contents;
	}

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public async Task<string> DownloadFTPAsync(string fileName, bool temp = false) {
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

		string inputFilePath = null;

		FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://kocicka.endora.cz/" + fileName);
		req.Credentials = new NetworkCredential("phage", "Abcd123");
		req.Method = WebRequestMethods.Ftp.DownloadFile;
		req.UseBinary = true;

		try {
			WebResponse response = await req.GetResponseAsync();
			UnityEngine.Debug.Log(fileName);
			response = (FtpWebResponse)response;

			using (Stream s = response.GetResponseStream()) {
				if (inputFilePath == null) {
					if (temp) {
						inputFilePath = tempPath + fileName;
					}
					else {
						inputFilePath = persistentPath + fileName;
					}
				}
				using (FileStream file = File.Create(inputFilePath)) {
					inputFilePath = null;
					byte[] buffer = new byte[4096];
					int readCount = await s.ReadAsync(buffer, 0, buffer.Length);
					while (readCount > 0) {
						await file.WriteAsync(buffer, 0, readCount);
						readCount = s.Read(buffer, 0, buffer.Length);
					}
					UnityEngine.Debug.Log("Successfully downloaded file" + file.Name + "  - Async");
					return file.Name;
				}
			}
		}
		catch (Exception e) {
			UnityEngine.Debug.LogError(e);
			return "";
		}
	}

	/// <summary>
	/// Downloads File from the server and return its contents
	/// </summary>
	public async Task<SaveData> GetLevelInfoAsync(string name) {
		UnityEngine.Debug.Log($"Getting level info from FTP... {name}");
		string str = await DownloadFTPAsync(name, true);

		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fileS = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
			SaveData s = (SaveData)bf.Deserialize(fileS);
			UnityEngine.Debug.Log($"Data downloaded successfully!");
			return s;
		}
	}



	/// <summary>
	/// Uploads file to server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void UploadFileFTP(string fileName) {

#if (UNITY_ANDROID || UNITY_IOS)
		string persistentPath = Application.persistentDataPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#else
		string persistentPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#endif

		string inputfilepath = persistentPath + fileName;
		string ftpfullpath = "ftp://kocicka.endora.cz/" + fileName;

		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");

			try {
				request.UploadDataAsync(new Uri(ftpfullpath), WebRequestMethods.Ftp.UploadFile, File.ReadAllBytes(inputfilepath));
			}
			catch (Exception e) {
				UnityEngine.Debug.Log(e);
			}
			UnityEngine.Debug.Log("Successfully uploaded file");
		}
	}
}
