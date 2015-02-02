/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb.  2, 2015
/// 
/// Class that controls marble properties and actions.
/// 
/// NOTES: - Current controls for marble are WASD, Space, and B.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Detect when ball is in air to prevent multijumping.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Marble : MonoBehaviour {
	// Variables
	public Transform cam;				// Reference to the main camera.

	Vector3 inputDirection;					// Holds desired direction of input before applying it.
	public float speedMultiplier;			// How speedy the variety of marble should be. Changes must be large to be noticeable.
	// public float weight;					// Weight may be nice for messing with gravity.


	// Start - Use this for initialization
	void Start () {
		inputDirection = new Vector3();
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
	}
	
	// Update - Called once per frame
	void Update () {

		inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.

		// Forward.
		if (Input.GetKey (KeyCode.W)) {
			inputDirection += cam.forward;
		}

		// Backward.
		if (Input.GetKey (KeyCode.S)) {
			inputDirection -= cam.forward;
		}

		// Left.
		if (Input.GetKey (KeyCode.A)) {
			inputDirection -= cam.right;
		}

		// Right.
		if (Input.GetKey (KeyCode.D)) {
			inputDirection += cam.right;
		}

		inputDirection.y = 0; // Removes vertical component from camera vectors.
		inputDirection = Vector3.Normalize(inputDirection); // Makes sure the magnitude of the direction is 1.

		rigidbody.AddForce(inputDirection * speedMultiplier * Time.deltaTime); // Applies force.
		//rigidbody.AddTorque(inputDirection * speedMultiplier * Time.deltaTime); // Can also try this, but I haven't.

		// Jump.
		if (Input.GetKeyDown (KeyCode.Space)) {
			rigidbody.AddForce (0, 200, 0);
		}

		// Very basic, extremely potent brake. Can currently pause ball midair for the most part.
		if (Input.GetKey (KeyCode.B)) {
			rigidbody.velocity = Vector3.zero;
		}
	}
}
