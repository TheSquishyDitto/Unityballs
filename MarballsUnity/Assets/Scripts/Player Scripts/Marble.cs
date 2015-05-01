﻿/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Jan. 28, 2015
/// Last Revision: May   1, 2015
/// 
/// Class that controls marble properties and actions.
/// 
/// NOTES: - Current controls for marble are WASD, Space, and B. Press R to respawn.
/// 	   - This is the base class for all marbles and should be designed with inheritance in mind.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Getters and setters may be nice to have soon!
/// 	   - Fix sounds: marble doesn't play sound if not technically grounded, even if rolling along.
/// 
/// </summary>

using UnityEngine;
using UnityEditor;
using System.Collections;

public class Marble : MonoBehaviour, IKillable {

	// Enum for what powerup the marble has.
	public enum PowerUp {
		None,		// Normal marble state.
		SpeedBoost,	// Marble is faster
		MultiJump,	// Marble can jump multiple times
		SuperJump,  // Marble jumps very high
		Ghost,		// Transparent marble can clip through certain objects
		SizeChange, // Changes marble size
		HeliBall, 	// Make the marble a helicopter
		God			// Debug ball that can go anywhere.
	}

	public delegate void ApplyBuff(float intensity, float duration);	// This is declaring a sorta data type for a variable that can hold functions.
	public delegate void ModifyBehavior();		// Datatype for most delegates that modify marble behavior.
	public delegate void EventAction(); 		// Datatype that most event functions will use.

	// Variables
	#region Variables
	[Header("References")]
	public GameMaster gm;				// Reference to the Game Master.
	public Transform cam;				// Reference to the main camera.
	public Transform marform;			// Reference to the marble's transform.
	public Rigidbody marbody;			// Reference to the marble's rigidbody.
	protected SphereCollider ballCol;	// Reference to the marble's collider.
	public AudioSource[] ballin;		// Reference to the marble's rolling sound.

	[Header("Starting Values")]
	public float maxAngVelocity = 50;	// Default maximum angular velocity.
	public float defSize = 1;			// Default marble size.
	public float defMass = 1;			// Default marble mass
	
	[Header("Movement Values")]
	public float speedMultiplier = 1;	// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed = 1000;		// Determines how quickly the marble will rev up to max angular velocity.
	public float brakeSpeed = 2;		// How fast the marble can brake in normal gameplay.
	public Vector3 inputDirection;		// Holds desired direction of input before applying it.
	public float shackle = 0.01f;		// Limiter constant for velocity.
	public ModifyBehavior moveFunction;	// Variables holding any changes to movement behavior.

	public float jumpHeight = 1300;		// How powerful the marble's jump is.
	public int midairJumps = 0;			// How many times the marble can jump in midair.
	public bool canJump = true;			// Whether the marble can currently jump or not.
	public bool grounded;				// True if marble is on the ground, false otherwise.
	public ModifyBehavior jumpFunction;	// Variable holding any changes to jump behavior.
	
	public RaycastHit hit;				// Saves grounded raycast hit.

	[Header("Held Buff Values")]
	[Tooltip("Read-only: Does not give buffs.")] // <- This lets you add tooltips to the Unity inspector!
	public PowerUp heldBuff;			// What buff the marble is holding onto.
	public GameObject heldParticles;	// The particles to instantiate when buff is used.
	public float heldIntensity;			// Intensity of held buff.
	public float heldDuration;			// Duration of held buff.
	public ApplyBuff buffFunction;		// Which function will be called to apply the buff.
	public ModifyBehavior heldCleaner;	// Which function will be called to remove the buff after it's used.

	[Header("Active Buff Values")]
	[Tooltip("Read-only: Does not give buffs.")] // <- This lets you add tooltips to the Unity inspector!
	public PowerUp buff = PowerUp.None;	// What buff the marble currently has.
	public GameObject buffParticles;	// Reference to aesthetic particles for buffs.
	public float buffTimeMax;			// How much time the buff timer had at the beginning.
	public float buffTimer;				// How much time until a buff expires.
	public ModifyBehavior buffCleaner;	// Which function should be used to get rid of the buff.

	[Header("Sounds")]
	public AudioClip rollingSound;		// Sound the marble makes when rolling.
	public AudioClip jumpSound;			// Sound the marble makes when jumping.
	public AudioClip landSound;			// Sound the marble makes when hitting anything.

	[Header("Misc. Options")]
	public bool flashLight;				// Whether marble should light up under certain scenarios.

	public static event EventAction die;	 // Container for actions when player dies.
	public static event EventAction respawn; // Container for actions when player respawns.
	
	#endregion

