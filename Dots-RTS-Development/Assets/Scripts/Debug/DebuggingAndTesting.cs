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
}

