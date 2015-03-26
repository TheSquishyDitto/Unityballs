/// <summary>
/// Fan.cs
/// Authors: Charlie Sun, Chris Viqueira, Kyle Dawson
/// Date Created:  ???
/// Last Revision: Mar. 26, 2015
/// 
/// Class that controls fan behavior.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour {

	public float boostPower;	// How strong the booster panel is.
	Rigidbody boostee;			// Reference to the object being boosted.

	public Transform blades;	// Reference to the fan blades.
	public ParticleSystem wind;	// Reference to the wind particles.
	//Collider windZone;			// Reference to the fan's collider.
	
	// Use this for initialization
	void Start () {
		//windZone = GetComponent<CapsuleCollider>();
	}

	// Update - Called every frame.
	void Update () {
		blades.Rotate(0, 0, boostPower * boostPower * Time.deltaTime); // Rotates blades based on speed.
		wind.startSpeed = boostPower / 2f; // Blows particles based on speed.
	}

	// FixedUpdate - Physics update function.
	void FixedUpdate() {
		if (boostee)
			boostee.AddForce(transform.right * boostPower * Time.timeScale);
	}
	
	// OnTriggerEnter - What happens when another object enters this collider.
	void OnTriggerEnter(Collider other) {
		// [insert cool sound effect here]
		if (other.attachedRigidbody)
			boostee = other.attachedRigidbody;
	}
	
	// OnTriggerExit - What happens when another object exits this collider.
	void OnTriggerExit(Collider other) {
		boostee = null;
	}
}
