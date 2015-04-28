/// <summary>
/// KillZone.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Feb. 16, 2015
/// Last Revision: Apr. 27, 2015
/// 
/// Class that handles behavior of killzone boundaries.
/// 
/// NOTES: - Can be attached to anything with any collider.
/// 	   - Currently respawns ball back at the spawn pad.
/// 
/// TO DO: - Allow the option for killzones to "destroy" the marble.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {
	
	GameMaster gm;	// Reference to Game Master.

	public bool showGizmo = false;	// Whether gizmo should be rendered or not.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// OnTriggerEnter - Called when an object collides with the trigger.
	void OnTriggerEnter(Collider other) {
		Kill(other);
	}

	// OnCollisionEnter - Called when an object collides with the collider.
	void OnCollisionEnter(Collision collision) {
		Kill (collision.collider);
	}

	// OnDrawGizmosSelected - Used to draw things when selected in scene view exclusively: does not affect gameplay.
	void OnDrawGizmosSelected() {
		if (showGizmo) {
			Gizmos.color = Color.red;
			Gizmos.DrawCube(transform.position, transform.localScale);	// Shows kill zone when the kill zone is selected.
		}
	}

	// Kill - Lives up to its namesake.
	void Kill(Collider other) {
		// Check if what fell into the killzone was a marble, since you wouldn't want a falling box to reset the player.
		if (other.CompareTag("Marble") && gm.state == GameMaster.GameState.Playing) {
			AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/WilhelmScream"), gm.cam.position);
			gm.marble.OnDie();
		}
	}
}
