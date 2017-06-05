using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	private int _elementCount;                                                                  //Current amount of elements inside the cell
	private float _regenFreq;                                                                   //How fast will the cell regenerate
	private int _maxElementCount;                                                               //How much can the cell hold
	private Vector2 _position;                                                                  //Cells position
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


	/// <summary>
	/// The elements that are available in selected cell.
	/// </summary>
	public int elementCount {
		get { return _elementCount; }
		set { _elementCount = value; }
	}

	/// <summary>
	/// How fast will this cell generate new elements.
	/// </summary>
	public float regenFrequency {
		get { return _regenFreq; }
		set { _regenFreq = value; }
	}
	/// <summary>
	/// The maximum amount of elements this cell can hold.
	/// </summary>
	public int maxElements {
		get { return _maxElementCount; }
		set { _maxElementCount = value; }
	}
	/// <summary>
	/// Twam this cell belongs to.
	/// </summary>
	public enmTeam cellTeam {
		get { return _team; }
		set { _team = value; }
	}
	/// <summary>
	/// The radius of the cell
	/// </summary>
	public float cellRadius {
		get { return _radius; }
		set { _radius = value; }
	}
}
