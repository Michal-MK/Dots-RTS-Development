using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellScript : MonoBehaviour {

	private int _count;                                                                         //Current amount of elements inside the cell
	private float _regenSpeed;                                                                  //How fast will the cell regenerate
	public float _radius;
	private int _maxCount;                                                                      //How much can the cell hold
	private Vector2 _position;                                                                  //Cells position
	public enmTeam team;                                                                        //Cell's team
	public enum enmTeam {
		NEUTRAL,
		ALLIED,
		ENEMY,
	}

	private Color32 enemy = new Color32(255, 0, 0, 255);
	private Color32 ally = new Color32(0, 255, 0, 255);
	private Color32 neutral = new Color32(255, 255, 255, 255);

	Coroutine generation;


	public bool isSelected = false;                                                             //Is cell selected for attack?
	public Upgrade_Manager um;
	public TextMesh textMesh;
	public MeshRenderer textRenderer;
	public SpriteRenderer cellSprite;
	public GameObject cellObj;
	public GameObject elementObj;


	public static event GameControll.TeamChangeEventHandler TeamChanged;

	//Call to set cell attributes
	public void SetCellData(Vector2 position, enmTeam cellTeam, int startingCount = 0, int maximum = 100, float regenerationRate = 2f) {
		gameObject.transform.position = position;
		_count = startingCount;
		_maxCount = maximum;
		_regenSpeed = regenerationRate;
		team = cellTeam;
		if (team != enmTeam.NEUTRAL) {
			generation = StartCoroutine(Generate());
		}
	}

	//Debug start generation
	private void Start() {
		_count = 10;
		_maxCount = 50;
		_regenSpeed = 2f;

		if (team != enmTeam.NEUTRAL) {
			generation = StartCoroutine(Generate());
		}
		GameControll.cells.Add(this);
		UpdateCellInfo();
	}

	#region Cell data altering code
	//Call to alter cell regeneration rate
	public void AlterRegen(float newRegenSpeed) {
		if (_regenSpeed == newRegenSpeed) {
			print("Already set to " + newRegenSpeed + " speed, returning.");
			return;
		}
		else {
			_regenSpeed = newRegenSpeed;
		}
	}

	//Call to alter cell size
	public void AlterCellMax(int newMaximum) {
		if (_maxCount == newMaximum) {
			print("Already set to " + newMaximum + " capacity, returning.");
		}
		else {
			_maxCount = newMaximum;
		}
	}

	//Chenge cells team
	public void AlterTeam(enmTeam t) {
		if (TeamChanged != null) {
			TeamChanged(this, team, t);
		}
		if (t == team) {
			print("Already set to team " + t + ", returning.");
		}
		else {
			team = t;
		}
	}
	//Change cells starting count
	public void AlterStartingElementCount(int elements) {
		if (elements == _count) {
			print("Already set to start with " + elements + ", returning.");
		}
		else {
			_count = elements;
		}

	}
	#endregion

	//Keeps generateing new elements for the cell
	public IEnumerator Generate() {
		while (true) {
			yield return new WaitForSecondsRealtime(_regenSpeed);
			if (_count < _maxCount) {
				_count++;
				textMesh.text = _count.ToString();
			}
		}
	}

	public int GetElements {
		get {
			return _count;
		}
		set {
			_count = value;
		}
	}


	//Selects or deselects a cell
	public void SetSelected() {

		if (isSelected) {
			isSelected = false;
		}
		else {
			isSelected = true;
			textMesh.color = new Color32(255, 0, 0, 255);
		}
	}

	//Code to attack selected cell
	public void AttackCell(CellScript target) {
		int numElements = _count = _count / 2;
		for (int i = 0; i < numElements; i++) {
			GameObject e = Instantiate(elementObj, gameObject.transform.position, Quaternion.identity);
			e.GetComponent<Element>().target = target;
			e.GetComponent<Element>().attacker = this;
		}
		UpdateCellInfo();

		//target._count -= numElements; Deprecated code for attacking -- not functional with elemets logic
		//if (target._count < 0) {
		//	//target._count = -target._count;
		//	switch (target.team) {
		//		case enmTeam.ALLIED: {
		//			//Attacking a cell as an enemy + losing all sotored elements causes it to become an enemy
		//			target.team = enmTeam.ENEMY;
		//			target.cellSprite.color = enemy;
		//			textMesh.text = _count.ToString();
		//			target.textMesh.text = target._count.ToString();
		//			return;
		//		}
		//		case enmTeam.ENEMY: {
		//			//Attacking a cell as an ally + losing all sotored elements causes it to become an ally
		//			target.team = enmTeam.ALLIED;
		//			target.cellSprite.color = ally;
		//			textMesh.text = _count.ToString();
		//			target.textMesh.text = target._count.ToString();
		//			return;
		//		}
		//		case enmTeam.NEUTRAL: {
		//			//Attacking a cell causes it to join the same team as the attacker / generation begins
		//			if (this.team == enmTeam.ALLIED) {
		//				print(this.team + "  " + target.team);
		//				target.team = enmTeam.ALLIED;
		//				target.cellSprite.color = ally;
		//				target.StartCoroutine(target.Generate());
		//			}
		//			else if (this.team == enmTeam.ENEMY) {
		//				print(this.team + "  " + target.team);
		//				target.team = enmTeam.ENEMY;
		//				target.cellSprite.color = enemy;
		//				target.StartCoroutine(target.Generate());
		//			}
		//			textMesh.text = _count.ToString();
		//			target.textMesh.text = target._count.ToString();
		//			return;
		//		}
		//	}
		//}
		//else {
		//	textMesh.text = _count.ToString();
		//	target.textMesh.text = target._count.ToString();
		//}
	}

	//Called when "attacking" your own cell
	public void EmpowerCell(CellScript target) {
		int numElements = _count = _count / 2;
		for (int i = 0; i < numElements; i++) {
			GameObject e = Instantiate(elementObj, gameObject.transform.position, Quaternion.identity);
			e.GetComponent<Element>().target = target;
			e.GetComponent<Element>().attacker = this;
		}
		UpdateCellInfo();
	}

	//Called when an element enters a cell, isAllied ? feed the cell : damage the cell
	public void DamageCell(enmTeam elementTeam) {
		if (team == elementTeam) {
			_count++;
			UpdateCellInfo();
			return;
		}
		_count--;
		if (_count < 0) {
			_count = -_count;
			AlterTeam(elementTeam);
		}
		UpdateCellInfo();
	}

	//Update Cell info and rendering
	public void UpdateCellInfo() {

		textMesh.text = _count.ToString();
		textRenderer.sortingOrder = 2;
		_radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
		if (generation == null && team != enmTeam.NEUTRAL) {
			generation = StartCoroutine(Generate());
		}
		switch (team) {
			case enmTeam.ALLIED: {
				cellSprite.color = ally;
				return;
			}
			case enmTeam.ENEMY: {
				cellSprite.color = enemy;

				return;
			}
			case enmTeam.NEUTRAL: {
				cellSprite.color = neutral;
				return;
			}
		}
	}

	//Changes colour when howered over
	private void OnMouseOver() {
		if (!isSelected) {
			textMesh.color = new Color32(255, 0, 0, 255);
		}
		else {
			textMesh.color = new Color32(0, 255, 255, 255);
		}
		//OnMouseDown() doesn't support other mouse imputs except LMB .. lame 
		if (Input.GetMouseButtonDown(1)) {
			if (team == enmTeam.ALLIED) {
				CellBehaviour.AttackCell(this, enmTeam.ALLIED);
			}
		}
		#region Cell Debug - Change regen speed and max size by hovering over the cell and pressing "8" to increase max count or "6" to increase regenSpeed, opposite buttons decrease the values
		//Adjust max cell size
		if (Input.GetKeyDown(KeyCode.Keypad8)) {
			if (_maxCount < 100) {
				_maxCount++;
				print(_maxCount);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad2)) {
			if (_maxCount >= 2) {
				_maxCount--;
				print(_maxCount);
			}
		}

		//Adjust cell regeneration rate
		if (Input.GetKeyDown(KeyCode.Keypad6)) {
			if (_regenSpeed < 10) {
				_regenSpeed += 0.5f;
				print(_regenSpeed);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (_regenSpeed > 0.5f) {
				_regenSpeed -= 0.5f;
				print(_regenSpeed);
			}
		}
		#endregion
	}

	//Changes the colour back to original
	private void OnMouseExit() {
		if (!isSelected) {
			textMesh.color = new Color32(255, 255, 255, 255);
		}
		else {
			textMesh.color = new Color32(255, 0, 0, 255);
		}
	}

	//Determine action depending on clicked cell's team.
	private void OnMouseDown() {
		switch (team) {
			case enmTeam.ALLIED: {
				CellBehaviour.ModifySelection(this);
				return;
			}
			case enmTeam.ENEMY: {
				CellBehaviour.AttackCell(this, team);
				return;
			}
			case enmTeam.NEUTRAL: {
				CellBehaviour.AttackCell(this, enmTeam.ENEMY);
				return;
			}
		}
	}
}