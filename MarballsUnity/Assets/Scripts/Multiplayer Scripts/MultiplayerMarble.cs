/// <summary>
/// MultiplayerMarble.cs
/// Authors: Kyle Dawson
/// Date Created:  May   5, 2015
/// Last Revision: May   7, 2015
/// 
/// Class for networked instances of the marble class.
/// 
/// NOTES: - Currently only supports basic movement; no buffs or fancy events.
/// 
/// TO DO: - Re-add offline features.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MultiplayerMarble : MonoBehaviour, IKillable {
	
	// Variables
	#region Variables
	[Header("References")]
	public NetworkMaster net;			// Reference to the Network Master.
	public NetworkView netView;			// Reference to the network view.
	public Transform cam;				// Reference to the main camera.
	public Transform marform;			// Reference to the marble's transform.
	public Rigidbody marbody;			// Reference to the marble's rigidbody.
	protected SphereCollider ballCol;	// Reference to the marble's collider.
	public GameObject deathBurst;		// Reference to the marble's death particles.
	public AudioSource[] ballin;		// Reference to the marble's rolling sound.
	
	[Header("Starting Values")]
	Vector3 spawnPoint;
	public float maxAngVelocity = 50;	// Default maximum angular velocity.
	public float defSize = 1;			// Default marble size.
	public float defMass = 1;			// Default marble mass
	
	[Header("Movement Values")]
	public float speedMultiplier = 1;	// How speedy the variety of marble should be. Changes are now highly noticeable.
	public float revSpeed = 1000;		// Determines how quickly the marble will rev up to max angular velocity.
	public float brakeSpeed = 2;		// How fast the marble can brake in normal gameplay.
	public Vector3 inputDirection;		// Holds desired direction of input before applying it.
	public float shackle = 0.01f;		// Limiter constant for velocity.
//	public ModifyBehavior moveFunction;	// Variables holding any changes to movement behavior.
	
	public float jumpHeight = 1300;		// How powerful the marble's jump is.
//	public int midairJumps = 0;			// How many times the marble can jump in midair.
	public bool canJump = true;			// Whether the marble can currently jump or not.
	public bool grounded;				// True if marble is on the ground, false otherwise.
//	public ModifyBehavior jumpFunction;	// Variable holding any changes to jump behavior.
	
	public RaycastHit hit;				// Saves grounded raycast hit.
	
	[Header("Sounds")]
	public AudioClip rollingSound;		// Sound the marble makes when rolling.
	public AudioClip jumpSound;			// Sound the marble makes when jumping.
	public AudioClip landSound;			// Sound the marble makes when hitting anything.
	
	[Header("Misc. Options")]
	public bool flashLight;				// Whether marble should light up under certain scenarios.
	
