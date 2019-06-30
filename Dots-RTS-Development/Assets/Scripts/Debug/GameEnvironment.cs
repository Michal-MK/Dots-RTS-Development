public static class GameEnvironment {

	static GameEnvironment() {
#if UNITY_EDITOR
		IsEditor = true;
#elif UNITY_STANDALONE
			IsStandalone = true;
#elif UNITY_ANDROID
			IsAndroid = true;
#endif
	}

	public static bool IsEditor { get; } = false;

	public static bool IsStandalone { get; } = false;

	public static bool IsAndroid { get; } = false;


}
