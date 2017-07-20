using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelSorting : MonoBehaviour {

	bool isAscending = false;

	// Use this for initialization
	void Start() {
		//string[] strings = new string[10] { "b", "g", "d", "w", "k", "p", "c", "z", "u", "a" }; Success
		string[] strings = new string[10] { "b", "G", "d", "w", "k", "P", "C", "z", "u", "a" };


		foreach (string s in strings) {
			print(s);
		}

		for (int j = strings.Length - 1; j > 0; j--) {
			for (int i = 0; i < j; i++) {
				string precedingName = strings[i];
				string folowingName = strings[i + 1];
				int value = string.Compare(precedingName, folowingName, true);
				if (value > 0) {
					string temp = strings[i];

					strings[i] = strings[i + 1];
					strings[i + 1] = temp;
				}
			}
		}
		print("---------------------------");
		foreach (string s in strings) {
			print(s);
		}

	}
	public void SortName() {
		isAscending = !isAscending;

		SaveFileInfo[] saves = LevelSelectScript.displayedSaves.ToArray();

		for (int j = saves.Length - 1; j > 0; j--) {
			for (int i = 0; i < j; i++) {
				string precedingName = saves[i].levelName.text;
				string folowingName = saves[i + 1].levelName.text;
				int value = string.Compare(precedingName, folowingName, true);
				if (isAscending) {
					if (value > 0) {
						Swap(saves, i, i + 1);
					}
				}
				else {
					if (value < 0) {
						Swap(saves, i, i + 1);
					}
				}
			}
		}

		foreach (SaveFileInfo save in saves) {
			LevelSelectScript.displayedSaves.Remove(save);
			SaveFileInfo g = Instantiate(save, GameObject.Find("Content").transform);
			LevelSelectScript.displayedSaves.Add(g);
			Destroy(save.gameObject);
		}
	}

	public void SortAuthor() {
		isAscending = !isAscending;

		SaveFileInfo[] saves = LevelSelectScript.displayedSaves.ToArray();

		for (int j = saves.Length - 1; j > 0; j--) {
			for (int i = 0; i < j; i++) {
				string precedingName = saves[i].author.text;
				string folowingName = saves[i + 1].author.text;
				int value = string.Compare(precedingName, folowingName, true);
				if (isAscending) {
					if (value > 0) {
						Swap(saves, i, i + 1);
					}
				}
				else {
					if (value < 0) {
						Swap(saves, i, i + 1);
					}
				}
			}
		}

		foreach (SaveFileInfo save in saves) {
			LevelSelectScript.displayedSaves.Remove(save);
			SaveFileInfo g = Instantiate(save, GameObject.Find("Content").transform);
			LevelSelectScript.displayedSaves.Add(g);
			Destroy(save.gameObject);
		}
	}

	int swaps = 0;
	public void SortDate() {
		isAscending = !isAscending;

		SaveFileInfo[] saves = LevelSelectScript.displayedSaves.ToArray();

		for (int j = saves.Length - 1; j > 0; j--) {
			for (int i = 0; i < j; i++) {
				int value = DateTime.Compare(Convert.ToDateTime(saves[i].timeRaw), Convert.ToDateTime(saves[i + 1].timeRaw));

				if (isAscending) {
					if (value > 0) {
						Swap(saves, i, i + 1);
					}
				}
				else {
					if (value < 0) {
						Swap(saves, i, i + 1);
					}
				}
			}
		}
		print(swaps);
		swaps = 0;

		foreach (SaveFileInfo save in saves) {
			LevelSelectScript.displayedSaves.Remove(save);
			SaveFileInfo g = Instantiate(save, GameObject.Find("Content").transform);
			LevelSelectScript.displayedSaves.Add(g);
			Destroy(save.gameObject);
		}
	}

	private void Swap(SaveFileInfo[] data, int i, int j) {
		SaveFileInfo temp = data[i];
		swaps++;
		data[i] = data[j];
		data[j] = temp;
	}

}