//	public static event EventAction die;	 // Container for actions when player dies.
//	public static event EventAction respawn; // Container for actions when player respawns.

	//public static int quantity;// = 0;
	
	#endregion
	
	// Monobehaviour Functions - Typical Unity-provided functions.
	#region Monobehaviour Functions
	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		netView = GetComponent<NetworkView>();
		marform = transform;
		marbody = GetComponent<Rigidbody>();
		ballCol = GetComponent<SphereCollider>();
		ballin = GetComponents<AudioSource>();
		cam.gameObject.SetActive(netView.isMine);
		//quantity++;
		//netView.RPC("UpdateQuantity", RPCMode.Server, 1);
	}
	
	// Start - Use this for initialization. If a reference from the Game Master is needed, make it here.
	void Start () {
		inputDirection = Vector3.zero;
		marbody.maxAngularVelocity = maxAngVelocity;
		marform.localScale = Vector3.one * defSize;
		spawnPoint = marform.position;
	}
	
	// Update - Called once per frame.
	void Update () {
		if (netView.isMine)
			MoveControls();
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
			if (!ballin[0].isPlaying) ballin[0].Play();
			ballin[0].volume = marbody.velocity.magnitude/60f;
			//ballin[0].pitch = (Mathf.Sin(Time.time / 4) / 4f) + 1f;
		} else {			
			//ballin.enabled = false;
			ballin[0].Stop();
			//ballin[0].volume = Mathf.MoveTowards(ballin[0].volume, 0, 0.1f);
		}
	}
	
	#endregion
	
	// Die - Called when player should die.
	public void Die() {	
		AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/WilhelmScream"), Vector3.zero/*gm.cam.position*/);
		marbody.isKinematic = true;
		GetComponent<MeshRenderer>().enabled = false;
		ballCol.enabled = false;
		Destroy(Instantiate(Resources.Load("Prefabs/Particle Prefabs/Deathburst"), marform.position, Quaternion.identity), 4);
		
		//if (die != null) die();
		
		Invoke("Respawn", 4f);	// Respawns in a few seconds.
	}
	
	// Control Functions - Functions that allow the player to manipulate the marble.
	// NOTE: If we want to add controller support, we'll need to add axis-based functions.
	#region Control Functions
	// Move Controls - Hackish input for multiplayer.
	public void MoveControls() {
		if (Input.GetKey(KeyCode.W)) Forward();
		if (Input.GetKey(KeyCode.A)) Left();
		if (Input.GetKey(KeyCode.S)) Backward();
		if (Input.GetKey(KeyCode.D)) Right();
		if (Input.GetKey(KeyCode.Space)) Jump();
		if (Input.GetKey(KeyCode.B)) Brake();
	}

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
		//if (moveFunction != null) {
		//	moveFunction();
			
			// Otherwise, do things normally.
		//} else {
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
		//}
	}
	
	// Jump - Makes the marble actually jump.
	public void Jump() {
		// If there are modified conditions, use them.
		//if (jumpFunction != null) {
		//	jumpFunction();
			
			// Otherwise, use the vanilla conditions. Marble may only jump when on the ground.
		//} else {
			if (grounded && canJump) {
				// Directly set velocity to avoid excessively complicated checks.
				marbody.velocity = new Vector3(marbody.velocity.x, 0, marbody.velocity.z) + (hit.normal * (jumpHeight / 100));
				//if (!ballin.isPlaying) ballin.PlayOneShot(jumpSound); // Keeps duplicating the sound for some reason
			}
		//}
	}
	
	// JumpCooldown - If the marble needs to wait for some reason before jumping again, Invoke this.
	public void JumpCooldown() {
		canJump = true;
	}
	
	// Brake - Slows down and stops the marble.
	public void Brake() {
		
		//if (gm.debug) { // If in debug mode, just freeze the ball.
		//	ForceBrake();
			
		//} else { // Otherwise, gradually slow it like an actual brake.
			if (grounded) marbody.AddForce(new Vector3(-marbody.velocity.x * Time.deltaTime, 0, -marbody.velocity.z * Time.deltaTime) * brakeSpeed, ForceMode.Impulse);
		//}
	}
	
	// ForceBrake - Completely stops the marble from moving.
	public void ForceBrake() {
		marbody.velocity = Vector3.zero;
		marbody.angularVelocity = Vector3.zero;
	}
	
	// Respawn - Respawns the marble to roughly its starting position.
	public void Respawn() {
		//Vector3 respawnOffset = new Vector3(0, 3, 0);
		
		ForceBrake();
		ResetState();

		//Network.Destroy(deathBurst);

		marform.position = spawnPoint;//respawnOffset;
		//cam.position = new Vector3(marform.position.x, 0, marform.position.z).normalized * (marform.position.magnitude + 10);
		
		/*if (gm.checkpoint) {
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
		}*/
		
		//if (respawn != null) respawn();
		
	}
	
	// ResetState - Clears marble's conditions. 
	public void ResetState() {
		marbody.isKinematic = false;
		marbody.constraints = RigidbodyConstraints.None;
		GetComponent<MeshRenderer>().enabled = true;
		ballCol.enabled = true;
		//ClearAllBuffs();
	}

	//[RPC]
	//public void UpdateQuantity(int i) {
		//quantity += i;
		//Debug.Log("Quantity: " + quantity);
	//}
	
	#endregion
	
	// OnCollisionEnter - Called when the marble bumps into anything.
	void OnCollisionEnter(Collision col) {
		ballin[1].PlayOneShot(landSound, col.relativeVelocity.sqrMagnitude / 10000);

		// Amplifies marble collisions.
		if (col.collider.GetComponent<MultiplayerMarble>() != null) {
			marbody.AddExplosionForce(col.relativeVelocity.sqrMagnitude, col.transform.position, 5);
			col.rigidbody.AddExplosionForce(col.relativeVelocity.sqrMagnitude, col.transform.position, 5);
		}
	}

	// OnDestroy - Called when marble is destroyed.
	void OnDestroy() {
		//quantity--;
		//netView.RPC("UpdateQuantity", RPCMode.Server, -1);
		Destroy(transform.parent.gameObject);
	}
}
