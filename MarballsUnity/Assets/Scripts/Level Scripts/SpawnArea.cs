/// <summary>
/// SpawnArea.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 16, 2015
/// Last Revision: Feb. 16, 2015
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

	public GameMaster gm;	// Reference to the game master.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM ();
		gm.respawn = transform;
	}

	// Start - Use this for initialization
	void Start () {
		if (gm.marble)
			gm.marble.GetComponent<Marble>().Respawn(); // Makes sure marble is in the right spot once the spawn pad is out.
	}
	
	// Update is called once per frame
	void Update () {
			
	}
	
	// OnTriggerStay - As long as another object is within the collision zone.
	void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody)
			// It turns out you can actually lock a rigidbody's position using binary flag presets. Behaves more nicely than turning gravity off.
			other.attachedRigidbody.constraints = (gm.state == GameMaster.GameState.Start) ? RigidbodyConstraints.FreezePosition : RigidbodyConstraints.None;
			//other.attachedRigidbody.useGravity = (gm.state == GameMaster.GameState.Start) ? false : true;
		
	}

	// OnTriggerExit - Called when a collider leaves the trigger collision zone.
	void OnTriggerExit(Collider other) {
		if (other.attachedRigidbody)
			other.attachedRigidbody.constraints = RigidbodyConstraints.None; // Failsafe in case the stay function fails.
		
	}
}
