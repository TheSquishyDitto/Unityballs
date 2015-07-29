/// <summary>
/// TimerTester.cs
/// Authors: Kyle Dawson
/// Date Created:  July 19, 2015
/// Last Revision: July 21, 2015
/// 
/// Class used to test timing and sequencing classes.
/// 
/// NOTES: - Not an actual class relevant to gameplay.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class TimerTester : MonoBehaviour {

	public string nickname;	// Name of this tester.
	public float time;		// Length of time before performing action.

	public TimeEvent packet;	// Saved so that the stopwatch information can be accessed.

	void Awake() {
		GameMaster.sequence.AddSequence(new SequenceSlot(0, Test()));
		GameMaster.sequence.AddSequence(new SequenceSlot(1, Test2()));
	}

	IEnumerator Test() {
		Debug.Log("Performed action 1!");
		yield return new WaitForSeconds(3);
	}

	IEnumerator Test2() {
		Debug.Log("Performed action 2!");
		yield return new WaitForSeconds(3);
	}

	// Start - Use this for initialization.
	void Start () {
		GameMaster.sequence.StartSequence(this);
		packet = new TimeEvent(time, () => { Debug.Log("Tester " + nickname + " finished!"); });
		TimeManager.CreateTimer().StartStopwatch(packet);
	}

	// Update - Called every frame.
	void Update() {
		if (Input.GetKeyDown(KeyCode.T) && packet != null && GameMaster.LoadSettings().debug) {
			Debug.Log(nickname + " time remaining: " + packet.duration);
		}
	}
}
