using System.IO;
using UnityEngine;

public class Crash : MonoBehaviour {

	public void SavePic() {
		SavePicture(Application.streamingAssetsPath + Path.DirectorySeparatorChar + "Pic.Png");
	}


	public static void SavePicture(string path) {

		Texture2D tex = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight);
		Rect r = Camera.main.pixelRect;
		Rect r2 = new Rect(Camera.main.transform.position + new Vector3(0, 200, 10), new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));
		print(r.size);
		print(r2.size);

		tex.ReadPixels(r2, 0, 0);
		File.WriteAllBytes(path, tex.EncodeToPNG());
	}
}
