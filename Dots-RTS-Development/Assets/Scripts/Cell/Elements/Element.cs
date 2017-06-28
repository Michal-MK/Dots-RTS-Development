using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour {
	private Vector3 acc;
	private Vector3 vel;
	private float maxSpeed = 10;

	public Cell.enmTeam team;
	public CellBehaviour attacker;
	public CellBehaviour target;

	private void Start() {
		attacker.UpdateCellInfo();
	}

	//Calculates steering behaviour for the element 
	private void Update() {
		Vector3 seek = Seek(target);
		ApplyForce(seek);
		gameObject.transform.position += vel;
		vel += acc;
		acc = acc * 0;
	}

	//Calculates steering behaviour for the element 
	private void ApplyForce(Vector3 force) {
		acc += force;
	}

	//Calculates the error between desired and current velocity
	private Vector3 Seek(CellBehaviour target) {
		float d = Vector3.Distance(target.transform.position, gameObject.transform.position);
		//print(d + " " + target._radius);
		if (d < target.cellRadius) {
			//Execute this code after collision with target.
			if (team != 0 || (int)team != -1) {
				target.DamageCell(team);
			} else {
				throw new System.InvalidOperationException();
			}
			Destroy(gameObject);
		}
		Vector3 seekF = target.gameObject.transform.position - gameObject.transform.position;
		seekF.Normalize();
		seekF = seekF * maxSpeed;
		return seekF - vel;
	}
}
