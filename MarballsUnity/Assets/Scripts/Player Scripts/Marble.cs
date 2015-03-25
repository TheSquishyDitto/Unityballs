/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Jan. 28, 2015
/// Last Revision: Mar. 24, 2015
/// 
/// Class that controls marble properties and actions.
/// 
/// NOTES: - Current controls for marble are WASD, Space, and B. Press R to reset position.
/// 	   - This is the base class for all marbles and should be designed with inheritance in mind.
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

	public delegate void ApplyBuff(float intensity, float duration);	// This is declaring a sorta data type for a variable that can hold functions.

	// Variables
	#region Variables
	public GameMaster gm;				// Reference to the Game Master.
	public Transform cam;				// Reference to the main camera.
	public Rigidbody marbody;			// Reference to the marble's rigidbody.
	public ParticleSystem buffParticles;// Reference to aesthetic particles for buffs.

	public float defSpeedMultiplier;	// Default speed multiplier.
	public float defRevSpeed;			// Default rev speed.
	public float defMaxAngVelocity;		// Default maximum angular velocity.
	public float defJumpHeight;			// Default jump height.
	public float defSize;				// Default marble size.

	protected Vector3 inputDirection;	// Holds desired direction of input before applying it.
	public bool grounded;				// True if marble is on the ground, false otherwise.
	protected float shackle = 0.01f;	// Limiter constant for velocity.
	public float speedMultiplier;		// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed;				// Determines how quickly the marble will rev up to max angular velocity.

	protected bool hasJumped = false;	// Check if a jump has occured
	public float jumpHeight;			// How powerful the marble's jump is.
	public int maxJumps = 1;			// How many jumps marble can have.
	public int jumpsLeft = 1;			// How many jumps the marble has remaining.
																			
	protected RaycastHit hit;			// Saves raycast hit.

	public PowerUp heldBuff;			// What buff the marble is holding onto.
	public ApplyBuff buffFunction;		// Which function will be called to apply the buff.
	public GameObject heldParticles;	// The particles to instantiate when buff is used.
	public float heldIntensity;			// Intensity of held buff.
	public float heldDuration;			// Duration of held buff.

	public PowerUp buff = PowerUp.None;	// What buff the marble currently has.
	public float buffTimeMax;			// How much time the buff timer had at the beginning.
	public float buffTimer;				// How much time until a buff expires.

	public bool debugLights;			// Whether marble should light up under certain scenarios.

	//public Vector3 tangent;			// Tangent vector to terrain.
	//public Vector3 cross;				// Holds cross products temporarily.

	
	#endregion

	// Monobehaviour Functions - Typical Unity-provided functions.
	#region Monobehaviour Functions
	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.marble = this;	// Tells the Game Master that this is the currently controlled marble.
		marbody = GetComponent<Rigidbody>();
	}

	// Start - Use this for initialization. If a reference from the Game Master is needed, make it here.
	void Start () {
		inputDirection = new Vector3();
		//tangent = new Vector3();
		cam = gm.cam;
		ClearBuffs();	// Resets marble's properties to default.
		//rigidbody.maxAngularVelocity = defMaxAngVelocity;
	}
	
	// Update - Called once per frame.
	void Update () {

		// If useOnGrab is active, automatically use powerups when grabbed.
		if (gm.useOnGrab && buffFunction != null)
			UseBuff();

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
		marbody.AddTorque(Vector3.Cross(Vector3.up, inputDirection) * speedMultiplier * revSpeed * shackle);

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

			marbody.drag = 0.5f;
			marbody.AddForce(inputDirection * speedMultiplier * marbody.angularVelocity.magnitude * shackle, ForceMode.Impulse); // Applies force.
			inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.

			if (debugLights) gameObject.GetComponent<Light>().enabled = true;	// Marble may light up when on the ground.
		} else {
			marbody.drag = 0.1f;			
			if (debugLights) gameObject.GetComponent<Light>().enabled = false;	// Marble's light turns off if it was on.
		}

		// Handles jumping.
		if (hasJumped) {	
			Vector3 jumpDir = (grounded)? hit.normal : Vector3.up; // Jumps off of surface's normal if there is one,
																   // otherwise jumps straight up.
			marbody.AddForce (jumpHeight * jumpDir);
			jumpsLeft--;
			if (jumpsLeft == 0 && (buff == PowerUp.MultiJump || buff == PowerUp.SuperJump))
				ClearBuffs();	// If ball has used up its extra/special jumps, clears powerup state.

			hasJumped = false;
		}

	}

	#endregion

	// PowerUp Functions - Functions that (de)buff the marble's behavior.
	#region PowerUp Functions

	// ClearBuffs - Returns marble to its default state. Does not clear held buffs.
	public void ClearBuffs() {
		transform.localScale = new Vector3(defSize, defSize, defSize);
		marbody.maxAngularVelocity = defMaxAngVelocity;
		speedMultiplier = defSpeedMultiplier;
		revSpeed = defRevSpeed;
		jumpHeight = defJumpHeight;
		maxJumps = 1;
		jumpsLeft = (grounded)? 1 : 0;

		if (buffParticles) GameObject.Destroy(buffParticles.gameObject);

		buffTimer = 0;
		buffTimeMax = 0;
		buff = PowerUp.None;

		if (gm.hud) { gm.hud.HideActiveBuff(); }
	}

	// Ghost - Makes the marble ethereal, able to move through some objects.

	// HeliBall - Turns the marble into a helicopter or allows it to levitate.

	#endregion

	// Control Functions - Functions that allow the player to manipulate the marble.
	#region Control Functions
	// Forward.
	public void Forward() {
		inputDirection += cam.forward * Time.deltaTime;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}

	// Backward.
	public void Backward() {
		inputDirection -= cam.forward * Time.deltaTime;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}
	
	// Left.
	public void Left() {
		inputDirection -= cam.right * Time.deltaTime;
	}
	
	// Right.
	public void Right() {
		inputDirection += cam.right * Time.deltaTime;
	}
	
	// Jump. Can't jump in the air.
	public void Jump() {
		if ((grounded || buff == PowerUp.MultiJump) && jumpsLeft > 0 /*!hasJumped*/) {
			hasJumped = true;
		}
	}

	// UseBuff - Applies the marble's currently held powerup. Currently overwrites any existing one.
	public void UseBuff() {
		if (buffFunction != null) {
			ClearBuffs();	// Safely clears existing buff.
			buffFunction(heldIntensity, heldDuration);	// Uses held powerup function to apply changes.
			buffFunction = null;	// Clears held function after using it.

			// Clears held buff properties and transitions others to active properties.
			heldBuff = PowerUp.None;
			heldIntensity = 0;
			buffTimer = heldDuration;
			buffTimeMax = buffTimer;
			heldDuration = 0;

			if (gm.hud) { gm.hud.ShowActiveBuff(); }

			if (heldParticles) {
				buffParticles = Instantiate(heldParticles).GetComponent<ParticleSystem>();
				heldParticles = null;
			}
		}
	}
	
	// Very basic, extremely potent brake. Can currently pause marble midair for the most part.
	public void Brake() {
		marbody.velocity = Vector3.zero;
		marbody.angularVelocity = Vector3.zero;
	}
	
	// Respawns marble to roughly its starting position.
	public void Respawn() {
		//Transform respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
		
		marbody.velocity = Vector3.zero;
		marbody.angularVelocity = Vector3.zero;

		if (gm.respawn) {
			transform.position = gm.respawn.transform.position + new Vector3(0,5,0);
		} else {
			Debug.LogWarning("(Marble.cs) No spawn point available! Placing in default location..."); // DEBUG
			transform.position = new Vector3(0, 5, 0);
		}

	}
	
	#endregion
}
