using System;
using System.Collections.Generic;
using UnityEngine;

class ElementBehaviour : Element {

	private Vector2 velocity;
    private float wobbleFreqency = 2;
    private float wobbleAmplitude = 0.2f;
    private float SlowDownFrequency = 3f;

    //Calculates steering behaviour for the element 
    private void FixedUpdate() {
        if (DistanceToTarget(target) < target.cellRadius) {
            //Execute this code after collision with target.
            if (team > 0) {
                ExecuteAttack();
            }
            else {
                throw new InvalidOperationException();
            }
            Destroy(gameObject);
        }

        velocity = DirectionToTarget(target) * eSpeed;

        velocity = Mathf.Abs(Mathf.Sin(AdjustedTime() * SlowDownFrequency)) * velocity;

        gameObject.transform.position += (Vector3)ApplySidewaysWobble(velocity);
	}

	private Vector2 DirectionToTarget(CellBehaviour target) {
		Vector2 seekF = target.gameObject.transform.position - gameObject.transform.position;
		seekF.Normalize();
		return seekF;
	}
    
    private float DistanceToTarget(CellBehaviour target) {
        return Vector2.Distance(target.transform.position, gameObject.transform.position);
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
