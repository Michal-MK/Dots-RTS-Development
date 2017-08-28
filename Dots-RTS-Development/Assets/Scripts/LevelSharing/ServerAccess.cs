using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Diagnostics;

public class ServerAccess {

	/// <summary>
	/// Connects to the FTP server and reads its contents
	/// </summary>
	/// <returns>List of all the files/folders in the root directory</returns>
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


	///// <summary>
	///// Downloads selected file
	///// </summary>
	///// <param name="filePath">Name of the file - NOT full name.</param>
	///// <returns>Location of the file.</returns>
	//public async Task<string> GetFilePathAsync(string filePath) {
	//	return await DownloadFTPAsync(filePath, true);
	//}



	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public async Task<string> DownloadFTPAsync(string fileName, bool temp = false) {

#if (UNITY_ANDROID || UNITY_IOS)
		string tempPath = Application.temporaryCachePath;
		string persistentPath = Application.persistentDataPath  + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#else
		string tempPath = Application.temporaryCachePath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
		string persistentPath = Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Saves" + Path.DirectorySeparatorChar;
#endif

		string inputfilepath = null;

		FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://kocicka.endora.cz/" + fileName);
		req.Credentials = new NetworkCredential("phage", "Abcd123");
		req.Method = WebRequestMethods.Ftp.DownloadFile;
		req.UseBinary = true;

		try {
			WebResponse response = await req.GetResponseAsync();
			UnityEngine.Debug.Log(fileName);
			response = (FtpWebResponse)response;

			using (Stream s = response.GetResponseStream()) {
				if (inputfilepath == null) {
					if (temp) {
						inputfilepath = tempPath + fileName;
					}
					else {
						inputfilepath = persistentPath + fileName;
					}
				}
				using (FileStream file = File.Create(inputfilepath)) {
					inputfilepath = null;
					byte[] buffer = new byte[4096];
					int readCount = await s.ReadAsync(buffer, 0, buffer.Length);
					while (readCount > 0) {
						await file.WriteAsync(buffer, 0, readCount);
						readCount = s.Read(buffer, 0, buffer.Length);
					}
					file.Close();
					s.Close();
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
	/// <param name="path">Path to the file</param>
	/// <returns>Data needed for level creation</returns>
	public async Task<SaveData> GetLevelInfoAsync(string name) {
		UnityEngine.Debug.Log(name);
		string str = await DownloadFTPAsync(name, true);
		string[] info = new string[3];

		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fileS = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true)) {
			SaveData s = (SaveData)bf.Deserialize(fileS);
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
