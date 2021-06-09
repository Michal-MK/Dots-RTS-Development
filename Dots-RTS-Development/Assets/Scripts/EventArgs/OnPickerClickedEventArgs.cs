using System;
public class OnPickerClickedEventArgs : EventArgs {

	public OnPickerClickedEventArgs(UpgradeSlot slot, EditorUpgradePicker instance) {
		Slot = slot;
		Instance = instance;
	}

	public UpgradeSlot Slot { get; }

	public EditorUpgradePicker Instance { get; }

}
