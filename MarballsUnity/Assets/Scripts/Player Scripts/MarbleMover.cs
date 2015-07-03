/// <summary>
/// MarbleMover.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Jun. 23, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class that controls marble movement
/// 
/// NOTES: - Current controls for marble are WASD, Space, and B.
/// 
/// TO DO: - Tweak movement until desired.
/// 	   - Getters and setters may be nice to have soon!
/// 	   - Fix sounds: marble doesn't play sound if not technically grounded, even if rolling along.
/// 	   - Look into changing jump cooldown to use timestamp method.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MarbleMover : MonoBehaviour {

	// Variables
	#region Variables
	[Header("References")]
	Settings settings;					// Reference to the game settings.

	public Transform marform;			// Reference to the marble's transform.
	public Rigidbody marbody;			// Reference to the marble's rigidbody.
	public Transform cam;				// Reference to the camera.
	protected SphereCollider ballCol;	// Reference to the marble's collider.

	[Header("Movement Values")]
	public float speedMultiplier = 1;	// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed = 1000;		// Determines how quickly the marble will rev up to max angular velocity.
	public float brakeSpeed = 2;		// How fast the marble can brake in normal gameplay.
	public Vector3 inputDirection;		// Holds desired direction of input before applying it.
	public float shackle = 0.01f;		// Limiter constant for velocity.
	public UnityAction moveFunction;	// Variables holding any changes to movement behavior.
	
	public float jumpHeight = 1300;		// How powerful the marble's jump is.
	public int midairJumps = 0;			// How many times the marble can jump in midair.
	public bool canJump = true;			// Whether the marble can currently jump or not.
	public bool grounded;				// True if marble is on the ground, false otherwise.
	public RaycastHit hit;				// Saves grounded raycast hit.
	public UnityAction jumpFunction;	// Variable holding any changes to jump behavior.
	
	#endregion

	// Monobehaviour Functions
	#region Monobehaviour Functions
	// Awake - Called before anything else.
	void Awake() {
		settings = GameMaster.LoadSettings();
		marform = transform;
		marbody = GetComponent<Rigidbody>();
		ballCol = GetComponent<SphereCollider>();
	}

	// OnEnable - Called when object is activated. Subscribes to events.
	void OnEnable() {
		Messenger.AddListener("MarbleForward", Forward);
		Messenger.AddListener("MarbleBackward", Backward);
		Messenger.AddListener("MarbleLeft", Left);
		Messenger.AddListener("MarbleRight", Right);
		Messenger.AddListener("MarbleJump", Jump);
		Messenger.AddListener("MarbleBrake", Brake);
	}

	// OnDisable - Called when object is deactivated. Unsubscribes from events to prevent memory leaks.
	void OnDisable() {
		Messenger.RemoveListener("MarbleForward", Forward);
		Messenger.RemoveListener("MarbleBackward", Backward);
		Messenger.RemoveListener("MarbleLeft", Left);
		Messenger.RemoveListener("MarbleRight", Right);
		Messenger.RemoveListener("MarbleJump", Jump);
		Messenger.RemoveListener("MarbleBrake", Brake);
	}

	// Use this for initialization
	void Start () {
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		inputDirection = Vector3.zero;
	}

	// FixedUpdate - Called every physics tick.
	void FixedUpdate() {
		// Check if marble is reasonably close to the ground.
		grounded = Physics.Raycast(transform.position, Vector3.down, out hit, (ballCol.radius * marform.localScale.x) + 0.1f);
		
		inputDirection = Vector3.Normalize(inputDirection); // Makes sure the magnitude of the direction is 1.
		Move();	// Move the marble.
		inputDirection = Vector3.zero;
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
				Vector3 jumpDir = hit.normal;
				jumpDir += inputDirection * Mathf.Clamp(1 / (marbody.velocity.magnitude + 0.001f), 0, 1);	// Allows jumps in this state to be more directionally influenced.
				jumpDir = jumpDir.normalized;
				
				// Directly set velocity to avoid excessively complicated checks.
				marbody.velocity = new Vector3(marbody.velocity.x, 0, marbody.velocity.z) + (jumpDir * (jumpHeight / 100));
				
				// Play jumping sound.
				//if (!ballin[1].isPlaying) ballin[1].PlayOneShot(jumpSound); // Keeps duplicating the sound for some reason
			}
		}
	}
	
	// JumpCooldown - If the marble needs to wait for some reason before jumping again, Invoke this.
	public void JumpCooldown() {
		canJump = true;
	}

	// Brake - Slows down and stops the marble.
	public void Brake() {
		
		if (settings.debug) { // If in debug mode, just freeze the ball.
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

	#endregion
}
