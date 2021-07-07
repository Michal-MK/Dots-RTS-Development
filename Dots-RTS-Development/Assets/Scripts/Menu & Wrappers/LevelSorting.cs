using System;
using System.Linq;
using UnityEngine;

public class LevelSorting : MonoBehaviour {

	private bool isAscending = false;

	public void SortName() {
		isAscending = !isAscending;

		SaveFileInfo[] saves;

		if (isAscending)
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderBy(save => save.levelName.text).ToArray();
		else
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderByDescending(save => save.levelName.text).ToArray();

		Spawn(saves);
	}

	public void SortAuthor() {
		isAscending = !isAscending;

		SaveFileInfo[] saves;

		if (isAscending)
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderBy(save => save.author.text).ToArray();
		else
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderByDescending(save => save.author.text).ToArray();

		Spawn(saves);
	}

	public void SortDate() {
		isAscending = !isAscending;

		SaveFileInfo[] saves;

		if (isAscending)
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderBy(save => Convert.ToDateTime(save.timeRaw)).ToArray();
		else
			saves = LevelSelectScript.DISPLAYED_SAVES.OrderByDescending(save => Convert.ToDateTime(save.timeRaw)).ToArray();

		Spawn(saves);
	}

	private void Spawn(SaveFileInfo[] saves) {
		foreach (SaveFileInfo save in saves) {
			LevelSelectScript.DISPLAYED_SAVES.Remove(save);
			SaveFileInfo g = Instantiate(save, GameObject.Find("Content").transform);
			g.gameObject.name = save.name;
			LevelSelectScript.DISPLAYED_SAVES.Add(g);
			Destroy(save.gameObject);
		}
	}
}
