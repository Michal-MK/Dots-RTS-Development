using System;

public class OnUpgradeSlotClickedEventArgs : EventArgs {
	public OnUpgradeSlotClickedEventArgs(UpgradeSlot slot, int slotID) {
		Slot = slot;
		SlotID = slotID;
	}

	public UpgradeSlot Slot { get; }

	public int SlotID { get; }
}
