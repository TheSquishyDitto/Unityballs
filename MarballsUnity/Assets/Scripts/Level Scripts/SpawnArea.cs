/// <summary>
/// SpawnArea.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira
/// Date Created:  Feb. 16, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Class that dictates how spawning pads should function.
/// 
/// NOTES: - Should lock ball's position in the starting phase, and release the ball afterwards.
/// 	   - Might be nice if it gave an initial speed boost to the ball as well.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SpawnArea : MonoBehaviour {

	GameMaster gm;			// Reference to the game master.
	public GameObject sfx;	// Reference to attached aesthetic effects.

	//bool active = true;		// Whether spawn point sets marble's respawn location to it or not.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM ();
		//gm.respawn = this;
	}

	void Start() {
		gm.marble.spawnPoint = transform;
	}

	// OnEnable - Called whenever the spawnpad is activated.
	void OnEnable() {
		GameMaster.start += SparkleOn;
		GameMaster.play += SparkleOff;
	}

	// OnDisable - Called whenever the spawnpad is deactivated.
	void OnDisable() {
		GameMaster.start -= SparkleOn;
		GameMaster.play -= SparkleOff;
	}

	// OnTriggerStay - As long as another object is within the collision zone.
	void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody)
			// It turns out you can actually lock a rigidbody's position using binary flag presets. Behaves more nicely than turning gravity off.
			other.attachedRigidbody.constraints = (gm.state != GameMaster.GameState.Playing) ? RigidbodyConstraints.FreezePosition : RigidbodyConstraints.None;
		
	}

	// OnTriggerExit - Called when a collider leaves the trigger collision zone.
	void OnTriggerExit(Collider other) {
		if (other.attachedRigidbody) {
			other.attachedRigidbody.constraints = RigidbodyConstraints.None; // Failsafe in case the stay function fails.
		}
	}

	// SparkleOn - Activates the spawn pad's pretty features.
	void SparkleOn() {
		//active = true;
		sfx.SetActive(true);
	}

	// SparkleOff - Deactivates the spawn pad's pretty features.
	void SparkleOff() {
		//active = false;
		sfx.SetActive(false);
	}
}
