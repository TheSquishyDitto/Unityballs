/// <summary>
/// BuffSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Apr. 16, 2015
/// 
/// General class for granting/clearing buffs via trigger.
/// 
/// NOTES: - Any buff source classes that inherit from this should have their own buffing functions.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BuffSource : MonoBehaviour {

	#region Variables
	protected GameMaster gm;		// Reference to the Game Master.
	protected Marble marble;		// Reference to the marble being modified.

	//public Marble.Powerup buff;		// Which buff this source gives.
	public float intensity;				// How strong the buff is. Acceptable values vary by type.
	public float duration;				// How long the given buff should last.
	public Sprite icon;					// What icon should be displayed for this buff.
	public Color iconTint = Color.white;// What color the icon should be tinted when active.
	public Transform badge;				// The badge levitating in this powerup.
	public float rotationSpeed = 100;	// How fast badge should rotate.
	public GameObject particles;		// What type of particle system this buff should give.
	public bool collectable;			// Whether this source disappears when collected. Respawns when used.
	public float respawnTime = 0;		// How long after using this buff it should respawn. Mainly for jump-based buffs.

	#endregion

	// Initialize - Any initialization the given source should have should be done here.
	protected virtual void Initialize() { }

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Use this for initialization
	void Start () {
		//marble = gm.marble;
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (badge) badge.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only grants buffs to marbles.
			marble = other.GetComponent<Marble>();

			if (marble.heldBuff == Marble.PowerUp.None) {
				GiveBuff(marble);	// Gives the buff to the marble.
				if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
			}
		}	
	}

	// GiveBuff - Gives a specific buff to the specified marble.
	protected virtual void GiveBuff(Marble marble) {

		// Adds icon to holding GUI box.
		if (gm.hud) {
			gm.hud.heldPowerup.sprite = icon;
			gm.hud.heldPowerup.color = iconTint;
		}

		// Passes information about the buff to the marble.
		marble.heldParticles = particles;
		marble.heldIntensity = intensity;
		marble.heldDuration = duration;
		marble.heldCleaner = TakeBuff;
		AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/swordClash"), gm.marble.transform.position);
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected virtual void TakeBuff() {
		Invoke("Respawn", respawnTime);	// If buff source is collected, respawns it after the time has passed.
	}

	// Respawn - Regenerates buff.
	protected virtual void Respawn() {
		gameObject.SetActive(true);
	}
}
