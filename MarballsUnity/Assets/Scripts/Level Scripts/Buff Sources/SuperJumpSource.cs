/// <summary>
/// SuperJumpSource.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 23, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// Class for super jump granting entities.
/// 
/// NOTES: - Should probably either use general purpose version (BuffSource.cs) or make a base class for these.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SuperJumpSource : MonoBehaviour {

	public float boostIntensity;	// How powerful or weak the jump boost should be.
	public float duration;			// How long the boost lasts unused.
	public bool collectable;		// Whether the superjump source should disappear when collected.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only gives superjump to marbles.
			other.GetComponent<Marble>().SuperJump(boostIntensity, duration);
			if (collectable) gameObject.SetActive(false);	// Disappears when collected.
		}	
	}
}
