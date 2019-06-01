using System;
using UnityEngine;

class ElementBehaviour : Element {

	private Vector2 velocity;
    private float wobbleFreqency = 2;
    private float wobbleAmplitude = 0.2f;
    private float slowDownFrequency = 3f;
	private bool destroyOnNextAttack = false;

    private void FixedUpdate() {
        if (DistanceToTarget(Target) < Target.Cell.CellRadius) {
            //Execute this code after collision with target.

            if (Team > 0) {
                ExecuteAttack();
            }
            else {
                throw new InvalidOperationException();
            }
			if (reflected) {
				if (destroyOnNextAttack) {
					Destroy(gameObject);
				}
				destroyOnNextAttack = true;
			}
        }

        velocity = DirectionToTarget(Target) * Speed;

        velocity = Mathf.Abs(Mathf.Sin(AdjustedTime() * slowDownFrequency)) * velocity;

        gameObject.transform.position += (Vector3)ApplySidewaysWobble(velocity);
	}

	private Vector2 DirectionToTarget(GameCell target) {
		Vector2 seekF = target.gameObject.transform.position - transform.position;
		seekF.Normalize();
		return seekF;
	}
    
    private float DistanceToTarget(GameCell target) {
        return Vector2.Distance(target.transform.position, transform.position);
    }

    private Vector2 ApplySidewaysWobble(Vector2 IN) {
        Vector2 Normal = new Vector2(IN.y, -IN.x);
        Normal = Normal * Mathf.Sin(AdjustedTime() * wobbleFreqency) * wobbleAmplitude;

        return IN + Normal;
    }

    float AdjustedTime() {
        return Time.time + RandomTimeOffset;
    }
}
