
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Test : MonoBehaviour {

	private async void Awake() {
		print("S");
		await Task.WhenAny(Tests(), MyMethodAsync());
		print("V");
	}

	public async Task Tests() {
		int i = await GetGoing();
		print(i);
	}

	private async Task<int> GetGoing() {

		Transform t = GameObject.Find("Image").transform;
		int j = 0;
		for (int i = 0; i < 50000; i++) {
			t.localScale = Vector3.one * Random.Range(1, 3);
			//print(transform.localScale);
			j = i;
		}
		return j;
	}

	public async Task MyMethodAsync() {
		Task<int> longRunningTask = LongRunningOperationAsync();
		// independent work which doesn't need the result of LongRunningOperationAsync can be done here

		//and now we call await on the task 
		int result = await longRunningTask;
		//use the result 
		Debug.Log("Waited 2 secs " + result);
	}

	public async Task<int> LongRunningOperationAsync() // assume we return an int from this long running operation 
	{
		await Task.Delay(2000); //1 seconds delay
		return 123;
	}
}
