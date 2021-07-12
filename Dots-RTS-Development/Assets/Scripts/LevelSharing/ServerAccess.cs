using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerAccess {
	public async Task<List<string>> GetLevelsAsync() {
		HttpWebRequest request = WebRequest.CreateHttp(new Uri("http://192.168.88.5:5000/Level"));

		try {
			using WebResponse response = await request.GetResponseAsync();
			using StreamReader s = new StreamReader(response.GetResponseStream());
			string content = await s.ReadToEndAsync();
			JArray objects = JArray.Parse(content);
			return objects.ToObject<List<string>>();
		}
		catch (Exception e) {
			Debug.Log(e);
		}
		return null;
	}

	/// <summary>
	/// Gets a file from a server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public async Task<SaveData> DownloadAsync(string fileName, bool temp = false) {

		string hostname = "http://192.168.88.5:5000/Level/Level?name=" + fileName;

		HttpWebRequest req = WebRequest.CreateHttp(hostname);

		try {
			WebResponse response = await req.GetResponseAsync();
			Debug.Log(fileName);

			using Stream s = response.GetResponseStream();

			string inputFilePath;
			if (temp) {
				inputFilePath = Paths.CachedLevel(fileName);
			}
			else {
				inputFilePath = Paths.SavedLevel(fileName);
			}
			using FileStream file = File.Create(inputFilePath);
			byte[] buffer = new byte[4096];
			int readCount = await s.ReadAsync(buffer, 0, buffer.Length);
			while (readCount > 0) {
				await file.WriteAsync(buffer, 0, readCount);
				readCount = await s.ReadAsync(buffer, 0, buffer.Length);
			}
			Debug.Log("Successfully downloaded file" + file.Name + "  - Async");
			return new SaveData() { SaveMeta = new SaveMeta(fileName, "Test", DateTime.Now) };
		}
		catch (Exception e) {
			Debug.LogError(e);
			return default;
		}
	}

	/// <summary>
	/// Uploads file to server
	/// </summary>
	/// <param name="fileName">Send just the Object's name, not the full path.</param>
	public void UploadLevel(string fileName) {

		string inputFilePath = Paths.SavedLevel(fileName);
		string hostname = $"http://192.168.88.5:5000/Level/UploadLevel?name={fileName}&levelData={File.ReadAllText(inputFilePath)}";

		HttpWebRequest req = WebRequest.CreateHttp(hostname);
		req.Method = WebRequestMethods.Http.Post;
		req.ContentLength = 0;
		
		HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
		Debug.Log(resp.StatusCode);
	}
}