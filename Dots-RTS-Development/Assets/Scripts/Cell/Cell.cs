using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cell : MonoBehaviour {



	private int _elementCount;                                                                  //Current amount of elements inside the cell
	private float _regenP;																		//How fast will the cell regenerate
	private int _maxElementCount;                                                               //How much can the cell hold
	private Vector2 _position;                                                                  //Cells position
	private bool _isRegenerating = false;
	public enmTeam _team;                                                                       //Cell's team

	public enum enmTeam {
		NEUTRAL,
		ALLIED,
		ENEMY,
	}

	private float _radius;

	public Color32 enemy = new Color32(255, 0, 0, 255);                                        //Default enemy colour
	public Color32 ally = new Color32(0, 255, 0, 255);                                         //Default ally colour
	public Color32 neutral = new Color32(255, 255, 255, 255);                                  //Default neutral colour

	public Upgrade u;


	public SpriteRenderer cellSprite;
	public TextMesh elementNrDisplay;
	public MeshRenderer textRenderer;


	public void UpdateCellInfo() {
		if (elementCount >= 10 && elementCount <= maxElements) {
			float mappedValue = Map.MapFloat(elementCount, 10, maxElements, 1, 3);

			transform.localScale = new Vector3(mappedValue, mappedValue, 1);
			cellRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
		}

		if(SceneManager.GetActiveScene().name == "Game") {
			StartGeneration();
		}

		elementNrDisplay.text = elementCount.ToString();
		textRenderer.sortingOrder = 2;

		switch (cellTeam) {
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

	public void StartGeneration() {
		if (!_isRegenerating && _team == enmTeam.ALLIED || !_isRegenerating && _team == enmTeam.ENEMY) {
			StartCoroutine(GenerateElements());
		}
	}

	//Keeps generateing new elements for the cell
	public IEnumerator GenerateElements() {
		_isRegenerating = true;
		while (true) {
			yield return new WaitForSecondsRealtime(regenPeriod);
			if (elementCount < maxElements) {
				elementCount++;
				elementNrDisplay.text = elementCount.ToString();
				UpdateCellInfo();
			}
		}
	}

	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int elementCount {
		get { return _elementCount; }
		set { _elementCount = value; UpdateCellInfo(); }
	}

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float regenPeriod {
		get { return _regenP; }
		set { _regenP = value; UpdateCellInfo(); }
	}
	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int maxElements {
		get { return _maxElementCount; }
		set { _maxElementCount = value; UpdateCellInfo(); }
	}
	/// <summary>
	/// Twam this cell belongs to.
	/// </summary>
	public enmTeam cellTeam {
		get { return _team; }
		set { _team = value; UpdateCellInfo(); }
	}
	/// <summary>
	/// The radius of the cell
	/// </summary>
	public float cellRadius {
		get { return _radius; }
		set { _radius = value; }
	}
	/// <summary>
	/// Cell's position in the world
	/// </summary>
	public Vector3 cellPosition {
		get { return _position; }
		set { _position = value; }
	}

}
