/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb. 11, 2015
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
	public GameMaster gm;				// Reference to the Game Master.
	public Transform cam;				// Reference to the main camera.

	Vector3 inputDirection;				// Holds desired direction of input before applying it.
	bool grounded;						// True if marble is on the ground, false otherwise.
	public float speedMultiplier;		// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed;				// Determines how quickly the marble will rev up to max angular velocity.
	public float maxAngVelocity;		// Maximum angular velocity.
	float shackle = 0.01f;				// Limiter for velocity.

	public int jumpHeight;				// Specify jump height
	bool hasJumped = false;				// Check if a jump has occured
																			
	private RaycastHit hit;				// Saves raycast hit.

	//public Vector3 tangent;			// Tangent vector to terrain.
	//public Vector3 cross;				// Holds cross products temporarily.

	
	#endregion

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.marble = this.transform;	// Tells the Game Master that this is the currently controlled marble.
	}

	// Start - Use this for initialization. If a reference from the Game Master is needed, make it here.
	void Start () {
		inputDirection = new Vector3();
		//tangent = new Vector3();
		cam = gm.cam;
		rigidbody.maxAngularVelocity = maxAngVelocity;
	}
	
	// Update - Called once per frame.
	void Update () {

	}
	
	// FixedUpdate - Called once per physics calculation. This happens independently of frames.
	// NOTE: Time.deltaTime is unnecessary in FixedUpdate because it is integrated in physics calculations.
	void FixedUpdate () {
		//tangent = Vector3.zero; // See above

		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.8f);	// Checks if marble is reasonably close to the ground		
		inputDirection = Vector3.Normalize(inputDirection); // Makes sure the magnitude of the direction is 1.

		// Spins marble to appropriate amount of spin speed.
		rigidbody.AddTorque(Vector3.Cross(Vector3.up, inputDirection) * speedMultiplier * revSpeed * shackle/* * Time.deltaTime*/);

		// Behavior is dependent on whether marble is in the air or on the ground.
		if (grounded) {
			/* Alternative method of movement
			cross = Vector3.Cross(inputDirection, hit.normal);
			float angle = Vector3.Angle(cross, inputDirection);
			tangent = Quaternion.AngleAxis(angle, hit.normal) * cross;
			tangent *= inputDirection.magnitude;			*/

			// Force is only applied on the ground, and is dependent on how much the ball is spinning.
			//rigidbody.AddForce(tangent * speedMultiplier * rigidbody.angularVelocity.magnitude * Time.deltaTime, ForceMode.Impulse); // Applies force.
			rigidbody.drag = 0.5f;
			rigidbody.AddForce(inputDirection * speedMultiplier * rigidbody.angularVelocity.magnitude * shackle/* * Time.deltaTime*/, ForceMode.Impulse); // Applies force.
			inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
			
			// Marble lights up and turns red on the ground.
			//gameObject.renderer.material.color = Color.red;
			gameObject.light.enabled = true;
		} else {
			// Marble dims and turns blue in air.
			//gameObject.renderer.material.color = Color.blue;
			rigidbody.drag = 0.1f;			
			gameObject.light.enabled = false;
		}

		// Handles jumping.
		if (hasJumped) {
			Debug.Log("Jump");
			
			Vector3 jump;
			jump = hit.normal;
			
			rigidbody.AddForce (jumpHeight * jump);
			hasJumped = false;
		}

	}

	// Control Functions - Functions that allow the player to manipulate the marble.
	#region Control Functions
	// Forward.
	public void Forward() {
		inputDirection += cam.forward;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}

	// Backward.
	public void Backward() {
		inputDirection -= cam.forward;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}
	
	// Left.
	public void Left() {
		inputDirection -= cam.right;
	}
	
	// Right.
	public void Right() {
		inputDirection += cam.right;
	}
	
	// Jump. Can't jump in the air.
	public void Jump() {
		if (grounded && !hasJumped) {
			hasJumped = true;
		}
	}
	
	// Very basic, extremely potent brake. Can currently pause marble midair for the most part.
	public void Brake() {
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}
	
	// Respawns marble to roughly its starting position.
	public void Respawn() {
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		transform.position = new Vector3(0, 5, 0);
	}
	
	#endregion
}
