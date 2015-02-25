/// <summary>
/// SpeedBoostSource.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 23, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// Class for speed boost granting entities.
/// 
/// NOTES: - Should probably either use general purpose version (BuffSource.cs) or make a base class for these.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SpeedBoostSource : MonoBehaviour {

	public float boostIntensity;	// How strong the boost should be.
	public float duration;			// How long the boost should last.
	public bool collectable;		// Whether this powerup source should disappear when collected.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only applies buff to marble.
			other.GetComponent<Marble>().SpeedBoost(boostIntensity, duration);
			if (collectable) gameObject.SetActive(false);	// Disappears when collected.
		}	
	}
}
