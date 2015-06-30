/// <summary>
/// BuffClearSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class for buff/powerup clearing entities.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BuffClearSource : MonoBehaviour {

	public bool collectable;	// Whether or not this buff source should disappear when collected.

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Clears buffs from a marble.
			other.GetComponent<Marble>().ClearBuffs();
			if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
		}	
	}
}
