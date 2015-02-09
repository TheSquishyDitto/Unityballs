/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb.  4, 2015
/// 
/// Class that controls marble properties and actions.
/// 
/// NOTES: - Current controls for marble are WASD, Space, and B. Press R to reset position.
/// 
/// TO DO: - Tweak movement until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Marble : MonoBehaviour {

	// Variables
	#region Variables
	public Transform cam;				// Reference to the main camera.
	public GameObject gauge;			// DEBUG REFERENCE TO GUI TEXT

	Vector3 inputDirection;				// Holds desired direction of input before applying it.
	bool grounded;						// True if marble is on the ground, false otherwise.
	public float speedMultiplier;		// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed;				// Determines how quickly the marble will rev up to max angular velocity.
	public float maxAngVelocity;		// Maximum angular velocity.
										// With a mass of 1, top speed currently limits itself to double the max angle velocity.
	public int jumpHeight;				// Specify jump height
																			
	private RaycastHit hit;				// Saves hit
	
	/*
	public Vector3 tangent;				// Alternative movement
	public Vector3 cross;
	*/
	
	#endregion

	// Start - Use this for initialization.
	void Start () {
		inputDirection = new Vector3();
		//tangent = new Vector3();
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		gauge = GameObject.FindGameObjectWithTag("Text"); // DEBUG
		rigidbody.maxAngularVelocity = maxAngVelocity;
	}
	
	// FixedUpdate - Called once per physics calculation.
	void FixedUpdate () {

		inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
		//tangent = Vector3.zero; // See above

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

		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.8f);	// Checks if marble is reasonably close to the ground		

		inputDirection.y = 0; // Removes vertical component from camera vectors.
		inputDirection = Vector3.Normalize(inputDirection); // Makes sure the magnitude of the direction is 1.

		// Spins marble to appropriate amount of spin speed.
		rigidbody.AddTorque(Vector3.Cross(Vector3.up, inputDirection) * speedMultiplier * revSpeed * Time.deltaTime);

		// Behavior is dependent on whether marble is in the air or on the ground.
		if (grounded) {
			/* Alternative method of movement
			cross = Vector3.Cross(inputDirection, hit.normal);
			float angle = Vector3.Angle(cross, inputDirection);
			tangent = Quaternion.AngleAxis(angle, hit.normal) * cross;
			tangent *= inputDirection.magnitude;
			*/
			
			
			// Force is only applied on the ground, and is dependent on how much the ball is spinning.
			// NOTE: Currently produces skidding when abrupting turning.
			//rigidbody.AddForce(tangent * speedMultiplier * rigidbody.angularVelocity.magnitude * Time.deltaTime, ForceMode.Impulse); // Applies force.
			rigidbody.AddForce(inputDirection * speedMultiplier * rigidbody.angularVelocity.magnitude * Time.deltaTime, ForceMode.Impulse); // Applies force.
			

			// Marble lights up and turns red on the ground.
			//gameObject.renderer.material.color = Color.red;
			gameObject.light.enabled = true;
		} else {
			// Marble dims and turns blue in air.
			//gameObject.renderer.material.color = Color.blue;
			gameObject.light.enabled = false;
		}

		// Jump. Can't jump in the air.
		if (Input.GetKeyDown (KeyCode.Space) && grounded) {
			Vector3 jump = Vector3.up;
			jump = hit.normal;
			
			rigidbody.AddForce (jumpHeight * jump);
		}

		// Very basic, extremely potent brake. Can currently pause marble midair for the most part.
		if (Input.GetKey (KeyCode.B)) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}

		// Respawns marble to roughly its starting position.
		if (Input.GetKeyDown (KeyCode.R)) {
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
			transform.position = new Vector3(0, 5, 0);
		}

		gauge.GetComponent<GUIText>().text = "Speed: " + Mathf.Round(rigidbody.velocity.magnitude); // DEBUG
	}
}
