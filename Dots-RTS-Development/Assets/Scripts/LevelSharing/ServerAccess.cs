using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ServerAccess {
	public bool isDownloading = false;
	public string downloadedFile = "";
	//public Text t = null;

	/// <summary>
	/// Connects to the FTP server and reads its contents
	/// </summary>
	/// <returns>List of all the files/folders in the root directory</returns>
	public List<string> GetContents() {
		//t.text += "Called | ";
		List<string> contents = new List<string>();

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new System.Uri("ftp://kocicka.endora.cz/"));
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
		catch (System.Exception e) {
			//t.text += e.ToString();
		}

		//t.text += "Returned value : ";
		for (int i = 0; i < contents.Count; i++) {
			//t.text += contents[i] + ", ";
		}

		return contents;
	}


	public string GetFile(string filePath) {
		isDownloading = true;
		DownloadFileFTP(filePath, true);
		return downloadedFile;
	}


	string inputfilepath;

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void DownloadFileFTP(string fileName, bool temp = false) {


		if (temp) {
			//t.text += "Getting temp file | ";
#if !UNITY_ANDROID
			inputfilepath = Application.temporaryCachePath + "\\";
#else
			inputfilepath = Application.temporaryCachePath + "/";
#endif
		}
		else {
			//t.text += "Getting normal file | ";
#if !UNITY_ANDROID
			inputfilepath = Application.streamingAssetsPath + "\\Saves\\";
#else
			inputfilepath = Application.persistentDataPath + "/Saves/";
#endif
		}
		inputfilepath += fileName;
		//t.text += "Path set | ";

		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");
			try {
				//Debug.Log("Dowload Initiated.");
				request.DownloadDataCompleted += Request_DownloadDataCompleted;
				request.DownloadDataAsync(new System.Uri("ftp://kocicka.endora.cz/" + fileName));
				//Debug.Log("download started");
			}
			catch (WebException e) {
				Debug.Log(e);
				if (e.Status == WebExceptionStatus.Timeout) {
					//DownloadFileFTP(fileName, temp);
				}
			}
			catch (Exception e) {
				Debug.Log(e);
			}

		}
	}

	public void DownloadFTP(string fileName, bool temp = false) {
		inputfilepath = "";

		if (temp) {
			//t.text += "Getting temp file | ";
#if !UNITY_ANDROID
			inputfilepath = Application.temporaryCachePath + "\\";
#else
			inputfilepath = Application.temporaryCachePath + "/";
#endif
		}
		else {
			//t.text += "Getting normal file | ";
#if !UNITY_ANDROID
			inputfilepath = Application.streamingAssetsPath + "\\Saves\\";
#else
			inputfilepath = Application.persistentDataPath + "/Saves/";
#endif
		}

		inputfilepath += fileName;

		FtpWebRequest req = (FtpWebRequest)WebRequest.Create("ftp://kocicka.endora.cz/" + fileName);
		req.Credentials = new NetworkCredential("phage", "Abcd123");
		req.Method = WebRequestMethods.Ftp.DownloadFile;
		req.UseBinary = true;

		try {
			FtpWebResponse response = (FtpWebResponse)req.GetResponse();
			//t.text += "Got response | ";
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
			}
		}
		catch(Exception e) {
			Debug.Log(e);
		}
		isDownloading = false;
	}

	private void Request_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
		using (FileStream file = File.Create(inputfilepath)) {
			file.Write(e.Result, 0, e.Result.Length);
			downloadedFile = inputfilepath;
			file.Close();
			isDownloading = false;
			//Debug.Log("Successfully downloaded file");
			//if (t != null) {
			//t.text += "Success";
			//}
			try {
				//t.text += "Successful download.";
			}
			catch (System.Exception ex) {
				Debug.Log(ex);
			}
		}
	}

	public IEnumerator GetLevelInfo(string path) {
		GetFile(path);
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
		yield return info;
	}



	/// <summary>
	/// Uploads file to server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void UploadFileFTP(string fileName) {
#if !UNITY_ANDROID
		string inputfilepath = Application.streamingAssetsPath + "\\Saves\\" + fileName;
#else
		string inputfilepath = Application.persistentDataPath + "/Saves/" + fileName;
#endif
		string ftpfullpath = "ftp://kocicka.endora.cz/" + fileName;
		System.Uri uri = new System.Uri(ftpfullpath);


		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");

			try {
				request.UploadDataAsync(uri, WebRequestMethods.Ftp.UploadFile, File.ReadAllBytes(inputfilepath));
			}
			catch (System.Exception e) {
				Debug.Log(e);
			}
			Debug.Log("Successfully uploaded file");
		}
	}
}
