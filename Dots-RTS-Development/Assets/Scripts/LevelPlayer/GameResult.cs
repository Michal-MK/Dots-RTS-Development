using System;

public class GameResult {
	public PlaySceneState State { get; set; }
	public bool isWinner { get; set; }
	public TimeSpan GameplayTime { get; set; }
	public bool IsDomination { get; set; }
}
