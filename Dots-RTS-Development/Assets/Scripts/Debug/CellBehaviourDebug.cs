//using UnityEngine;
//using System.Collections;

//public class CellBehaviourDebug : GameCell {

//	private void Start() {
//		StartCoroutine(GenerateElements());
//	}

//	public void UpdateCellInfo(bool calledFromBase = false) {
//		ScaleCell();

//		elementCountDisplay.text = elementCount.ToString();
//		elementCountDisplayRenderer.sortingLayerName = "Cells";
//		elementCountDisplayRenderer.sortingOrder = 1;

//		//Change Colour depending on the team
//		switch (cellTeam) {
//			case Team.ALLIED: {
//				cellSprite.color = allyColour;
//				return;
//			}
//			case Team.NEUTRAL: {
//				cellSprite.color = neutralColour;
//				return;
//			}
//			case Team.ENEMY1: {
//				cellSprite.color = enemy1Colour;
//				return;
//			}
//			case Team.ENEMY2: {
//				cellSprite.color = enemy2Colour;
//				return;
//			}
//			case Team.ENEMY3: {
//				cellSprite.color = enemy3Colour;
//				return;
//			}
//			case Team.ENEMY4: {
//				cellSprite.color = enemy4Colour;
//				return;
//			}
//			case Team.ENEMY5: {
//				cellSprite.color = enemy5Colour;
//				return;
//			}
//			case Team.ENEMY6: {
//				cellSprite.color = enemy6Colour;
//				return;
//			}
//			case Team.ENEMY7: {
//				cellSprite.color = enemy7Colour;
//				return;
//			}
//			case Team.ENEMY8: {
//				cellSprite.color = enemy8Colour;
//				return;
//			}


//		}
//	}

//	new private IEnumerator ScaleCell() {
//		float mappedValue;
//		if (elementCount < 10) {
//			mappedValue = 1;
//		}
//		else if (elementCount >= 10 && elementCount <= maxElements) {
//			mappedValue = Map.MapFloat(elementCount, 10, maxElements, 1f, 2f);
//		}
//		else {
//			if (elementCount < 1000) {
//				mappedValue = Map.MapFloat(elementCount, maxElements, 999f, 2f, 4f);
//			}
//			else {
//				mappedValue = 4;
//			}
//		}


//		for (float f = 0; f < mappedValue; f += regenPeriod * Time.fixedDeltaTime) {
//			transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(mappedValue, mappedValue), f);
//			yield return null;
//		}
//		cellRadius = col.radius * transform.localScale.x;

//	}
//}