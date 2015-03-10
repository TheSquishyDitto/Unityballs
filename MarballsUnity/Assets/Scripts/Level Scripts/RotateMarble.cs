using UnityEngine;
using System.Collections;

public class RotateMarble : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		gameObject.GetComponent<Rigidbody>().AddTorque (transform.up * transform.position.x);
	}
}