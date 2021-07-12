using System;
using System.Diagnostics.CodeAnalysis;

[Serializable]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "JsonUtil class uses files only, this is deo to mimic property syntax")]
public class UpgradeData {
	public int ID;

	public string Name;

	public string FunctionName;

	public string Description;
}