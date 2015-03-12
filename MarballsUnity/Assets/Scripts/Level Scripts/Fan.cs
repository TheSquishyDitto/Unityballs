using UnityEngine;
using System.Collections;

public class Fan : MonoBehaviour {

	public float boostPower;	// How strong the booster panel is.
	Rigidbody boostee;			// Reference to the object being boosted.
	
	// Use this for initialization
	void Start () {
		
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
