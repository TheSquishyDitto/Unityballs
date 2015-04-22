/// <summary>
/// KillZone.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Feb. 16, 2015
/// Last Revision: Apr. 22, 2015
/// 
/// Class that handles behavior of killzone boundaries.
/// 
/// NOTES: - Should be attached to objects with plane-like trigger colliders.
/// 	   - Currently respawns ball back at the spawn pad.
/// 
/// TO DO: - Allow the option for killzones to "destroy" the marble.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {
	
	GameMaster gm;	// Reference to Game Master.

	//public bool destroyMarble;	// Whether this killzone should obliterate the marble on contact or not.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {
		//if (GetComponent<Renderer>())
		//	GetComponent<Renderer>().enabled = false;	// Kill zone should only be rendered in scene view, not in gameplay.
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// OnTriggerEnter - Called when an object collides with the trigger.
	void OnTriggerEnter(Collider other) {
		// Check if what fell into the killzone was a marble, since you wouldn't want a falling box to reset the player.
		if (other.CompareTag("Marble") && gm.state == GameMaster.GameState.Playing) {
			AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/WilhelmScream"), gm.cam.position);
			gm.hud.StartCoroutine("OnDeath");	// Call gm.OnDeath instead.
		}
	}

	// OnDrawGizmosSelected - Used to draw things when selected in scene view exclusively: does not affect gameplay.
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawCube(transform.position, transform.localScale);	// Shows kill zone when the kill zone is selected.
	}
}
