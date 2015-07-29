/// <summary>
/// Marble.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Jan. 28, 2015
/// Last Revision: July 28, 2015
/// 
/// Class that controls marble game logic.
/// 
/// NOTES: - This is the base class for all marbles and should be designed with inheritance in mind.
/// 	   - BuffSlot class is found at bottom of this one.
/// 
/// TO DO: - Fix sounds: marble doesn't play sound if not technically grounded, even if rolling along.
/// 	   - Modify buff usage functions to allow the future possibility of extra held buffs.
/// 	   - Class should probably be split further. MarbleBuff class?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class Marble : MonoBehaviour, IDamageable, IKillable {
	
	// Variables
	#region Variables
	[Header("References")]
	GameMaster gm;						// Reference to the Game Master.
	Settings settings;					// Reference to the game settings.
	
	public MarbleData data;				// Reference to data holding typical values of this marble form.
	public Transform cam;				// Reference to the main camera.
	public Transform marform;			// Reference to the marble's transform.
	public Rigidbody marbody;			// Reference to the marble's rigidbody.
	protected SphereCollider ballCol;	// Reference to the marble's collider.
	public MarbleMover mover;			// Reference to marble's movement controller.
	public AudioSource[] ballin;		// Reference to the marble's rolling sound.
	public Transform spawnPoint;		// Reference to location where marble should respawn.
	public GameObject deathParticle;	// Reference to particle that shows up when player dies.
	public GameObject hudPrefab;		// Reference to prefab for the heads up display.
	public HitBox hitbox;				// Reference to marble's damaging component.
	
	[Header("Stats")]
	public int level = 1;				// Possible RPG element? Leveling up would allow increasing a stat.
	public int exp = 0;					// XP - Progression towards next level. 100 means levelup!
	public int health = 10;				// HP - How much damage the marble can take before critical existence failure.
	public int defense = 0;				// 		Mitigates damage taken.
	public int marblePower = 5; 		// MP - Resource used to perform special abilities. 
	public int charmCapacity = 3;		// CC - Resource held by equipped charms. Can be increased by 3 per level.

	public int maxHP = 10;				// The maximum amount of HP the marble can have. Can be increased by 5 per level.
	public int maxMP = 5;				// The maximum amount of MP the marble can have. Can be increased by 5 per level.
	public int maxCC = 3;				// The maximum amount of CP the marble can have. Can be increased by 3 per level.

	public float damageThreshold = 20;	// Minimum velocity to begin inflicting damage.
	public float damageInterval = 15;	// How quickly damage stacks at velocities higher than the threshold.

	[Header("Buff Slots")]
	public TimeEvent buffTimer;				// Stopwatch tracking active buff duration.
	public int maxHeldBuffs = 1;			// How many buffs the marble can hold on backup.
	public List<BuffSlot> buffs = new List<BuffSlot>();	// List of buffs the player has collected/activated.

	[Header("Charms & Abilities")]
	public List<Charm> charms = new List<Charm>();			// List of owned charms.
	public List<Ability> abilities = new List<Ability>();	// List of abilities granted by charms.
	public int abilityIndex = 0;							// Which ability is currently selected.
	public float timeStamp = 0;								// Used for ability cooldown.

	[Header("Sounds")]
	public AudioClip deathSound;		// Sound the marble makes upon death.
	public AudioClip rollingSound;		// Sound the marble makes when rolling.
	public AudioClip jumpSound;			// Sound the marble makes when jumping.
	public AudioClip landSound;			// Sound the marble makes when hitting anything.

	[Header("Misc. Options")]
	public bool flashLight;				// Whether marble should light up under certain scenarios.
	public bool invulnerable = false;	// Whether marble should take damage when touching a hitbox.

	public static event UnityAction die;	 	// Container for actions when player dies.
	public static event UnityAction respawn; 	// Container for actions when player respawns.
	//public static event UnityAction damaged;	// Container for actions when player is damaged.
	
	#endregion

	// Monobehaviour Functions - Typical Unity-provided functions.
	#region Monobehaviour Functions
	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();	// Refers to Game Master, see GameMaster code for details.
		gm.marble = this;	// Tells the Game Master that this is the currently controlled marble.
		settings = GameMaster.LoadSettings();

		Instantiate(hudPrefab).transform.SetParent(GameObject.FindGameObjectWithTag("GUI").transform);

		marform = transform;
		marbody = GetComponent<Rigidbody>();
		mover = GetComponent<MarbleMover>();
		ballCol = GetComponent<SphereCollider>();
		ballin = GetComponents<AudioSource>();

		// DEBUG - Testing charms. The order in which they are added is the order they are displayed.
		//charms.Add(new Charm());
		charms.Add(new DashCharm());
		charms.Add(new DefenseCharm());
		charms.Add(new MPRegenCharm());
		charms.Add(new BombCharm());
		charms.Add(new HealthCharm());
		charms.Add(new HealthCharm());
		charms.Add(new HealthCharm());
	}

	// OnEnable - Called when the marble is activated. Used to subscribe to events.
	void OnEnable() {
		Messenger.AddListener("UseBuff", UseBuff);
		Messenger.AddListener("UseAbility", UseAbility);
		Messenger.AddListener("ScrollLeft", ScrollLeft);
		Messenger.AddListener("ScrollRight", ScrollRight);
	}

	// OnDisable - Called when the marble is deactivated. Used to unsubscribe from events.
	// NOTE: Anything subscribed to in OnEnable should be unsubscribed from here to prevent memory leaks.
	void OnDisable() {
		Messenger.RemoveListener("UseBuff", UseBuff);
		Messenger.RemoveListener("UseAbility", UseAbility);
		Messenger.RemoveListener("ScrollLeft", ScrollLeft);
		Messenger.RemoveListener("ScrollRight", ScrollRight);
	}

	// Start - Use this for initialization. If a reference from the Game Master is needed, make it here.
	void Start () {
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		ResetState();
	}
	
	// Update - Called once per frame.
	void Update () {

		// If useOnGrab is active, automatically use powerups when grabbed.
		// NOTE: There's probably a better way to handle this than checking every frame.
		if (settings.useOnGrab && (buffs[1] != null && buffs[1].buff != BuffSource.PowerUp.None))
			UseBuff();

		// Updates marble's own hitbox's damage is based on how fast the marble is moving. More speed, more damage.
		hitbox.damage = (int)Mathf.Max((marbody.velocity.magnitude - damageThreshold) / damageInterval, 0);

	}

	// FixedUpdate - Called once per physics calculation. This happens independently of frames.
	void FixedUpdate () {
		// Behavior is dependent on whether marble is in the air or on the ground.
		if (mover) {
			if (Grounded) {
				if (flashLight) gameObject.GetComponent<Light>().enabled = true;	// Marble may light up when on the ground.
				if (!ballin[0].isPlaying) ballin[0].Play();
				ballin[0].volume = marbody.velocity.magnitude/60f * settings.FXScaler;
				//ballin[0].pitch = (Mathf.Sin(Time.time / 4) / 4f) + 1f;
			} else {			
				if (flashLight) gameObject.GetComponent<Light>().enabled = false;	// Marble's light turns off if it was on.
				//ballin.enabled = false;
				ballin[0].Stop();
				//ballin[0].volume = Mathf.MoveTowards(ballin[0].volume, 0, 0.1f);
			}
		}
	}

	// OnCollisionEnter - Called when the marble bumps into anything.
	void OnCollisionEnter(Collision col) {
		ballin[1].PlayOneShot(landSound, (col.relativeVelocity.sqrMagnitude / 10000) * settings.FXScaler);
	}

	#endregion

	// PowerUp & Ability Functions - Functions that allow the marble to use buffs and abilities.
	#region PowerUp and Ability Functions

	// UseAbility - Allows the marble to use its active ability, if any.
	void UseAbility() {
		abilityIndex = (abilityIndex >= abilities.Count)? abilities.Count - 1 : abilityIndex;

		// Checks if cooldown has finished.
		if (timeStamp <= Time.time && abilityIndex >= 0) {
			// If the ability's cost can be afforded,
			if (abilities[abilityIndex].cost <= marblePower) {
				// Uses it and starts cooldown.
				abilities[abilityIndex].ability();
				marblePower -= abilities[abilityIndex].cost;
				timeStamp = Time.time + abilities[abilityIndex].cooldown;
				Messenger<int, int>.Broadcast("UpdateMarpower", marblePower, maxMP);
			} else {
				// If it can't be afforded, complain mildly.
				Debug.Log("Not enough MP to use " + abilities[abilityIndex].type);
				Messenger<int, int>.Broadcast("UpdateMarpower", marblePower, maxMP); // Blink for emphasis
			}
		} else {
			// If still cooling down, complain in some way.
			Debug.Log("COOLING DOWN: Wait " + (timeStamp - Time.time) + " more seconds.");
			// play some sort of sound here?
		}
	}

	// ScrollLeft - Moves ability index to the "left".
	void ScrollLeft() {
		if (abilities.Count > 0 && timeStamp < Time.time) {
			abilityIndex = (abilityIndex - 1 >= 0)? abilityIndex - 1 : abilities.Count - 1;
			Messenger.Broadcast("UpdateAbility");
		}
	}

	// ScrollRight - Moves ability index to the "right".
	void ScrollRight() {
		if (abilities.Count > 0 && timeStamp < Time.time) {
			abilityIndex = (abilityIndex + 1) % abilities.Count;
			Messenger.Broadcast("UpdateAbility");
		}
	}

	// ClearBuffs - Returns marble to its default state. Does not clear held buffs.
	public void ClearBuffs() {

		if (buffs[0] != null && buffs[0].buff != BuffSource.PowerUp.None) {
			if (buffs[0].cleaner != null) {
				buffs[0].cleaner();
			}

			if (buffs[0].particles != null) {
				GameObject.Destroy(buffs[0].particles);
			}

			buffs[0] = null;
		}

		if (buffTimer != null && buffTimer.routine != null) {
			TimeManager.CreateTimer().StopCoroutine(buffTimer.routine);
			buffTimer = null;
		}

		Messenger<bool>.Broadcast("ShowActiveBuff", false);
	}

	// ClearAllBuffs - Does the same as ClearBuffs, but clears the held buff as well.
	// NOTE: Current implementation will cause the buff function to be triggered first.
	public void ClearAllBuffs() {
		UseBuff();
		ClearBuffs();
	}
	
	// UseBuff - Applies the marble's currently held powerup. Currently overwrites any existing one.
	public void UseBuff() {

		if (buffs.Count > 1/*buffs[1] != null && buffs[1].buff != BuffSource.PowerUp.None*/) {
			ClearBuffs();

			// Moves held buff into active buff slot.
			buffs[0] = buffs[1];
			buffs.RemoveAt(1);
			//buffs[1] = null;

			// Activates the buff.
			buffs[0].heldActive = "Active: " + buffs[0].buff.ToString();
			if (buffs[0].particles != null) buffs[0].particles = Instantiate(buffs[0].particles);
			if (buffs[0].buffFunction != null) buffs[0].buffFunction();
			buffTimer = new TimeEvent(buffs[0].duration, ClearBuffs);

			TimeManager.CreateTimer().StartStopwatch(buffTimer);
			Messenger<bool>.Broadcast("ShowActiveBuff", true);
		}
	}

	#endregion

	// Death and Respawning Functions
	#region Death and Respawning Functions
	// TakeDamage - Called when player should take damage.
	public void TakeDamage(int damage) {
		// Damage can only be taken during gameplay, and not constantly.
		if (!invulnerable/* && gm.state == GameMaster.GameState.Playing*/) {
			
			// If the damage done is greater than 0, do something.
			if (damage - defense > 0) {
				Messenger<float>.Broadcast("CameraShake", (damage - defense) * 0.1f);
				health = Mathf.Max(health - (damage - defense), 0);
				Messenger<int,int>.Broadcast("UpdateHealth", health, maxHP);
				
				// Marble dies if out of health.
				if (health <= 0)
					Die();
				else {
					// Make marble invulnerable shortly to prevent continuous damage.
					GetComponent<Renderer>().material.color = Color.red;
					invulnerable = true;
					Invoke("RevokeArmor", 0.5f);
				}
			}
		}
	}
	
	// RevokeArmor - Removes invulnerability.
	public void RevokeArmor() {
		invulnerable = false;
		GetComponent<Renderer>().material.color = Color.white;
	}
	
	// Die - Called when player should die.
	public void Die() {
		//if (gm.state == GameMaster.GameState.Playing) {
			AudioSource.PlayClipAtPoint(deathSound, marform.position, settings.FXScaler);
			ClearBuffs();
			marbody.isKinematic = true;
			GetComponent<MeshRenderer>().enabled = false;
			ballCol.enabled = false;
			GameObject burst = (GameObject)(Instantiate(deathParticle, marform.position, Quaternion.identity));
			burst.transform.position = marform.position;
			Destroy(burst, 2);
			
			if (die != null) die();
			
			Invoke("Respawn", 4f);	// Respawns in a few seconds.
		//}
	}

	// Respawn - Respawns the marble to roughly its starting position.
	public void Respawn() {
		Vector3 respawnOffset = new Vector3(0, 3, 0);

		if (mover) mover.ForceBrake();
		ResetState();

		if (spawnPoint) {
			marform.position = spawnPoint.position + respawnOffset;

			// Rotates camera to match respawn point's rotation.
			ICamera camScript = cam.GetComponent<ICamera>();
			camScript.ResetPosition();	// Makes sure camera is in right location.
			camScript.Freeze(true);		// Prevents it from updating while changes are made.

			// Rotates it to face the direction of the respawn point.
			cam.RotateAround(marform.position, Vector3.up, -Vector3.Angle(spawnPoint.forward, 
			                                                             Vector3.Scale(cam.forward, new Vector3(1, 0, 1))));

			camScript.RecalculatePosition();	// Forces it to update to new position.
			camScript.Freeze(false);			// Unlocks.

		} else {
			Debug.LogWarning("(Marble.cs) No spawn point available! Placing in default location..."); // DEBUG
			marform.position = respawnOffset;
		}

		if (respawn != null) respawn();

	}

	// ResetState - Returns marble to normal conditions.
	public void ResetState() {
		ResetStats();
		marbody.isKinematic = false;
		marbody.constraints = RigidbodyConstraints.None;
		GetComponent<MeshRenderer>().enabled = true;
		ballCol.enabled = true;
		ClearAllBuffs();
	}

	// ResetStats - Restores marble's stats to data defaults.
	public void ResetStats() {
		health = maxHP;
		marblePower = maxMP;

		Messenger<int,int>.Broadcast("UpdateHealth", health, maxHP);
		Messenger<int,int>.Broadcast("UpdateMarpower", marblePower, maxMP);

		//defense = data.defense;
		marform.localScale = Vector3.one * data.size;
		marbody.maxAngularVelocity = data.maxAngVelocity;
		SpeedMultiplier = data.speedMultiplier;
		RevSpeed = data.revSpeed;
		Brake = data.brakeSpeed;
		Shackle = data.shackle;
		JumpHeight = data.jumpHeight;
	}

	#endregion

	// Getters/Setters - If something is requesting marble internals, use these instead.
	#region Getters/Setters

	// Stat Properties
	public int HP {
		get { return health; }
		set { health = value; }
	}

	public int MaxHP {
		get { return maxHP; }
		set { maxHP = value; }
	}

	public int MP {
		get { return marblePower; }
		set { marblePower = value; }
	}
	
	public int MaxMP {
		get { return maxMP; }
		set { maxMP = value; }
	}

	public int CC {
		get { return charmCapacity; }
		set { charmCapacity = value; }
	}
	
	public int MaxCC {
		get { return maxCC; }
		set { maxCC = value; }
	}

	public int XP {
		get { return exp; }
		set { exp = value; }
	}

	public int Level {
		get { return level; }
		set { level = value; }
	}

	// Buff Properties
	public float BuffTimer {
		get { return buffTimer.duration; }
		set { buffTimer.duration = value; }
	}

	// Movement Properties
	public float Speed {
		get { return marbody.velocity.magnitude; }
	}
	
	public float SpeedMultiplier {
		get { return mover.speedMultiplier; }
		set { mover.speedMultiplier = value;}
	}

	public float RPS {
		get { return marbody.angularVelocity.magnitude; }
	}

	public float RevSpeed {
		get { return mover.revSpeed; }
		set { mover.revSpeed = value; }
	}

	public float Shackle {
		get { return mover.shackle; }
		set { mover.shackle = value; }
	}

	public float Brake {
		get { return mover.brakeSpeed; }
		set { mover.brakeSpeed = value; }
	}

	public Vector3 Direction {
		get { return mover.inputDirection; }
	}

	public UnityAction MoveFunction {
		get { return mover.moveFunction; }
		set { mover.moveFunction = value; }
	}

	// Jumping Properties
	public float JumpHeight {
		get { return mover.jumpHeight; }
		set { mover.jumpHeight = value; }
	}

	public int MidairJumps {
		get { return mover.midairJumps; }
		set { mover.midairJumps = value; }
	}

	public bool CanJump {
		get { return mover.canJump; }
		set { mover.canJump = value; }
	}

	public bool Grounded {
		get { return mover.grounded; }
	}

	public RaycastHit GroundInfo {
		get { return mover.hit; }
	}

	public UnityAction JumpFunction {
		get { return mover.jumpFunction; }
		set { mover.jumpFunction = value; }
	}

	#endregion

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

[System.Serializable]
// Class used for easily managing collected and active powerups.
public class BuffSlot {
	public string heldActive = "Empty";	// Whether this buff is in the active or held slot. Exclusively used for the editor.
	public BuffSource.PowerUp buff;		// The ID/name of the powerup.
	public float intensity;				// How strong the powerup is. Marble may not need to know about this.
	public float duration;				// How much time remains on this powerup.
	public GameObject particles;		// Particles or other object that spawns when buff is active.
	public UnityAction cleaner;			// Function that removes buff from marble.
	public UnityAction buffFunction;	// Function that adds buff to marble.
	// icons and stuff?
	
	public BuffSlot(BuffSource.PowerUp buff, float intensity, float duration, GameObject particles, UnityAction cleaner, UnityAction buffFunction) {
		heldActive = "Held: " + buff.ToString();
		this.buff = buff;
		this.intensity = intensity;
		this.duration = duration;
		this.particles = particles;
		this.cleaner = cleaner;
		this.buffFunction = buffFunction;
	}
}
