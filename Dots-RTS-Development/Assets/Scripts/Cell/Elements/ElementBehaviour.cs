using System;
using UnityEngine;

public class ElementBehaviour : Element {

	private Vector2 velocity;
	private const float WOBBLE_FREQUENCY = 2;
	private const float WOBBLE_AMPLITUDE = 0.2f;
	private const float SLOW_DOWN_FREQUENCY = 3f;
	private bool destroyOnNextAttack;

	private void FixedUpdate() {
		if (DistanceToTarget(target) < target.Cell.cellRadius) {
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

		velocity = Mathf.Abs(Mathf.Sin((Time.time + randomTimeOffset) * SLOW_DOWN_FREQUENCY)) * velocity;

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
		normal *= Mathf.Sin((Time.time + randomTimeOffset) * WOBBLE_FREQUENCY) * WOBBLE_AMPLITUDE;

		return input + normal;
	}
}
