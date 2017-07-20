using UnityEngine;
using System.Collections;

class DebuggingAndTesting {
	int call = 0;
	string[] info = new string[5] { "A", "B", "C", "D", "E" };

	private IEnumerator GetA(string path) {
		yield return new WaitForSeconds(Random.Range(1f, 5f));
		yield return info;
	}

	public IEnumerator Test() {
		int callLocal = call;
		call++;
		Debug.Log("Call " + callLocal + " expecting " + info[callLocal]);

		IEnumerator e = GetA("S");
		yield return e;
		yield return new WaitForSeconds(Random.Range(0, 5f));
		string[] s = e.Current as string[];

		Debug.Log("For " + callLocal + " Got: " + s[callLocal]);
	}

	void Comparison() {
		//string[] strings = new string[10] { "b", "g", "d", "w", "k", "p", "c", "z", "u", "a" }; Success
		string[] strings = new string[10] { "b", "G", "d", "w", "k", "P", "C", "z", "u", "a" }; //Success


		foreach (string s in strings) {
			Debug.Log(s);
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
		Debug.Log("---------------------------");
		foreach (string s in strings) {
			Debug.Log(s);
		}

	}
}

