/// <summary>
/// KillZone.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Feb. 16, 2015
/// Last Revision: Apr. 30, 2015
/// 
/// Class that handles behavior of deadly objects and zones.
/// 
/// NOTES: - Can be attached to anything with any collider.
/// 
/// TO DO: - Maybe allow more customizable deaths? Could just inherit from this class then, possibly.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class KillZone : MonoBehaviour {

	public bool showGizmo = false;	// Whether gizmo should be rendered or not.

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
		if (other.GetComponent<IKillable>() != null) {
			other.GetComponent<IKillable>().Die();
		}
	}
}

// Interface for anything that should instantly die or otherwise take action when touched by a killzone.
public interface IKillable {
	void Die();
}
