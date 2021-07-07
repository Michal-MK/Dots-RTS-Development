using System;
using UnityEngine.SceneManagement;

public class SceneChangedEventArgs : EventArgs {
	public string Name { get; }
	
	public LoadSceneMode Mode { get; }

	public SceneChangedEventArgs(string name, LoadSceneMode mode) {
		Name = name;
		Mode = mode;
	}
}
