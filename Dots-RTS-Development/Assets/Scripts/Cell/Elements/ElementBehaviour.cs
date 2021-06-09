using System;
using UnityEngine;

public class ElementBehaviour : Element {

	private Vector2 velocity;
	private float wobbleFrequency = 2;
	private float wobbleAmplitude = 0.2f;
	private float slowDownFrequency = 3f;
	private bool destroyOnNextAttack;

	private void FixedUpdate() {
		if (DistanceToTarget(target) < target.Cell.CellRadius) {
			//Execute this code after collision with target.

			if (team > 0) {
				ExecuteAttack();
			}
			if (reflected) {
				if (destroyOnNextAttack) {
					Destroy(gameObject);
				}
				destroyOnNextAttack = true;
			}
		}

		velocity = DirectionToTarget(target) * speed;

		velocity = Mathf.Abs(Mathf.Sin((Time.time + randomTimeOffset) * slowDownFrequency)) * velocity;

		gameObject.transform.position += (Vector3)ApplySidewaysWobble(velocity);
	}

	private Vector2 DirectionToTarget(GameCell targetCell) {
		Vector2 seekF = targetCell.gameObject.transform.position - transform.position;
		seekF.Normalize();
		return seekF;
	}

	private float DistanceToTarget(GameCell targetCell) {
		return Vector2.Distance(targetCell.transform.position, transform.position);
	}

	private Vector2 ApplySidewaysWobble(Vector2 input) {
		Vector2 normal = new Vector2(input.y, -input.x);
		normal *= Mathf.Sin((Time.time + randomTimeOffset) * wobbleFrequency) * wobbleAmplitude;

		return input + normal;
	}
}
