using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

	private int _count;
	private float _regenSpeed;
	private int _maxCount;
	private Vector2 _position;
	public team _team;
	public enum team {
		NEUTRAL,
		ALLIED,
		ENEMY,
	}

	private Color32 enemy = new Color32(255, 0, 0, 255);
	private Color32 ally = new Color32(0, 255, 0, 255);
	private Color32 neutral = new Color32(255, 255, 255, 255);

	Coroutine generation;


	public bool isSelected = false;

	public TextMesh textMesh;
	public SpriteRenderer cellSprite;
	public GameObject cellObj;

	//Call to set cell attributes
	public void SetCellData(Vector2 position, team team, int startingCount = 0, int maximum = 100, float regenerationRate = 2f) {
		gameObject.transform.position = position;
		_count = startingCount;
		_maxCount = maximum;
		_regenSpeed = regenerationRate;
		_team = team;

		if (_team != team.NEUTRAL) {
			generation = StartCoroutine(Generate());
		}
	}

	//Debug start generation
	private void Start() {
		if (generation == null) {
			_count = 10;
			_maxCount = 50;
			_regenSpeed = 2f;
			if (this._team != team.NEUTRAL) {
				generation = StartCoroutine(Generate());
			}
		}
		textMesh.text = _count.ToString();
		textMesh.gameObject.GetComponent<MeshRenderer>().sortingOrder = 2;
		switch (_team) {
			case team.ALLIED: {
				cellSprite.color = ally;
				return;
			}
			case team.ENEMY: {
				cellSprite.color = enemy;
				return;
			}
			case team.NEUTRAL: {
				cellSprite.color = neutral;
				return;
			}
		}
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
		if(_maxCount == newMaximum) {
			print("Already set to " + newMaximum + " capacity, returning.");
		}
		else {
			_maxCount = newMaximum;
		}
	}

	public void AlterTeam(team t) {
		if (t == _team) {
			print("Already set to " + t + " capacity, returning.");
		}
		else {
			_team = t;
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

	public void AttackCell(CellScript target) {
		int numElements = _count = _count / 2;
		target._count -= numElements;
		if (target._count < 0) {
			target._count = -target._count;
			switch (target._team) {
				case team.ALLIED: {
					target._team = team.ENEMY;
					target.cellSprite.color = enemy;
					textMesh.text = _count.ToString();
					target.textMesh.text = target._count.ToString();
					return;
				}
				case team.ENEMY: {
					target._team = team.ALLIED;
					target.cellSprite.color = ally;
					textMesh.text = _count.ToString();
					target.textMesh.text = target._count.ToString();
					return;
				}
				case team.NEUTRAL: {
					if (this._team == team.ALLIED) {
						print(this._team + "  " + target._team);
						target._team = team.ALLIED;
						target.cellSprite.color = ally;
						target.StartCoroutine(target.Generate());
					}
					else if (this._team == team.ENEMY) {
						print(this._team + "  " + target._team);
						target._team = team.ENEMY;
						target.cellSprite.color = enemy;
						target.StartCoroutine(target.Generate());

					}
					textMesh.text = _count.ToString();
					target.textMesh.text = target._count.ToString();
					return;
				}
			}
		}
		else {
			textMesh.text = _count.ToString();
			target.textMesh.text = target._count.ToString();
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
				_regenSpeed += 0.1f;
				print(_regenSpeed);
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			if (_regenSpeed > 0.5f) {
				_regenSpeed -= 0.1f;
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
		if (Input.GetMouseButtonDown(0)) {
			switch (_team) {
				case team.ALLIED: {
					CellBehaviour.ModifySelection(this);
					return;
				}
				case team.ENEMY: {
					CellBehaviour.AttackCell(this);
					return;
				}
				case team.NEUTRAL: {
					CellBehaviour.AttackCell(this);
					return;
				}
			}
		}
	}
}