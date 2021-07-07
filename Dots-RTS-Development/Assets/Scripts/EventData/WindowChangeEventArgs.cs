using System;

public class WindowChangeEventArgs : EventArgs {

	public Window Changed { get; set; }
	public bool IsOpening { get; set; }
}
