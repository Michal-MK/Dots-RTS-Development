public static class GameEnvironment {

	static GameEnvironment() {
#if UNITY_EDITOR
		IsEditor = true;
#elif UNITY_STANDALONE
			_inStandalone = true;
#elif UNITY_ANDROID
			_onAndroid = true;
#endif
	}

	public static bool IsEditor { get; } = false;

	public static bool IsStandalone { get; } = false;

	public static bool IsAndroid { get; } = false;


}
