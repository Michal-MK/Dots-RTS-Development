using System.IO;
using UnityEngine;

public class Paths {

	public const string SAVE_EXT = ".pwl";
	
	public static readonly string STREAMED_RES = Path.Combine(Application.streamingAssetsPath, "Resources");
	public static readonly string SAVES = Path.Combine(Application.persistentDataPath, "Saves");
	public static readonly string SAVES_CACHE = Path.Combine(Application.temporaryCachePath, "Saves");

	public static string StreamedResource(string identifier) => Path.Combine(STREAMED_RES, identifier);
	public static string SavedLevel(string name) => Path.Combine(SAVES, name);
	public static string CachedLevel(string name) => Path.Combine(SAVES_CACHE, name);
}