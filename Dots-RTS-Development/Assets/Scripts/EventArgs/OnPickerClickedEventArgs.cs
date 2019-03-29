using System;
public class OnPickerClickedEventArgs : EventArgs {

	public OnPickerClickedEventArgs(UpgradeSlot slot, UpgradePickerInstance instance) {
		Slot = slot;
		Instance = instance;
	}

	public UpgradeSlot Slot { get; set; }

	public UpgradePickerInstance Instance { get; set; }

}
