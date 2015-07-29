/// <summary>
/// BuffSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: July 28, 2015
/// 
/// General class for granting/clearing buffs via trigger.
/// 
/// NOTES: - Any buff source classes that inherit from this should have their own buffing functions.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BuffSource : MonoBehaviour {

	// Enum for what powerups exist in this game.
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

	#region Variables
	protected GameMaster gm;		// Reference to the Game Master.
	protected Settings settings;	// Reference to the game settings.
	protected Marble marble;		// Reference to the marble being modified.
	
	public PowerUp buffType;			// Which buff this source gives.
	public float intensity;				// How strong the buff is. Acceptable values vary by type.
	public float duration;				// How long the given buff should last.
	public Sprite icon;					// What icon should be displayed for this buff.
	public Color iconTint = Color.white;// What color the icon should be tinted when active.
	public Transform badge;				// The badge levitating in this powerup.
	public float rotationSpeed = 100;	// How fast badge should rotate.
	public GameObject particles;		// What type of particle system this buff should give.
	public bool collectable;			// Whether this source disappears when collected. Respawns when used.
	public float respawnTime = 0;		// How long after using this buff it should respawn. Mainly for jump-based buffs.
	public AudioClip buffCollect;		// Sound made when a buff is collected.

	protected BuffSlot buffSlot;		// The slot to be passed.

	#endregion

	// Initialize - Any initialization the given source should have should be done here.
	protected virtual void Initialize() { }

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		settings = GameMaster.LoadSettings();
	}

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (badge) badge.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only grants buffs to marbles.
			marble = other.GetComponent<Marble>();

			// If marble still has room to hold on to new buffs...
			if (marble.buffs.Count <= marble.maxHeldBuffs) {
				GiveBuff(marble);	// Gives the buff to the marble.
				if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
			}
		}	
	}

	// GiveBuff - Gives a specific buff to the specified marble.
	protected virtual void GiveBuff(Marble marble) {

		// Adds icon to holding GUI box.
		Messenger<Sprite, Color>.Broadcast("SetHeldSprite", icon, iconTint);

		// Passes information about the buff to the marble.
		buffSlot = new BuffSlot(buffType, intensity, duration, particles, TakeBuff, BuffFunction);
		//marble.buffs[1] = buffSlot;
		marble.buffs.Add(buffSlot);
		AudioSource.PlayClipAtPoint(buffCollect, Vector3.zero, settings.FXScaler);
	}

	// BuffFunction - The function that applies the buff.
	protected virtual void BuffFunction() {
		Debug.LogWarning("(BuffSource.cs) You shouldn't see this.");
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected virtual void TakeBuff() {
		if (collectable)
			Invoke("Respawn", respawnTime);	// If buff source is collected, respawns it after the time has passed.
	}

	// Respawn - Regenerates buff.
	protected virtual void Respawn() {
		gameObject.SetActive(true);
	}
}
