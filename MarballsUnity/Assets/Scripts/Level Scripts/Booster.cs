/// <summary>
/// Booster.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan.  9, 2015
/// Last Revision: Mar.  9, 2015
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
	Rigidbody boostee;			// Reference to the object being boosted.
	
	// Use this for initialization
	void Start () {
	
	}

	// FixedUpdate - Physics update function.
	void FixedUpdate() {
		if (boostee)
			boostee.AddForce(-transform.right * boostPower * Time.timeScale);
	}

	// OnTriggerEnter - What happens when another object enters this collider.
<<<<<<< HEAD
	void OnTriggerEnter(Collider other) {
=======
	void OnTriggerEnter() {
>>>>>>> 646579061aaf4323906bfee14b256a360ba1dd70
		GetComponent<Light>().intensity = 7.5f;
		// [insert cool sound effect here]
		if (other.attachedRigidbody)
			boostee = other.attachedRigidbody;
	}

	// OnTriggerExit - What happens when another object exits this collider.
<<<<<<< HEAD
	void OnTriggerExit(Collider other) {
		GetComponent<Light>().intensity = 1;
		boostee = null;
=======
	void OnTriggerExit() {
		GetComponent<Light>().intensity = 1;
>>>>>>> 646579061aaf4323906bfee14b256a360ba1dd70
	}

	/*// OnTriggerStay - As long as another object is within the collision zone.
	void OnTriggerStay(Collider other) {
		// If the other object has a rigidbody, boost it along.
		// Currently always boosts based on transform of physical panel.
		if (other.attachedRigidbody)

		
	}*/
}
