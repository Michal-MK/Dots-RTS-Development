using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

public class ServerAccess {
	public bool isDownloading = false;
	public string downloadedFile = "";


	/// <summary>
	/// Connects to the FTP server and reads its contents
	/// </summary>
	/// <returns>List of all the files/folders in the root directory</returns>
	//public List<string> GetContents() {
	//	//t.text += "Called | ";
	//	List<string> contents = new List<string>();

	//	FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://kocicka.endora.cz/"));
	//	//t.text += "Request created | ";
	//	request.Credentials = new NetworkCredential("phage", "Abcd123");
	//	request.Method = WebRequestMethods.Ftp.ListDirectory;
	//	//t.text += "Methods done | ";
	//	try {
	//		FtpWebResponse response = (FtpWebResponse)request.GetResponse();
	//		//t.text += "Got response | ";
	//		StreamReader s = new StreamReader(response.GetResponseStream());
	//		string line = s.ReadLine();
	//		while (!string.IsNullOrEmpty(line)) {
	//			if (line != "." && line != "..") {
	//				if (line.Contains(".phage")) {
	//					//t.text += line + " | ";
	//					contents.Add(line);
	//				}
	//			}
	//			line = s.ReadLine();
	//		}
	//		s.Close();
	//		response.Close();
	//	}
	//	catch (Exception e) {
	//		Debug.Log(e);
	//	}

	//	//t.text += "Returned value : ";
	//	for (int i = 0; i < contents.Count; i++) {
	//		//t.text += contents[i] + ", ";
	//	}

	//	return contents;
	//}


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


	/// <summary>
	/// Downloads selected file
	/// </summary>
	/// <param name="filePath">Name of the file - NOT full name.</param>
	/// <returns>Location of the file.</returns>
	//public string GetFile(string filePath) {
	//	isDownloading = true;
	//	DownloadFileFTP(filePath, true);
	//	return downloadedFile;
	//}

	public async Task<string> GetFilePathAsync(string filePath) {
		isDownloading = true;
		return await DownloadFTPAsync(filePath, true);
	}



	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	//	public void DownloadFileFTP(string fileName, bool temp = false) {
	//		isDownloading = true;

	//#if (UNITY_ANDROID || UNITY_IOS)
	//		string tempPath = Application.temporaryCachePath;
	//		string persistentPath = Application.persistentDataPath + "/Saves/";
	//#else
	//		string tempPath = Application.temporaryCachePath + "\\Saves\\";
	//		string persistentPath = Application.streamingAssetsPath + "\\Saves\\";
	//#endif
	//		if (temp) {
	//			inputfilepath = tempPath;
	//		}
	//		else {
	//			inputfilepath = persistentPath;
	//		}
	//		inputfilepath += fileName;
	//		using (WebClient request = new WebClient()) {
	//			request.Credentials = new NetworkCredential("phage", "Abcd123");
	//			try {
	//				request.DownloadDataCompleted += Request_DownloadDataCompleted;
	//				request.DownloadDataAsync(new Uri("ftp://kocicka.endora.cz/" + fileName));
	//			}
	//			catch (Exception e) {
	//				Debug.Log(e);
	//			}

	//		}
	//	}


	public async Task<string> DownloadFTPAsync(string fileName, bool temp = false) {
		//isDownloading = true;
		//inputfilepath = "";
		//UnityEngine.Debug.Log(fileName);
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
		//Debug.Log(fileName);

		try {
			WebResponse response = await req.GetResponseAsync();
			UnityEngine.Debug.Log(fileName);
			response = (FtpWebResponse)response;

			using (Stream s = response.GetResponseStream()) {
				if (inputfilepath == null){
					if (temp) {
						inputfilepath = tempPath + fileName;
					}
					else {
						inputfilepath = persistentPath + fileName;
					}
					//UnityEngine.Debug.LogError(inputfilepath);
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
					isDownloading = false;
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


	//Async function to trigger when the download finishes.
	//private void Request_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
	//	try {
	//		using (FileStream file = new FileStream(inputfilepath, FileMode.Create)) {
	//			file.Write(e.Result, 0, e.Result.Length);
	//			downloadedFile = inputfilepath;
	//			file.Close();
	//			isDownloading = false;
	//			Debug.Log("Successfully downloaded file - Async");
	//		}

	//	}
	//	catch (Exception ex) {
	//		Debug.Log(ex);
	//	}

	//	if (!File.Exists(inputfilepath)) {
	//		Debug.Log("OMG");
	//	}
	//}

	/// <summary>
	/// Downloads File from the server and return its contents
	/// </summary>
	/// <param name="path">Path to the file</param>
	/// <returns>An array - [0] = Level name, [1] = Author, [2] = Formated date+time.</returns>
	//public IEnumerator GetLevelInfo(string path) {
	//	Debug.Log(path);
	//	DownloadFileFTP(path, true);
	//	yield return new WaitUntil(() => !isDownloading);

	//	string[] info = new string[3];

	//	BinaryFormatter bf = new BinaryFormatter();
	//	using (FileStream fileS = File.Open(downloadedFile, FileMode.Open)) {
	//		SaveData s = (SaveData)bf.Deserialize(fileS);

	//		fileS.Close();

	//		info[0] = s.levelInfo.levelName;
	//		info[1] = s.levelInfo.creator;
	//		info[2] = string.Format("{0:dd/MM/yy H:mm:ss}", s.levelInfo.creationTime);

	//	}
	//	Debug.Log(info[0]);
	//	yield return info;
	//}


	public async Task<SaveData> GetLevelInfoAsync(string name) {
		UnityEngine.Debug.Log(name);
		string str = await DownloadFTPAsync(name, true);
		string[] info = new string[3];

		//SaveData s = await LevelMarket.DeserializeObjectAsync<SaveData>(new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true));
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
		string persistentPath = Application.persistentDataPath + "/Saves/";
#else
		string persistentPath = Application.streamingAssetsPath + "\\Saves\\";
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
