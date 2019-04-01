using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameCell : CellBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler {

	public static List<GameCell> cellsInSelection = new List<GameCell>();                                 //Global list of selected cells
	public static event Control.TeamChanged TeamChanged;
	public static GameCell lastEnteredCell = null;

	public bool isSelected = false;                                                                                 //Is cell in global selection ?

	public Coroutine generateCoroutine;

	public GameObject elementObj;

	protected SoundManager sound;


	//Add cell to global list
	public void Awake() {
		cellSprite = GetComponent<SpriteRenderer>();
		col = GetComponent<CircleCollider2D>();
		rg = GetComponent<Rigidbody2D>();
		uManager = GetComponent<Upgrade_Manager>();

		GameObject count = transform.Find("Count").gameObject;
		elementCountDisplay = count.GetComponent<TextMeshPro>();
		elementCountDisplayRenderer = count.GetComponent<MeshRenderer>();

		cellSelectedRenderer = transform.Find("Selected").GetComponent<SpriteRenderer>();

		Cell.CellRadius = col.radius * transform.localScale.x;

		PlayManager.cells.Add(this);
		if (Cell.CellTeam == Team.NEUTRAL) {
			PlayManager.neutralCells.Add(this);
		}
	}

	//Set default
	private void Start() {

		if (Cell.MaxElements == 0) {
			Cell.MaxElements = 50;
		}
		if (Cell.ElementCount == 0) {
			Cell.ElementCount = 10;
		}
		if (Cell.RegenPeriod == 0) {
			Cell.RegenPeriod = 0.3f;
		}
		if (Cell.CellRadius == 0) {
			Cell.CellRadius = col.radius * transform.localScale.x;
		}

		//UpdateCellInfo();
		StartCoroutine(ScaleCell());
		sound = GameObject.Find("GameManager").GetComponent<SoundManager>();
	}


	#region Cell Selection/Deselection
	//Checks the list whether the cell is already selected ? removes it : adds it
	public static void ModifySelection(GameCell cell) {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			if (cell == cellsInSelection[i]) {
				cellsInSelection.RemoveAt(i);
				cell.SetSelected();
				return;
			}
		}
		cellsInSelection.Add(cell);
		cell.SetSelected();
	}

	//Selects or deselects a cell
	public void SetSelected() {
		if (isSelected) {
			isSelected = false;
			elementCountDisplay.color = new Color32(255, 255, 255, 255);
			cellSelectedRenderer.enabled = false;
		}
		else {
			isSelected = true;
			elementCountDisplay.color = new Color32(255, 0, 0, 255);
			cellSelectedRenderer.enabled = true;
		}
	}

	//Resets Cell colour and clears the selection list
	public static void ClearSelection() {
		//print("Clearing");
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].elementCountDisplay.color = new Color(1, 1, 1);
			cellsInSelection[i].cellSelectedRenderer.enabled = false;
		}
		cellsInSelection.Clear();
	}
	#endregion

	//Wrapper for cell atacking
	public void AttackWrapper(GameCell target, Team team) {
		if (cellsInSelection.Count != 0) {
			if ((int)team >= 2) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
			else if (team == Team.ALLIED) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].EmpowerCell(target);
				}
			}
			else if (team == Team.NEUTRAL) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
		}
		ClearSelection();
	}

	#region Attack Handling

	//Code to attack selected cell
	public void AttackCell(GameCell target) {
		if (Cell.ElementCount > 1) {
			int numElements = Cell.ElementCount = Cell.ElementCount / 2;
			for (int i = 0; i < numElements; i++) {
				Element e = Instantiate(elementObj, ElementSpawnPoint(), Quaternion.identity).GetComponent<Element>();
				e.target = target;
				e.attacker = this;
				e.team = Cell.CellTeam;
				e.eSpeed = Cell.ElementSpeed;
			}
			UpdateCellInfo();
			sound.AddToSoundQueue(Cell.elementSpawn);
		}
	}

	//Called when "attacking" your own cell
	public void EmpowerCell(GameCell target) {
		if (Cell.ElementCount > 1 && target != this) {
			int numElements = Cell.ElementCount = Cell.ElementCount / 2;
			for (int i = 0; i < numElements; i++) {
				Element e = Instantiate(elementObj, ElementSpawnPoint(), Quaternion.identity).GetComponent<Element>();
				e.target = target;
				e.attacker = this;
				e.team = Cell.CellTeam;
				e.eSpeed = Cell.ElementSpeed;
			}
			UpdateCellInfo();
			sound.AddToSoundQueue(Cell.elementSpawn);
		}
	}
	#endregion

	#region Element Damage Handling
	//Called when an element enters a cell, isAllied ? feed the cell : damage the cell
	public void DamageCell(Element element, int amoutOfDamage, Upgrades[] additionalArgs) {

		#region Element Specific Code for Upgrade management -- Offensive Upgrades
		if (Cell.CellTeam == element.team) {
			float aidBonus = 0;
			for (int i = 0; i < uManager.upgrades.Length; i++) {
				if (uManager.upgrades[i] == Upgrades.DEF_AID_BONUS_CHANCE) {
					aidBonus += 0.2f;
				}
			}
			if (aidBonus != 0) {
				if (Random.Range(0f, 1f) < aidBonus) {
					Cell.ElementCount += 2;
				}
				else {
					Cell.ElementCount += 1;
				}
			}
			else {
				Cell.ElementCount += 1;
			}
			Destroy(element.gameObject);
			UpdateCellInfo();
			return;
		}

		int DOTStrength = 0;
		float critChance = 0;
		int slowRegenStrength = 0;

		for (int i = 0; i < additionalArgs.Length; i++) {
			switch (additionalArgs[i]) {
				case Upgrades.NONE: {
					//Nothing to Change
					break;
				}
				case Upgrades.ATK_DOT: {
					DOTStrength += 1;
					break;
				}
				case Upgrades.ATK_CRITICAL_CHANCE: {
					critChance += 0.2f;
					break;
				}
				case Upgrades.ATK_DOUBLE_DAMAGE: {
					//Nothing to Change
					break;
				}
				case Upgrades.ATK_SLOW_REGENERATION: {
					slowRegenStrength += 1;
					break;
				}
			}
		}
		if (DOTStrength != 0) {
			if (!Cell.appliedDebuffs.Contains(Upgrades.ATK_DOT)) {
				StartCoroutine(Cell.DoT(1, 4 * DOTStrength));
			}
		}
		if (critChance != 0) {
			if (Random.Range(0, 1) <= critChance) {
				amoutOfDamage *= 2;
			}
		}
		if (slowRegenStrength != 0) {
			if (!Cell.appliedDebuffs.Contains(Upgrades.ATK_SLOW_REGENERATION)) {
				Cell.appliedDebuffs.Add(Upgrades.ATK_SLOW_REGENERATION);
				Cell.RegenPeriod *= 1.33f;
			}
		}
		#endregion

		#region Cell Specific Code for Upgrade management -- Defensive upgrades
		float resistChance = 0;
		float reflectChance = 0;
		for (int i = 0; i < uManager.upgrades.Length; i++) {
			if (uManager.upgrades[i] != Upgrades.NONE && (int)uManager.upgrades[i] >= 100 && (int)uManager.upgrades[i] < 200) {
				switch (uManager.upgrades[i]) {
					case Upgrades.DEF_ELEMENT_RESIST_CHANCE: {
						resistChance += 0.1f;
						break;
					}
					case Upgrades.DEF_REFLECTION: {
						reflectChance += 0.1f;
						break;
					}
				}
			}
		}
		if (reflectChance != 0) {
			if (Random.Range(0f, 1f) < reflectChance) {
				element.Refelcted();
				return;
			}
		}
		if (resistChance != 0) {
			if (Random.Range(0f, 1f) < resistChance) {
				amoutOfDamage -= 1;
				Mathf.Clamp(amoutOfDamage, 0, float.MaxValue);
			}
		}

		#endregion

		Cell.ElementCount -= amoutOfDamage;

		if (Cell.ElementCount < 0) {
			if (TeamChanged != null) {
				TeamChanged(this, Cell.CellTeam, element.team);
			}
			if (isSelected) {
				ModifySelection(this);
			}
			Cell.ElementCount = -Cell.ElementCount;
			Cell.CellTeam = element.team;
		}
		Destroy(element.gameObject);
		UpdateCellInfo();
	}


	public Vector3 ElementSpawnPoint() {
		float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
		float x = Mathf.Sin(angle);
		float y = Mathf.Cos(angle);
		return new Vector3(transform.position.x + x * Cell.CellRadius * 0.70f, transform.position.y + y * Cell.CellRadius * 0.70f);
	}
	#endregion


	//Overriden function to include regeneration call
	public void UpdateCellInfo(bool calledFromBase = false) {

		if (calledFromBase == false) {
			//print(!isRegenerating && (cellTeam == enmTeam.ALLIED || (int)cellTeam >=2));

			if (!Cell.isRegenerating && (Cell.CellTeam == Team.ALLIED || (int)Cell.CellTeam >= 2)) {
				generateCoroutine = StartCoroutine(Cell.GenerateElements());
			}

			if (Cell.ElementCount > Cell.MaxElements) {
				Cell.Decay(0.5f, this);
			}
			if (isSelected) {
				cellSelectedRenderer.enabled = true;
			}
			else {
				cellSelectedRenderer.enabled = false;
			}
		}
	}

	Vector3 oldPos = Vector3.zero;
	private void FixedUpdate() {
		if (oldPos == Vector3.zero) {
			oldPos = transform.position;
		}

		Vector3 vel = transform.position - oldPos;

		if (vel != Vector3.zero) {
			UpdateCellInfo(false);
		}
	}

	private void Update() {
		if (Input.GetMouseButtonUp(0) && lastEnteredCell == this) {
			AttackWrapper(lastEnteredCell, lastEnteredCell.Cell.CellTeam);
		}
	}

	private void OnMouseOver() {
		#region Cell Debug - Change regen speed and max size by hovering over the cell and pressing "8" to increase max count or "6" to increase regenSpeed, opposite buttons decrease the values
		//Adjust max cell size
		if (Input.GetKeyDown(KeyCode.Keypad8)) {
			if (Cell.MaxElements < 100) {
				Cell.MaxElements++;
				print(Cell.MaxElements);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			if (Cell.MaxElements >= 2) {
				Cell.MaxElements--;
				print(Cell.MaxElements);
			}
		}

		//Adjust cell regeneration rate
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			if (Cell.RegenPeriod < 10) {
				Cell.RegenPeriod += 0.5f;
				print(Cell.RegenPeriod);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (Cell.RegenPeriod > 0.5f) {
				Cell.RegenPeriod -= 0.5f;
				print(Cell.RegenPeriod);
			}
		}
		#endregion
	}

	public void OnPointerEnter(PointerEventData eventData) {

		Background.onReleaseClear = false;
		lastEnteredCell = this;

		if (Input.GetMouseButton(0) && Cell.CellTeam == Team.ALLIED) {
			ModifySelection(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		//print("Click " + gameObject.name);
		ClearSelection();
	}

	public void OnPointerDown(PointerEventData eventData) {
		if (Cell.CellTeam == Team.ALLIED) {
			ModifySelection(this);
		}
	}
}