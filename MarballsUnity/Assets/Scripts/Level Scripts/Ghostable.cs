/// <summary>
/// Ghostable.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  1, 2015
/// 
/// Class for objects that should be passable with the ghost powerup.
///
/// TO DO: - Tweak behavior until desired.
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

	// OnTriggerEnter - Used to cache the marble thats in the area.
	void OnTriggerEnter(Collider other) {
		marble = other.GetComponent<Marble>();
	}

	// OnTriggerStay - Makes the wall passable or impassable based on marble conditions.
	void OnTriggerStay(Collider other) {
		ghostWall.enabled = !(marble && marble.buff == Marble.PowerUp.Ghost);
	}

	// OnTriggerExit - Resets wall conditions when something, hopefully a marble, exits the trigger.
	void OnTriggerExit(Collider other) {
		if (other.GetComponent<Marble>()) marble = null;
		ghostWall.enabled = true;
	}
}
