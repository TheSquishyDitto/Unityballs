/// <summary>
/// Ghostable.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  6, 2015
/// 
/// Class for objects that should be passable with the ghost powerup.
///
/// TO DO: - Tweak behavior until desired.
/// 	   - Currently, solid objects other than the marble can touch non-physical entities.
/// 	   - Make spirit walls fade in and out rather than blipping in and out.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class Ghostable : MonoBehaviour {

	GameMaster gm;		// Reference to the GameMaster.
	Marble marble;		// Reference to any marble that happens to enter the collider.
	Collider ghostWall;	// Reference to the collider that will be toggled.
	Renderer appearance;// Reference to the renderer on this object.

	public bool physical = true;	// Whether this object is solid normally or not.


	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization
	void Start () {
		ghostWall = GetComponent<Collider>();
		appearance = GetComponent<Renderer>();

		appearance.enabled = physical;
		marble = gm.marble;

		Physics.IgnoreCollision(ghostWall, gm.marble.GetComponent<Collider>(), !physical);
		gameObject.layer = (!physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");

		// Tells the static GhostSource that this particular wall needs to be toggled on or off when the player ghosts.
		GhostSource.Ghosting += GhostMode;
		GhostSource.Unghosting += NormalMode;
	}
	
	// Update - Called once per frame.
	void Update () {
	
	}

	// OnCollisionEnter - Currently can be used to prevent spirit walls from being touched by physical objects.
	void OnCollisionEnter(Collision collision) {
		//if (!physical && collision.collider.GetComponent<Marble>().buff != Marble.PowerUp.Ghost)
		//	Physics.IgnoreCollision(ghostWall, collision.collider);
	}

	// OnDestroy - Tells the static GhostSource that this wall will no longer exist.
	void OnDestroy() {
		GhostSource.Ghosting -= GhostMode;
		GhostSource.Unghosting -= NormalMode;
	}

	// GhostMode - Makes physical walls passable, and spiritual walls solid (only to the marble).
	protected void GhostMode() {
		Physics.IgnoreCollision(ghostWall, marble.GetComponent<Collider>(), physical);
		if (!physical) appearance.enabled = true;
		gameObject.layer = (physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");

	}

	// NormalMode - The opposite of GhostMode.
	protected void NormalMode() {

		Physics.IgnoreCollision(ghostWall, marble.GetComponent<Collider>(), !physical);
		if (!physical) appearance.enabled = false;
		gameObject.layer = (!physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
	}
}
