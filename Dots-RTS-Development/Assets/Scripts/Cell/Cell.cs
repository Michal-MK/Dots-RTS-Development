using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LineRenderer), typeof(CircleCollider2D))]

public class Cell : MonoBehaviour {

	public int _elementCount;                                                                  //Current amount of elements inside the cell
	public float _regenP;                                                                      //How fast will the cell regenerate
	public int _maxElementCount;                                                               //How much can the cell hold
	public Vector2 _position;                                                                  //Cells position
	public bool isRegenerating = false;
	public bool isDecaying = false;
	public enmTeam _team;                                                                       //Cell's team

	public enum enmTeam {
		NONE = -1,
		NEUTRAL,
		ALLIED,
		ENEMY1,
		ENEMY2,
		ENEMY3,
		ENEMY4,
		ENEMY5,
		ENEMY6,
		ENEMY7,
		ENEMY8,
	}

	private float _radius;

	public static Color32 ally = new Color32(0, 255, 0, 255);                                         //Default ally colour
	public static Color32 neutral = new Color32(255, 255, 255, 255);                                  //Default neutral colour

	public static Color32 enemy1 = new Color32(255, 0, 0, 255);                                        //Default enemy1 colour
	public static Color32 enemy2 = new Color32(80, 0, 255, 255);                                       //Default enemy2 colour
	public static Color32 enemy3 = new Color32(220, 255, 0, 255);                                      //Default enemy3 colour
	public static Color32 enemy4 = new Color32(120, 60, 0, 255);                                       //Default enemy4 colour
	public static Color32 enemy5 = new Color32(150, 140, 0, 255);                                      //Default enemy5 colour
	public static Color32 enemy6 = new Color32(255, 0, 255, 255);                                      //Default enemy6 colour
	public static Color32 enemy7 = new Color32(0, 0, 0, 255);                                          //Default enemy7 colour
	public static Color32 enemy8 = new Color32(255, 150, 200, 255);                                    //Default enemy8 colour


	//Prefab references
	public SpriteRenderer cellSprite;
	public TextMesh elementNrDisplay;
	public MeshRenderer textRenderer;
	public LineRenderer circle;
	public LineRenderer lineToMouse;


	//Not implemented yet
	public Upgrade_Manager um;


	private void Start() {
		UpdateCellInfo();
	}

	public virtual void UpdateCellInfo(bool calledFromBase = true) {
		//print(c.gameObject.name);
		if (elementCount >= 10 && elementCount <= maxElements) {
			float mappedValue = Map.MapFloat(elementCount, 0, maxElements, 1, 2);

			transform.localScale = new Vector3(mappedValue, mappedValue, 1);
			cellRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
		}

		elementNrDisplay.text = elementCount.ToString();
		textRenderer.sortingLayerName = "Cells";
		textRenderer.sortingOrder = 1;
		//circle.sortingLayerName = "Cells";
		//circle.sortingOrder = 0;

		//Change Colour depending on the team
		switch (cellTeam) {
			case enmTeam.ALLIED: {
				cellSprite.color = ally;
				return;
			}
			case enmTeam.NEUTRAL: {
				cellSprite.color = neutral;
				return;
			}
			case enmTeam.ENEMY1: {
				cellSprite.color = enemy1;
				return;
			}
			case enmTeam.ENEMY2: {
				cellSprite.color = enemy2;
				return;
			}
			case enmTeam.ENEMY3: {
				cellSprite.color = enemy3;
				return;
			}
			case enmTeam.ENEMY4: {
				cellSprite.color = enemy4;
				return;
			}
			case enmTeam.ENEMY5: {
				cellSprite.color = enemy5;
				return;
			}
			case enmTeam.ENEMY6: {
				cellSprite.color = enemy6;
				return;
			}
			case enmTeam.ENEMY7: {
				cellSprite.color = enemy7;
				return;
			}
			case enmTeam.ENEMY8: {
				cellSprite.color = enemy8;
				return;
			}

		}
	}

	//Keeps generateing new elements for the cell
	public IEnumerator GenerateElements() {
		isRegenerating = true;
		while (isRegenerating) {
			yield return new WaitForSecondsRealtime(regenPeriod);
			if (elementCount < maxElements) {
				elementCount++;
				elementNrDisplay.text = elementCount.ToString();
				UpdateCellInfo();
			}
		}
	}
	public void Decay(float decayRate) {
		if (!isDecaying) {
			isDecaying = true;
			StartCoroutine(DecayElements(decayRate));
		}

	}

	//Elements start decaying when they go over the cap.
	private IEnumerator DecayElements(float decayRate) {
		float d = decayRate;
		StopCoroutine(GenerateElements());
		while (isDecaying) {
			yield return new WaitForSeconds(d);
			elementCount--;
			if(maxElements - elementCount > maxElements * 0.5f) {
				d = d * 0.5f;
			}
			if(elementCount <= maxElements) {
				isDecaying = false;
				isRegenerating = false;
			}
		}
	}

	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int elementCount {
		get { return _elementCount; }
		set { _elementCount = value; UpdateCellInfo(true); }
	}

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float regenPeriod {
		get { return _regenP; }
		set { _regenP = value; UpdateCellInfo(true); }
	}

	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int maxElements {
		get { return _maxElementCount; }
		set { _maxElementCount = value;  UpdateCellInfo(true); }
	}

	/// <summary>
	/// Team this cell belongs to.
	/// </summary>
	public enmTeam cellTeam {
		get { return _team; }
		set { _team = value; UpdateCellInfo(true); }
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
