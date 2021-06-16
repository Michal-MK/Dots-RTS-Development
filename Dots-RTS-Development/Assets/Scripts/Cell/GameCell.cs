using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameCell : CellBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler {

	public static List<GameCell> cellsInSelection = new List<GameCell>();
	public static event EventHandler<CellTeamChangeEventArgs> TeamChanged;
	public event EventHandler<GameCell> OnSelectionAttempt;
	public static GameCell lastEnteredCell = null;

	public bool isSelected = false;

	public Coroutine generateCoroutine;

	public GameObject elementObj;

	protected SoundManager sound;


	//Add cell to global list
	public void Awake() {
		cellSprite = GetComponent<SpriteRenderer>();
		col = GetComponent<CircleCollider2D>();
		rg = GetComponent<Rigidbody2D>();
		uManager = GetComponent<UpgradeManager>();

		GameObject count = transform.Find("Count").gameObject;
		elementCountDisplay = count.GetComponent<TextMeshPro>();
		elementCountDisplayRenderer = count.GetComponent<MeshRenderer>();

		cellSelectedRenderer = transform.Find("Selected").GetComponent<SpriteRenderer>();

		Cell.cellRadius = col.radius * transform.localScale.x;
		Cell.coroutineRunner = this;

		Cell.OnElementDecayed += (s, e) => { UpdateVisual(); };
		Cell.OnElementGenerated += (s, e) => { UpdateVisual(); };
	}

	private void Start() {
		if (Cell.maxElements == 0) {
			Cell.maxElements = 50;
		}
		if (Cell.elementCount == 0) {
			Cell.elementCount = 10;
		}
		if (Cell.regenPeriod == 0) {
			Cell.regenPeriod = 0.3f;
		}
		if (Cell.cellRadius == 0) {
			Cell.cellRadius = col.radius * transform.localScale.x;
		}

		StartCoroutine(ScaleCell());
		sound = GameObject.Find("GameManager").GetComponent<SoundManager>();
	}

	#region Cell Selection/Deselection

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

	public void SetSelected() {
		isSelected ^= true;
		cellSelectedRenderer.enabled ^= true;
		elementCountDisplay.color = elementCountDisplay.color == Color.white ? Color.red : Color.white;
	}

	public static void ClearSelection() {
		for (int i = 0; i < cellsInSelection.Count; i++) {
			cellsInSelection[i].isSelected = false;
			cellsInSelection[i].elementCountDisplay.color = Color.white;
			cellsInSelection[i].cellSelectedRenderer.enabled = false;
		}
		cellsInSelection.Clear();
	}

	#endregion

	//Wrapper for cell attacking
	public void AttackWrapper(GameCell target, Team team) {
		if (cellsInSelection.Count != 0) {
			if (team >= Team.ENEMY1) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
				}
			}
			else if (team == Team.ALLIED) {
				for (int i = 0; i < cellsInSelection.Count; i++) {
					cellsInSelection[i].AttackCell(target);
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

	public void AttackCell(GameCell target) {
		if (Cell.elementCount > 1 && target != this) {
			int numElements = Cell.elementCount /= 2;

			for (int i = 0; i < numElements; i++) {
				Element e = Instantiate(elementObj, transform.position, Quaternion.identity).GetComponent<Element>();
				e.target = target;
				e.attacker = this;
				e.team = Cell.team;
				e.speed = Cell.elementSpeed;
			}
			UpdateCellInfo();
			sound.PlaySound(Cell.elementSpawn);
		}
	}

	#region Element Damage Handling

	public void DamageCell(Element element, int amoutOfDamage, Upgrades[] additionalArgs) {

		#region Element Specific Code for Upgrade management -- Offensive Upgrades
		if (Cell.team == element.team) {
			float aidBonus = 0;
			for (int i = 0; i < uManager.upgrades.Length; i++) {
				if (uManager.upgrades[i] == Upgrades.DEF_AID_BONUS_CHANCE) {
					aidBonus += 0.2f;
				}
			}
			if (aidBonus != 0) {
				if (Random.Range(0f, 1f) < aidBonus) {
					Cell.elementCount += 2;
				}
				else {
					Cell.elementCount += 1;
				}
			}
			else {
				Cell.elementCount += 1;
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
				Cell.regenPeriod *= 1.33f;
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
				element.Reflected();
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

		Cell.elementCount -= amoutOfDamage;

		if (Cell.elementCount < 0) {
			TeamChanged?.Invoke(this, new CellTeamChangeEventArgs(this, Cell.team, element.team));

			if (isSelected) {
				ModifySelection(this);
			}

			Cell.elementCount = -Cell.elementCount;
			Cell.team = element.team;
		}

		Destroy(element.gameObject);
		sound.PlaySound(Cell.elementAttack);
		UpdateCellInfo();
	}

	#endregion

	public void UpdateCellInfo() {
		if (!Cell.isRegenerating && Cell.team >= Team.ALLIED) {
			generateCoroutine = StartCoroutine(Cell.GenerateElements());
		}
		if (Cell.elementCount > Cell.maxElements) {
			Cell.Decay(0.5f, this);
		}
		cellSelectedRenderer.enabled = isSelected;

		cellSprite.color = CellColours.GetColor(Cell.team);
		elementCountDisplay.text = Cell.elementCount.ToString();
	}

	private Vector3 oldPos = Vector3.zero;
	private void FixedUpdate() {
		if (oldPos == Vector3.zero) {
			oldPos = transform.position;
		}

		Vector3 vel = transform.position - oldPos;

		if (vel != Vector3.zero) {
			UpdateCellInfo();
		}
	}

	private void Update() {
		if (Input.GetMouseButtonUp(0) && lastEnteredCell == this) {
			AttackWrapper(lastEnteredCell, lastEnteredCell.Cell.team);
		}
	}

	private void OnMouseOver() {
		#region Cell Debug - Change regen speed and max size by hovering over the cell and pressing "8" to increase max count or "6" to increase regenSpeed
		if (Input.GetKeyDown(KeyCode.Keypad8)) {
			if (Cell.maxElements < 100) {
				Cell.maxElements++;
				print(Cell.maxElements);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			if (Cell.maxElements >= 2) {
				Cell.maxElements--;
				print(Cell.maxElements);
			}
		}

		//Adjust cell regeneration rate
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			if (Cell.regenPeriod < 10) {
				Cell.regenPeriod += 0.5f;
				print(Cell.regenPeriod);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (Cell.regenPeriod > 0.5f) {
				Cell.regenPeriod -= 0.5f;
				print(Cell.regenPeriod);
			}
		}
		#endregion
	}

	public void OnPointerEnter(PointerEventData eventData) {
		Background.onReleaseClear = false;
		lastEnteredCell = this;

		if (Input.GetMouseButton(0) && Cell.team == Team.ALLIED) {
			OnSelectionAttempt?.Invoke(this, this);
			ModifySelection(this);
		}
	}

	public void OnPointerClick(PointerEventData eventData) {
		ClearSelection();
	}

	public void OnPointerDown(PointerEventData eventData) {
		OnSelectionAttempt?.Invoke(this, this);
		if (Cell.team == Team.ALLIED) {
			ModifySelection(this);
		}
	}
}
