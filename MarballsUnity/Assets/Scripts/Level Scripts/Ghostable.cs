/// <summary>
/// Ghostable.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  3, 2015
/// 
/// Class for objects that should be passable with the ghost powerup.
///
/// TO DO: - Tweak behavior until desired.
/// 	   - Possibly fix screwy camera behavior when ghosting through walls.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Ghostable : MonoBehaviour {

	Marble marble;		// Reference to any marble that happens to enter the collider.
	Collider ghostWall;	// Reference to the collider that will be toggled.

	// Start - Use this for initialization
	void Start () {
		ghostWall = GetComponent<Collider>();
	}
	
	// Update - Called once per frame.
	void Update () {
	
	}

	// OnTriggerEnter - 
	void OnTriggerEnter(Collider other) {

	}

	// OnTriggerStay - Makes the wall passable or impassable based on marble conditions.
	void OnTriggerStay(Collider other) {

		marble = other.GetComponent<Marble>();	// Attempts to find a marble script on the other collider.

		// If it's a marble, and it's ghosting, ignore collisions. Otherwise, stay solid.
		if (marble) {
			Physics.IgnoreCollision(ghostWall, other, (marble.buff == Marble.PowerUp.Ghost));
		}
	}

	// OnTriggerExit - 
	void OnTriggerExit(Collider other) {

	}
}
