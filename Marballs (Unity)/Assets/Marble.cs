using UnityEngine;
using System.Collections;

public class Marble : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			this.gameObject.rigidbody.AddForce (Vector3.forward);
		}

		if (Input.GetKey (KeyCode.S)) {
			this.gameObject.rigidbody.AddForce (0f, 0f, -1f);
		}

		if (Input.GetKey (KeyCode.A)) {
			this.gameObject.rigidbody.AddForce (-1f, 0f, 0f);
		}

		if (Input.GetKey (KeyCode.D)) {
			this.gameObject.rigidbody.AddForce (1f, 0f, 0f);
		}

		if (Input.GetKeyDown (KeyCode.Space)) {
			this.gameObject.rigidbody.AddForce (0f, 500f, 0f);
		}
	}
}
