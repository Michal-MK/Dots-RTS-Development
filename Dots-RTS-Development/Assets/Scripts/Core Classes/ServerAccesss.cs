using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ServerAccesss {
	public bool isDownloading = false;

	//public string[] ServerAccess(string filePath){
	//	DownloadFileFTP(filePath,true);
	//	FileStream file = File.Open(Application.temporaryCachePath + "\\" + filePath, FileMode.Open);
	//	return GetLevelInfo(file);
	//}

	public string downloadedFile = "";




	public string GetFile(string filePath) {
		DownloadFileFTP(filePath, true);
		return downloadedFile;
	}

	public List<string> GetContents() {
		//string inputfilepath = Application.streamingAssetsPath + "\\Downloads";

		string ftpfullpath = "ftp://" + "kocicka.endora.cz/";

		FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpfullpath);

		request.Credentials = new NetworkCredential("phage", "Abcd123");
		request.Method = WebRequestMethods.Ftp.ListDirectory;

		List<string> contents = new List<string>();

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

		foreach (string s in contents) {
			Debug.Log(s);
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
		string ftpfullpath = "ftp://kocicka.endora.cz/" + fileName;
		System.Uri uri = new System.Uri(ftpfullpath);

		using (WebClient request = new WebClient()) {
			request.Credentials = new NetworkCredential("phage", "Abcd123");
			try {
				request.DownloadDataCompleted += Request_DownloadDataCompleted;
				request.DownloadDataAsync(uri);

			}
			catch (System.Exception e) {
				Debug.Log(e);
			}
			Debug.Log("Success");
		}
	}

	public string[] GetLevelInfo() {

		string[] info = new string[3];

		BinaryFormatter bf = new BinaryFormatter();
		using (FileStream fileS = File.Open(downloadedFile, FileMode.Open)) {
			SaveData s = (SaveData)bf.Deserialize(fileS);

			fileS.Close();

			Debug.Log(s.levelInfo.levelName + "----------------------------------------");
			info[0] = s.levelInfo.levelName;
			info[1] = s.levelInfo.creator;
			info[2] = "time placeholder";
		}
		return info;
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
		}
	}
}
