using System;

public class WindowChangeEventArgs : EventArgs {

	public WindowChangeEventArgs(Window changed, bool isOpening) {
		Changed = changed;
		IsOpening = isOpening;
	}
	
	public Window Changed { get; }
	public bool IsOpening { get; }
}
