/// <summary>
/// BuffSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 24, 2015
/// 
/// General class for granting/clearing buffs via trigger.
/// 
/// NOTES: - Any buff source classes that inherit from this should have their own buffing functions.
/// 	   - Currently always overwrites the marble's currently held buff.
///        - If there was a nice one or two line way to convert the enum into a method/function name, having separate classes would be mostly unnecessary.
/// 
/// TO DO: - Investigate ways to make the inheritance unnecessary.
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
	public GameObject particles;		// What type of particle system this buff should give.
	public bool collectable;			// Whether this source disappears when collected.

	#endregion

	// Initialize - Any initialization the given source should have should be done here.
	protected virtual void Initialize() { }

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Use this for initialization
	void Start () {
		marble = gm.marble;
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {	// Only grants buffs to marbles.
			Marble marble = other.GetComponent<Marble>();

			GiveBuff(marble);	// Gives the buff to the marble.

			if (collectable) gameObject.SetActive(false);	// Disappears if collectable.
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
	}
}
