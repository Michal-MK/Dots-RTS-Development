using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ServerAccesss {
	public bool isDownloading = false;
	public string downloadedFile = "";

	public string GetFile(string filePath) {
		DownloadFileFTP(filePath, true);
		return downloadedFile;
	}


	public List<string> GetContents() {
		List<string> contents = new List<string>();

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://kocicka.endora.cz/");

		request.Credentials = new NetworkCredential("phage", "Abcd123");
		request.Method = WebRequestMethods.Ftp.ListDirectory;

		try {
			FtpWebResponse response = (FtpWebResponse)request.GetResponse();
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
		catch (System.Exception e) {
			Debug.Log(e);
		}
		return contents;
	}

	string inputfilepath;

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void DownloadFileFTP(string fileName, bool temp = false) {
		isDownloading = true;

		if (temp) {
			inputfilepath = Application.temporaryCachePath + "\\";
		}
		else {
			inputfilepath = Application.streamingAssetsPath + "\\Saves\\";
		}
		inputfilepath += fileName;

		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");
			try {
				request.DownloadDataCompleted += Request_DownloadDataCompleted;
				request.DownloadDataAsync(new System.Uri("ftp://kocicka.endora.cz/" + fileName));
			}
			catch (System.Exception e) {
				Debug.Log(e);
			}
			Debug.Log("Successfully downloaded file");
		}
	}

	public string[] GetLevelInfo() {

		string[] info = new string[3];

		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fileS = File.Open(downloadedFile, FileMode.Open)) {
			SaveData s = (SaveData)bf.Deserialize(fileS);

			fileS.Close();

			info[0] = s.levelInfo.levelName;
			info[1] = s.levelInfo.creator;
			info[2] = string.Format("{0:dd/MM/yy H:mm:ss}", s.levelInfo.creationTime);

		}
		return info;
	}

	//public IEnumerator GetLevelInfo(string path) {
	//	string filePath = GetFile(path);
	//	yield return new WaitUntil(() => !isDownloading);
	//	saveInfo = GetLevelInfo();
	//}

	public IEnumerator GetLevelInfo(string path) {
		string filePath = GetFile(path);
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
		//yield return GetLevelInfo();
	}




	private void Request_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e) {
		using (FileStream file = File.Create(inputfilepath)) {
			file.Write(e.Result, 0, e.Result.Length);
			downloadedFile = inputfilepath;
			isDownloading = false;
			file.Close();
		}

	}

	/// <summary>
	/// Uploads file to server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void UploadFileFTP(string fileName) {
		string inputfilepath = Application.streamingAssetsPath + "\\Saves\\" + fileName;
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
