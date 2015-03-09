/// <summary>
/// BuffClearSource.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 23, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// Class for buff/powerup clearing entities.
/// 
/// NOTES: - Should probably either use general purpose version (BuffSource.cs) or make a base class for these.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BuffClearSource : MonoBehaviour {

	public bool collectable;	// Whether or not this buff source should disappear when collected.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Clears buffs from a marble.
			other.GetComponent<Marble>().ClearBuffs();
			if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
		}	
	}
}
