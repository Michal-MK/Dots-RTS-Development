using System;
using UnityEngine.SceneManagement;

public class SceneChangedEventArgs : EventArgs {
	public SceneChangedEventArgs(string name, LoadSceneMode mode) {
		Name = name;
		Mode = mode;
	}

	public string Name { get; }

	public LoadSceneMode Mode { get; }
}
