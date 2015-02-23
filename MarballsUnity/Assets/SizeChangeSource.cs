/// <summary>
/// SizeChangeSource.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 23, 2015
/// Last Revision: Feb. 23, 2015
/// 
/// Class for size change granting entities.
/// 
/// NOTES: - Should probably either use general purpose version (BuffSource.cs) or make a base class for these.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SizeChangeSource : MonoBehaviour {

	public float boostIntensity;	// How large/small the marble should become.
	public float duration;			// How long this change lasts.
	public bool collectable;		// Whether change source should disappear when collected.

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only changes marble size.
			other.GetComponent<Marble>().SizeChange(boostIntensity, duration);
			if (collectable) gameObject.SetActive(false);	// Disappears when collected.
		}	
	}
}
