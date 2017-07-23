using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;

public class ServerAccess {
	public bool isDownloading = false;
	public string downloadedFile = "";


	/// <summary>
	/// Connects to the FTP server and reads its contents
	/// </summary>
	/// <returns>List of all the files/folders in the root directory</returns>
	public List<string> GetContents() {
		//t.text += "Called | ";
		List<string> contents = new List<string>();

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://kocicka.endora.cz/"));
		//t.text += "Request created | ";
		request.Credentials = new NetworkCredential("phage", "Abcd123");
		request.Method = WebRequestMethods.Ftp.ListDirectory;
		//t.text += "Methods done | ";
		try {
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
			//t.text += "Got response | ";
			StreamReader s = new StreamReader(response.GetResponseStream());
			string line = s.ReadLine();
			while (!string.IsNullOrEmpty(line)) {
				if (line != "." && line != "..") {
					if (line.Contains(".phage")) {
						//t.text += line + " | ";
						contents.Add(line);
					}
				}
				line = s.ReadLine();
			}
			s.Close();
			response.Close();
		}
		catch (Exception e) {
			Debug.Log(e);
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
	public string GetFile(string filePath) {
		isDownloading = true;
		DownloadFileFTP(filePath, true);
		return downloadedFile;
	}


	string inputfilepath = "";

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void DownloadFileFTP(string fileName, bool temp = false) {
		isDownloading = true;

#if (UNITY_ANDROID || UNITY_IOS)
		string tempPath = Application.temporaryCachePath;
		string persistentPath = Application.persistentDataPath + "/Saves/";
#else
		string tempPath = Application.temporaryCachePath + "\\Saves\\";
		string persistentPath = Application.streamingAssetsPath + "\\Saves\\";
		#endif
		if (temp) {
			inputfilepath = tempPath;
		}
		else {
			inputfilepath = persistentPath;
		}
		inputfilepath += fileName;
		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");
			try {
				request.DownloadDataCompleted += Request_DownloadDataCompleted;
				request.DownloadDataAsync(new Uri("ftp://kocicka.endora.cz/" + fileName));
			}
			catch (Exception e) {
				Debug.Log(e);
			}

		}
	}

	/*
	public void DownloadFTP(string fileName, bool temp = false) {
		isDownloading = true;
		inputfilepath = "";

#if (UNITY_ANDROID || UNITY_IOS)
		string tempPath = Application.temporaryCachePath;
		string persistentPath = Application.persistentDataPath + "/Saves/";
#else
		string tempPath = Application.temporaryCachePath + "\\Saves\\";
		string persistentPath = Application.streamingAssetsPath + "\\Saves\\";
#endif

		if (temp) {
			inputfilepath = tempPath;
		}
		else {
			inputfilepath = persistentPath;
		}

		inputfilepath += fileName;

		FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://kocicka.endora.cz/" + fileName);
		req.Credentials = new NetworkCredential("phage", "Abcd123");
		req.Method = WebRequestMethods.Ftp.DownloadFile;
		req.UseBinary = true;

		try {
			FtpWebResponse response = (FtpWebResponse)req.GetResponse();
			Stream s = response.GetResponseStream();

			using (FileStream file = new FileStream(inputfilepath, FileMode.Create)) {
				byte[] buffer = new byte[4096];
				int ReadCount = s.Read(buffer, 0, buffer.Length);
				Debug.Log(response.StatusDescription);
				Debug.Log(ReadCount);
				while (ReadCount > 0) {
					file.Write(buffer, 0, ReadCount);
					ReadCount = s.Read(buffer, 0, buffer.Length);
					Debug.Log(ReadCount);
				}
				file.Close();
				s.Close();
				downloadedFile = inputfilepath;
				isDownloading = false;
				Debug.Log("Successfully downloaded file - Standard");
			}
		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}
	*/

	//Async function to trigger when the download finishes.
	private void Request_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
		try {
			using (FileStream file = new FileStream(inputfilepath, FileMode.Create)) {
				file.Write(e.Result, 0, e.Result.Length);
				downloadedFile = inputfilepath;
				file.Close();
				isDownloading = false;
				Debug.Log("Successfully downloaded file - Async");
			}

		}
		catch (Exception ex) {
			Debug.Log(ex);
		}

		if (!File.Exists(inputfilepath)) {
			Debug.Log("OMG");
		}
	}

	/// <summary>
	/// Downloads File from the server and return its contents
	/// </summary>
	/// <param name="path">Path to the file</param>
	/// <returns>An array - [0] = Level name, [1] = Author, [2] = Formated date+time.</returns>
	public IEnumerator GetLevelInfo(string path) {
		Debug.Log(path);
		DownloadFileFTP(path, true);
		yield return new WaitUntil(() => !isDownloading);

		string[] info = new string[3];

		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fileS = File.Open(downloadedFile, FileMode.Open)) {
			SaveData s = (SaveData)bf.Deserialize(fileS);

			fileS.Close();

			info[0] = s.levelInfo.levelName;
			info[1] = s.levelInfo.creator;
			info[2] = string.Format("{0:dd/MM/yy H:mm:ss}", s.levelInfo.creationTime);

		}
		Debug.Log(info[0]);
		yield return info;
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
				Debug.Log(e);
			}
			Debug.Log("Successfully uploaded file");
		}
	}
}