	// Monobehaviour Functions - Typical Unity-provided functions.
	#region Monobehaviour Functions
	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.marble = this;	// Tells the Game Master that this is the currently controlled marble.
		marform = transform;
		marbody = GetComponent<Rigidbody>();
		ballCol = GetComponent<SphereCollider>();
		ballin = GetComponents<AudioSource>();
	}

	// OnEnable - Called when the marble is activated. Used to subscribe to events.
	void OnEnable() {
		GameMaster.pan += Respawn; 		// When the game starts, marble should respawn.
		GameMaster.start += Respawn; 	// When the game starts, marble should respawn.
		GameMaster.play += ResetState;	// When the gameplay begins, marble should be fresh.
	}

	// OnDisable - Called when the marble is deactivated. Used to unsubscribe from events.
	// NOTE: Anything subscribed to in OnEnable should be unsubscribed from here to prevent memory leaks.
	void OnDisable() {
		GameMaster.pan -= Respawn;
		GameMaster.start -= Respawn;
		GameMaster.play -= ResetState;
	}

	// Start - Use this for initialization. If a reference from the Game Master is needed, make it here.
	void Start () {
		inputDirection = Vector3.zero;
		cam = gm.cam;
		ClearBuffs();	// Resets marble's properties to default.
		marbody.maxAngularVelocity = maxAngVelocity;
		transform.localScale = Vector3.one * defSize;
		marbody.interpolation = RigidbodyInterpolation.Interpolate; // Used for smooth damping.
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
		// Check if marble is reasonably close to the ground.
		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, (ballCol.radius * marform.localScale.x) + 0.1f);

		inputDirection = Vector3.Normalize(inputDirection); // Makes sure the magnitude of the direction is 1.
		Move();	// Move the marble.

		// Behavior is dependent on whether marble is in the air or on the ground.
		if (grounded) {
			if (flashLight) gameObject.GetComponent<Light>().enabled = true;	// Marble may light up when on the ground.
			if (!ballin[0].isPlaying) ballin[0].Play();
			ballin[0].volume = marbody.velocity.magnitude/60f;
			//ballin[0].pitch = (Mathf.Sin(Time.time / 4) / 4f) + 1f;
		} else {			
			if (flashLight) gameObject.GetComponent<Light>().enabled = false;	// Marble's light turns off if it was on.
			//ballin.enabled = false;
			ballin[0].Stop();
			//ballin[0].volume = Mathf.MoveTowards(ballin[0].volume, 0, 0.1f);
		}
	}

	#endregion

	// Die - Called when player should die.
	public void Die() {

		AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/WilhelmScream"), gm.cam.position);
		ClearBuffs();
		marbody.isKinematic = true;
		GetComponent<MeshRenderer>().enabled = false;
		ballCol.enabled = false;
		GameObject burst = (GameObject)(Instantiate(Resources.Load("Prefabs/Particle Prefabs/Deathburst")));
		burst.transform.position = marform.position;
		Destroy(burst, 2);

		if (die != null) die();

		Invoke("Respawn", 4f);	// Respawns in a few seconds.
	}

	// PowerUp Functions - Functions that (de)buff the marble's behavior.
	#region PowerUp Functions

	// ClearBuffs - Returns marble to its default state. Does not clear held buffs.
	public void ClearBuffs() {

		if (buffCleaner != null) {
			buffCleaner();
			buffCleaner = null;
		}

		if (buffParticles) {
			GameObject.Destroy(buffParticles.gameObject);
			buffParticles = null;
		}

		buffTimer = 0;
		buffTimeMax = 0;
		buff = PowerUp.None;

		if (gm.hud) { gm.hud.HideActiveBuff(); }
	}

	// ClearAllBuffs - Does the same as ClearBuffs, but clears the held buff as well.
	// NOTE: Current implementation will cause the buff function to be triggered first.
	public void ClearAllBuffs() {
		UseBuff();
		ClearBuffs();
	}

	#endregion

	// Control Functions - Functions that allow the player to manipulate the marble.
	// NOTE: If we want to add controller support, we'll need to add axis-based functions.
	#region Control Functions
	// Forward.
	public void Forward() {
		inputDirection += cam.forward;// * Time.deltaTime;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}

	// Backward.
	public void Backward() {
		inputDirection -= cam.forward;// * Time.deltaTime;
		inputDirection.y = 0; // Removes vertical component from camera vectors.
	}
	
	// Left.
	public void Left() {
		inputDirection -= cam.right;// * Time.deltaTime;
	}
	
	// Right.
	public void Right() {
		inputDirection += cam.right;// * Time.deltaTime;
	}

	// Move - Adds force in the direction of input.
	void Move() {
		// If movement is modified, use the modification.
		if (moveFunction != null) {
			moveFunction();
		
		// Otherwise, do things normally.
		} else {
			// Spins marble to appropriate amount of spin speed.
			marbody.AddTorque(Vector3.Cross(Vector3.up, inputDirection) * speedMultiplier * revSpeed * shackle);

			// Applies force if marble is on the ground.
			if (grounded) {
				marbody.drag = 0.5f;
				marbody.AddForce(inputDirection * speedMultiplier * marbody.angularVelocity.magnitude * shackle, ForceMode.Impulse);
				inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
			}
			else {
				marbody.drag = 0.1f;
			}
		}
	}

	// Jump - Makes the marble actually jump.
	public void Jump() {
		// If there are modified conditions, use them.
		if (jumpFunction != null) {
			jumpFunction();

		// Otherwise, use the vanilla conditions. Marble may only jump when on the ground.
		} else {
			if (grounded && canJump) {
				// Directly set velocity to avoid excessively complicated checks.
				marbody.velocity = new Vector3(marbody.velocity.x, 0, marbody.velocity.z) + (hit.normal * (jumpHeight / 100));
				//if (!ballin.isPlaying) ballin.PlayOneShot(jumpSound); // Keeps duplicating the sound for some reason
			}
		}
	}

	// JumpCooldown - If the marble needs to wait for some reason before jumping again, Invoke this.
	public void JumpCooldown() {
		canJump = true;
		//Debug.Log("Refreshed! " + gm.timer);
	}

	// UseBuff - Applies the marble's currently held powerup. Currently overwrites any existing one.
	public void UseBuff() {
		if (buffFunction != null) {
			// Clears existing buff and applies new one.
			ClearBuffs();
			buffFunction(heldIntensity, heldDuration);
			buffFunction = null;

			// Clears held buff properties and transitions others to active properties.
			heldBuff = PowerUp.None;
			heldIntensity = 0;
			buffTimer = heldDuration;
			buffTimeMax = buffTimer;
			heldDuration = 0;
			buffCleaner = heldCleaner;
			heldCleaner = null;

			if (gm.hud) { gm.hud.ShowActiveBuff(); }

			if (heldParticles) {
				buffParticles = Instantiate(heldParticles);
				heldParticles = null;
			}
		}
	}
	
	// Brake - Slows down and stops the marble.
	public void Brake() {

		if (gm.debug) { // If in debug mode, just freeze the ball.
			ForceBrake();

		} else { // Otherwise, gradually slow it like an actual brake.
			if (grounded) marbody.AddForce(new Vector3(-marbody.velocity.x * Time.deltaTime, 0, -marbody.velocity.z * Time.deltaTime) * brakeSpeed, ForceMode.Impulse);
		}
	}

	// ForceBrake - Completely stops the marble from moving.
	public void ForceBrake() {
		marbody.velocity = Vector3.zero;
		marbody.angularVelocity = Vector3.zero;
	}
	
	// Respawn - Respawns the marble to roughly its starting position.
	public void Respawn() {
		Vector3 respawnOffset = new Vector3(0, 3, 0);

		ForceBrake();
		ResetState();

		if (gm.checkpoint) {
			marform.position = gm.checkpoint.position + respawnOffset;

			// Rotates camera to match checkpoint's rotation.
			ICamera camScript = cam.GetComponent<ICamera>();
			camScript.ResetPosition();	// Makes sure camera is in right location.
			camScript.Freeze(true);		// Prevents it from updating while changes are made.

			// Rotates it to face the direction of the checkpoint.
			cam.RotateAround(marform.position, Vector3.up, -Vector3.Angle(gm.checkpoint.forward, 
			                                                             Vector3.Scale(cam.forward, new Vector3(1, 0, 1))));

			camScript.RecalculatePosition();	// Forces it to update to new position.
			camScript.Freeze(false);			// Unlocks.

		} else if (gm.respawn) {
			marform.position = gm.respawn.transform.position + respawnOffset;
		} else {
			Debug.LogWarning("(Marble.cs) No spawn point available! Placing in default location..."); // DEBUG
			marform.position = respawnOffset;
		}

		if (respawn != null) respawn();

	}

	// ResetState - Clears marble's conditions. 
	public void ResetState() {
		marbody.isKinematic = false;
		marbody.constraints = RigidbodyConstraints.None;
		GetComponent<MeshRenderer>().enabled = true;
		ballCol.enabled = true;
		ClearAllBuffs();
	}
	
	#endregion

	// OnCollisionEnter - Called when the marble bumps into anything.
	void OnCollisionEnter(Collision col) {
		ballin[1].PlayOneShot(landSound, col.relativeVelocity.sqrMagnitude / 10000);
	}

	/*	TANGENT MOVEMENT CODE

	variables:

	//public Vector3 tangent;			// Tangent vector to terrain.
	//public Vector3 cross;				// Holds cross products temporarily.

	start:
	//tangent = new Vector3();

	fixedupdate:
	//tangent = Vector3.zero; // See above
	...
	cross = Vector3.Cross(inputDirection, hit.normal);
	float angle = Vector3.Angle(cross, inputDirection);
	tangent = Quaternion.AngleAxis(angle, hit.normal) * cross;
	tangent *= inputDirection.magnitude;

	// Force is only applied on the ground, and is dependent on how much the ball is spinning.
	//rigidbody.AddForce(tangent * speedMultiplier * rigidbody.angularVelocity.magnitude * Time.deltaTime, ForceMode.Impulse); // Applies force.

	*/
}
