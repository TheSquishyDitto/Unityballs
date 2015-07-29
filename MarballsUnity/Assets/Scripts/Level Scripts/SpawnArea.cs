/// <summary>
/// SpawnArea.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira
/// Date Created:  Feb. 16, 2015
/// Last Revision: July 22, 2015
/// 
/// Class that dictates how spawning pads should function.
/// 
/// NOTES: - Locks marble's position on contact, and releases after countdown. (PRIORITY 200)
/// 	   - Might be nice if it gave an initial speed boost to the ball as well.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SpawnArea : MonoBehaviour {

	GameMaster gm;				// Reference to the game master.
	Marble marble;				// Reference to marble that touched this spawn pad.
	public GameObject sfx;		// Reference to attached aesthetic effects.

	bool active = true;			// Whether spawn point is going to perform its functions.
	TimeEvent countdownTimer;	// Timer for countdown.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM ();
		GameMaster.sequence.AddSequence(new SequenceSlot(200, StartPrep()));
	}

	// Start - Use this for initialization.
	void Start() {
		GameMaster.sequence.StartSequence(gm, true);
	}

	/*// OnEnable - Called whenever the spawnpad is activated.
	void OnEnable() {

	}

	// OnDisable - Called whenever the spawnpad is deactivated.
	void OnDisable() {

	}*/

	// StartPrep - Coroutine that prepares the marble and its countdown.
	public IEnumerator StartPrep() {
		// Start the countdown and release the marble at the end of it.
		countdownTimer = new TimeEvent(gm.settings.countdownLength, () => { ReleaseMarble(); });
		TimeManager.CreateTimer().StartStopwatch(countdownTimer);
		Messenger<TimeEvent>.Broadcast("Countdown", countdownTimer);

		yield return countdownTimer.routine;

		SparkleOff();
	}

	// ReleaseMarble - Releases the marble.	
	void ReleaseMarble() {
		if (marble != null)
			marble.marbody.constraints = RigidbodyConstraints.None;

		Messenger.Broadcast("BeginTimer");	// Tell whatever is managing the level timer to get going.
	}

	// OnTriggerEnter - Triggered when an object enters the trigger.
	void OnTriggerEnter(Collider other) {

		// Locks marble in place on entry and saves reference to marble.
		if (other.GetComponent<Marble>() != false && active) {
			marble = other.GetComponent<Marble>();
			marble.spawnPoint = transform;

			if (marble.marbody != null) {
				marble.marbody.constraints = RigidbodyConstraints.FreezePosition;
			}
		}
	}

	// SparkleOn - Activates the spawn pad's pretty features.
	void SparkleOn() {
		active = true;
		sfx.SetActive(true);
	}

	// SparkleOff - Deactivates the spawn pad's pretty features.
	void SparkleOff() {
		active = false;
		sfx.SetActive(false);
	}
}
