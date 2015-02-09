/// <summary>
/// Booster.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan.  9, 2015
/// Last Revision: Feb.  9, 2015
/// 
/// Class that handles booster panel properties.
/// 
/// NOTES: - Might be better to make it inherit from a base panel class if other panel types have common behavior.
/// 
/// TO DO: - Tweak until it behaves as desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Booster : MonoBehaviour {

	public float boostPower;	// How strong the booster panel is.
	
	// Use this for initialization
	void Start () {
	
	}

	// OnTriggerEnter - What happens when another object enters this collider.
	void OnTriggerEnter() {
		light.intensity = 7.5f;
		// [insert cool sound effect here]
	}

	// OnTriggerExit - What happens when another object exits this collider.
	void OnTriggerExit() {
		light.intensity = 1;
	}

	// OnTriggerStay - As long as another object is within the collision zone.
	void OnTriggerStay(Collider other) {
		// If the other object has a rigidbody, boost it along.
		// Currently always boosts based on transform of physical panel.
		if (other.attachedRigidbody)
			other.attachedRigidbody.AddForce(-transform.right * boostPower);
		
	}
}
