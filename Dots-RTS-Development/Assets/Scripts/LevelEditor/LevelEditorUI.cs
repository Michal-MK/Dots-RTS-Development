using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour {
	//Cell modification panel
	public InputField startingElementCount;
	public InputField maxElementCount;
	public InputField regenerationSpeed;
	public Button activeTeamButton;

	//GameSetttingPanel
	public InputField aiDifficultyAllInput;
	public InputField aiDifficultySingleInput;
	public InputField sizeInput;

	//ExportPanel
	public InputField levelNameInput;
	public InputField authorNameInput;
	public Button cellTeam;

	//EditorRefs
	public GameObject savePanel;
	public GameObject gameSettingsPanel;
	public GameObject cellModificationInputs;
	public Transform uiUpgradeSlots;
	public MovePanel cellPanelHandle;
	public Animator slideInPanel;
	public GameObject teamPickerButtons;
	public GameObject upgradePickerButtons;

	public GameObject menuPanel;
	public GameObject upgradeSelector;
}
