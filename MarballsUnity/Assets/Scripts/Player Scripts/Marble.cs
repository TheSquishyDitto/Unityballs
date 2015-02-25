/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Jan. 28, 2015
/// Last Revision: Feb. 23, 2015
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

	// Enum for what powerup the marble has.
	public enum PowerUp {
		None,		// Normal marble state.
		SpeedBoost,	// Marble is faster
		MultiJump,	// Marble can jump multiple times
		SuperJump,  // Marble jumps very high
		Ghost,		// Transparent marble can clip through certain objects
		SizeChange, // Changes marble size
		HeliBall 	// Make the marble a helicopter (OPTIONAL)
	}

	// Variables
	#region Variables
	public GameMaster gm;				// Reference to the Game Master.
	public Transform cam;				// Reference to the main camera.
	public ParticleSystem buffParticles;// Reference to aesthetic particles for buffs.

	public float defSpeedMultiplier;	// Default speed multiplier.
	public float defRevSpeed;			// Default rev speed.
	public float defMaxAngVelocity;		// Default maximum angular velocity.
	public float defJumpHeight;			// Default jump height.
	public float defSize;				// Default marble size.

	Vector3 inputDirection;				// Holds desired direction of input before applying it.
	bool grounded;						// True if marble is on the ground, false otherwise.
	float shackle = 0.01f;				// Limiter constant for velocity.
	public float speedMultiplier;		// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed;				// Determines how quickly the marble will rev up to max angular velocity.

	bool hasJumped = false;				// Check if a jump has occured
	public float jumpHeight;			// How powerful the marble's jump is.
	public int maxJumps = 1;			// How many jumps marble can have.
	public int jumpsLeft = 1;			// How many jumps the marble has remaining.
																			
	private RaycastHit hit;				// Saves raycast hit.

	public PowerUp buff = PowerUp.None;	// What buff the marble currently has.
	public float buffTimer;				// How much time until a buff expires.

	public bool debugLights = true;		// Whether marble should light up under certain scenarios.

	//public Vector3 tangent;			// Tangent vector to terrain.
	//public Vector3 cross;				// Holds cross products temporarily.

	
	#endregion

	// Monobehaviour Functions - Typical Unity-provided functions.
	#region Monobehaviour Functions
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
		ClearBuffs();	// Resets marble's properties to default.
		//rigidbody.maxAngularVelocity = defMaxAngVelocity;

		// Allows the GameMaster to begin properly in the starting phase regardless of which scene we're in.
		if (gm.debug)
			gm.OnStart();	// DEBUG
	}
	
	// Update - Called once per frame.
	void Update () {
		if(Physics.Raycast(transform.position, rigidbody.velocity.normalized, cam.GetComponent<CameraController>().radius)) { // If camera hits a wall
			//cam.GetComponent<CameraController>().radius -= 1;
		}
/*
				else
		{
			cam.GetComponent<CameraController>().radius = cam.GetComponent<CameraController>().playerRadius;
		}
*/
		// Counts down until a buff runs out.
		if (buffTimer > 0 && !gm.paused) {
			buffTimer -= Time.deltaTime;

			if (buffTimer <= 0) {
				buffTimer = 0;
				ClearBuffs();
			}
		}
	}
	
	// FixedUpdate - Called once per physics calculation. This happens independently of frames.
	// NOTE: Time.deltaTime is unnecessary in FixedUpdate because it is integrated in physics calculations.
	void FixedUpdate () {
		//tangent = Vector3.zero; // See above

		bool touchdown = !grounded;	// Boolean used to see if the marble landed since the previous frame.
		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.8f * transform.localScale.x);	// Checks if marble is reasonably close to the ground		

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

			if (touchdown) jumpsLeft = maxJumps; // If marble has just hit the ground, refresh jumps.

			rigidbody.drag = 0.5f;
			rigidbody.AddForce(inputDirection * speedMultiplier * rigidbody.angularVelocity.magnitude * shackle/* * Time.deltaTime*/, ForceMode.Impulse); // Applies force.
			inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
			
			// Marble lights up and turns red on the ground.
			//gameObject.renderer.material.color = Color.red;
			if (debugLights) gameObject.light.enabled = true;
		} else {
			// Marble dims and turns blue in air.
			//gameObject.renderer.material.color = Color.blue;
			rigidbody.drag = 0.1f;			
			if (debugLights) gameObject.light.enabled = false;
		}

		// Handles jumping.
		if (hasJumped) {	
			Vector3 jumpDir = (grounded)? hit.normal : Vector3.up; // Jumps off of surface's normal if there is one,
																   // otherwise jumps straight up.
			rigidbody.AddForce (jumpHeight * jumpDir);
			jumpsLeft--;
			if (jumpsLeft == 0 && (buff == PowerUp.MultiJump || buff == PowerUp.SuperJump))
				ClearBuffs();	// If ball has used up its extra/special jumps, clears powerup state.

			//Debug.Log("(Marble.cs) Jumped! Jumps remaining: " + jumpsLeft); // DEBUG

			hasJumped = false;
		}

	}

	#endregion

	// PowerUp Functions - Functions that (de)buff the marble's behavior.
	#region PowerUp Functions
	// ClearBuffs - Returns marble to its default state.
	public void ClearBuffs() {
		transform.localScale = new Vector3(defSize, defSize, defSize);
		rigidbody.maxAngularVelocity = defMaxAngVelocity;
		speedMultiplier = defSpeedMultiplier;
		revSpeed = defRevSpeed;
		jumpHeight = defJumpHeight;
		maxJumps = 1;
		jumpsLeft = (grounded)? 1 : 0;
		

		if (buffParticles) GameObject.Destroy(buffParticles.gameObject);

		buffTimer = 0;
		buff = PowerUp.None;
	}

	// SpeedBoost - Modifies the marble's speed for a while.
	public void SpeedBoost(float intensity, float duration) {
		ClearBuffs();
		buff = PowerUp.SpeedBoost;

		buffParticles = ((GameObject)Instantiate(Resources.Load ("Prefabs/Particle Prefabs/Speedboost"))).particleSystem;

		speedMultiplier = intensity;
		buffTimer = duration;
	}

	// MultiJump - Grants the marble the ability to jump multiple times, even in midair. By default does not expire by time.
	public void MultiJump(int jumpCount, float duration = Mathf.Infinity) {
		ClearBuffs();
		buff = PowerUp.MultiJump;

		maxJumps = jumpCount;
		if (grounded) jumpsLeft = maxJumps;
	}

	// SuperJump - Modifies marble's jumping height.
	public void SuperJump(float intensity, float duration = Mathf.Infinity) {
		ClearBuffs();
		buff = PowerUp.SuperJump;

		buffParticles = ((GameObject)Instantiate(Resources.Load ("Prefabs/Particle Prefabs/Superjump"))).particleSystem;

		jumpHeight = intensity;
		buffTimer = duration;
	}

	// Ghost - Makes the marble ethereal, able to move through some objects.

	// SizeChange - Modifies marble's size.
	public void SizeChange(float newSize, float duration) {
		ClearBuffs();
		buff = PowerUp.SizeChange;

		buffParticles = ((GameObject)Instantiate(Resources.Load ("Prefabs/Particle Prefabs/Sizechange"))).particleSystem;

		transform.localScale = new Vector3(newSize, newSize, newSize);
		buffTimer = duration;
	}

	// HeliBall - Turns the marble into a helicopter or allows it to levitate.

	#endregion

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
		if ((grounded || buff == PowerUp.MultiJump) && jumpsLeft > 0 /*!hasJumped*/) {
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
		//Transform respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
		
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;

		if (gm.respawn) {
			transform.position = gm.respawn.position + new Vector3(0,5,0);
		} else {
			Debug.LogWarning("(Marble.cs) No spawn point available! Placing in default location..."); // DEBUG
			transform.position = new Vector3(0, 5, 0);
		}

	}
	
	#endregion
}
