using System;
using System.Collections.Generic;
using UnityEngine;

class ElementBehaviour : Element {

	private Vector3 acc;
	private Vector3 vel;

	//Calculates steering behaviour for the element 
	private void FixedUpdate() {
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
		//print(d + " " + target.cellRadius);
		if (d < target.cellRadius) {
			//Execute this code after collision with target.
			if (team > 0) {
				ExecuteAttack();
			}
			else {
				throw new InvalidOperationException();
			}
			Destroy(gameObject);
		}
		Vector3 seekF = target.gameObject.transform.position - gameObject.transform.position;
		seekF.Normalize();
		seekF = seekF * eSpeed;
		return seekF - vel;
	}
}
