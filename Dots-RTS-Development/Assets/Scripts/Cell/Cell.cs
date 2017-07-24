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
	public Element.enmDebuffs providedDebuff;

	public List<Element.enmDebuffs> appliedDebuffs = new List<Element.enmDebuffs>();

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

	public static Color32 allyColour = new Color32(0, 255, 0, 255);                                         //Default ally colour
	public static Color32 neutralColour = new Color32(255, 255, 255, 255);                                  //Default neutral colour

	public static Color32 enemy1Colour = new Color32(255, 0, 0, 255);                                        //Default enemy1 colour
	public static Color32 enemy2Colour = new Color32(80, 0, 255, 255);                                       //Default enemy2 colour
	public static Color32 enemy3Colour = new Color32(220, 255, 0, 255);                                      //Default enemy3 colour
	public static Color32 enemy4Colour = new Color32(120, 60, 0, 255);                                       //Default enemy4 colour
	public static Color32 enemy5Colour = new Color32(150, 140, 0, 255);                                      //Default enemy5 colour
	public static Color32 enemy6Colour = new Color32(255, 0, 255, 255);                                      //Default enemy6 colour
	public static Color32 enemy7Colour = new Color32(0, 0, 0, 255);                                          //Default enemy7 colour
	public static Color32 enemy8Colour = new Color32(255, 150, 200, 255);                                    //Default enemy8 colour


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
	//Function to update visuals of the cell
	public virtual void UpdateCellInfo(bool calledFromBase = true) {

		float mappedValue;
		if (elementCount < 10) {
			mappedValue = 1;
		}
		else if (elementCount >= 10 && elementCount <= maxElements) {
			mappedValue = Map.MapFloat(elementCount, 10, maxElements, 1f, 2f);
		}
		else {
			mappedValue = Map.MapFloat(elementCount, maxElements, 999f, 2f, 4f);

		}

		transform.localScale = new Vector3(mappedValue, mappedValue, 1);
		cellRadius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

		elementNrDisplay.text = elementCount.ToString();
		textRenderer.sortingLayerName = "Cells";
		textRenderer.sortingOrder = 1;

		//Change Colour depending on the team
		switch (cellTeam) {
			case enmTeam.ALLIED: {
				cellSprite.color = allyColour;
				return;
			}
			case enmTeam.NEUTRAL: {
				cellSprite.color = neutralColour;
				return;
			}
			case enmTeam.ENEMY1: {
				cellSprite.color = enemy1Colour;
				return;
			}
			case enmTeam.ENEMY2: {
				cellSprite.color = enemy2Colour;
				return;
			}
			case enmTeam.ENEMY3: {
				cellSprite.color = enemy3Colour;
				return;
			}
			case enmTeam.ENEMY4: {
				cellSprite.color = enemy4Colour;
				return;
			}
			case enmTeam.ENEMY5: {
				cellSprite.color = enemy5Colour;
				return;
			}
			case enmTeam.ENEMY6: {
				cellSprite.color = enemy6Colour;
				return;
			}
			case enmTeam.ENEMY7: {
				cellSprite.color = enemy7Colour;
				return;
			}
			case enmTeam.ENEMY8: {
				cellSprite.color = enemy8Colour;
				return;
			}

		}
	}

	//Keeps generateing new elements for the cell
	public IEnumerator GenerateElements() {
		isRegenerating = true;
		while (isRegenerating) {
			yield return new WaitForSeconds(regenPeriod);
			if (elementCount < maxElements) {
				elementCount++;
				elementNrDisplay.text = elementCount.ToString();
				UpdateCellInfo();
			}
		}
	}
	//Wrapper for Decaying
	public void Decay(float decayRate) {
		if (!isDecaying) {
			isDecaying = true;
			StartCoroutine(DecayElements(decayRate));
		}
	}

	//Elements start decaying when they go over the cap(max Elements).
	private IEnumerator DecayElements(float decayRate) {
		float d = decayRate;
		StopCoroutine(GenerateElements());
		while (isDecaying) {
			yield return new WaitForSeconds(d);
			elementCount--;
			if (maxElements - elementCount > maxElements * 0.5f) {
				d = d * 0.5f;
			}
			if (elementCount <= maxElements) {
				isDecaying = false;
				isRegenerating = false;
			}
		}
	}

	public IEnumerator DoT(float speed, int strength) {
		Debug.LogWarning("Called using default values, possibly not implemented yet!");
		for (int i = 0; i < strength; i++) {
			yield return new WaitForSeconds(speed);
			if (elementCount >= 1) {
				elementCount--;
			}
		}
		appliedDebuffs.Remove(Element.enmDebuffs.DOT);
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
		set { _maxElementCount = value; UpdateCellInfo(true); }
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
