using UnityEngine;
using System.Collections;

public class rotateMarble : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		gameObject.GetComponent<Rigidbody> ().AddTorque (transform.up * 10);
	}
}