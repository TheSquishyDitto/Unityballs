using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
		
	public float theta;  // degrees around y-axis
	public float psy;	 // degrees around x-axis
	public float radius; // distance from ball

	public Transform ball; // get the ball coordinates

	// Use this for initialization
	void Start () {
		/*theta = 0f;
		psy = 45f;*/
		radius = .001f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			psy += .01f;		
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			psy -= .01f;		
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			theta -= .01f;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			theta += .01f;	
		}
					
	}

	void LateUpdate () {
		gameObject.transform.position = getSphericalPosition ();
	}


	// Return spherical coordinate of camera
	public Vector3 getSphericalPosition() {
		Vector3 retPos = new Vector3();

		retPos.x = radius * (Mathf.Cos (psy) * Mathf.Rad2Deg) * (Mathf.Cos (theta) * Mathf.Rad2Deg) + ball.position.x;
		retPos.y = radius * (Mathf.Sin (psy)  * Mathf.Rad2Deg)  + ball.position.y;
		retPos.z = radius * (Mathf.Cos (psy) * Mathf.Rad2Deg) * (Mathf.Sin (theta) * Mathf.Rad2Deg)  + ball.position.z;

		return retPos;
	}
}
