using System;

public class OnProfileInfoDeletedEventArgs : EventArgs {

	public OnProfileInfoDeletedEventArgs(ProfileInfo info) {
		Deleted = info;
	}

	public ProfileInfo Deleted { get; }
}