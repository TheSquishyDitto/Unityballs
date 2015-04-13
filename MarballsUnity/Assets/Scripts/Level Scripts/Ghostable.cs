/// <summary>
/// Ghostable.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr. 12, 2015
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

	protected GameMaster gm;		// Reference to the GameMaster.
	protected Marble marble;		// Reference to any marble that happens to enter the collider.
	protected Collider ghostWall;	// Reference to the collider that will be toggled.
	Renderer appearance;			// Reference to the renderer on this object.
	Color originalColor;			// Original color of shader material.

	public bool physical = true;	// Whether this object is solid normally or not.


	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// OnEnable - Called when object wakes up.
	void OnEnable() {
		// Tells the static GhostSource that this particular wall needs to be toggled on or off when the player ghosts.
		// In other words, subscribes the wall to the ghosting event.
		GhostSource.Ghosting += GhostMode;
		GhostSource.Unghosting += NormalMode;
	}

	// Start - Use this for initialization
	protected virtual void Start () {
		ghostWall = GetComponent<Collider>();
		appearance = GetComponent<Renderer>();
		if (!physical) {
			originalColor = appearance.material.GetColor("_TintColor");
			appearance.material.SetColor("_TintColor", Color.clear);
		}

		appearance.enabled = physical;
		marble = gm.marble;

		Physics.IgnoreCollision(ghostWall, gm.marble.GetComponent<Collider>(), !physical);
		gameObject.layer = (!physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");

	}
	
	// Update - Called once per frame.
	protected virtual void Update () {

	}

	// OnCollisionEnter - Currently can be used to prevent spirit walls from being touched by physical objects.
	void OnCollisionEnter(Collision collision) {
		//if (!physical && collision.collider.GetComponent<Marble>().buff != Marble.PowerUp.Ghost)
		//	Physics.IgnoreCollision(ghostWall, collision.collider);
	}

	// OnDisable - Tells the static GhostSource that this wall doesn't need to be bothered with.
	void OnDisable() {
		GhostSource.Ghosting -= GhostMode;
		GhostSource.Unghosting -= NormalMode;
	}

	// GhostMode - Makes physical walls passable, and spiritual walls solid (only to the marble).
	protected virtual void GhostMode() {
		Physics.IgnoreCollision(ghostWall, marble.GetComponent<Collider>(), physical);

		gameObject.layer = (physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");

		StopAllCoroutines();
		if (!physical) StartCoroutine("FadeIn");
	}

	// NormalMode - The opposite of GhostMode.
	protected virtual void NormalMode() {
		Physics.IgnoreCollision(ghostWall, marble.GetComponent<Collider>(), !physical);

		gameObject.layer = (!physical)? LayerMask.NameToLayer("Ignore Raycast") : LayerMask.NameToLayer("Default");
		StopAllCoroutines();
		if (!physical) StartCoroutine("FadeOut");
	}

	// FadeIn - Makes the wall slowly fade in to existence.
	protected virtual IEnumerator FadeIn() {
		appearance.enabled = true;
		Color currentColor = appearance.material.GetColor("_TintColor");
		for (int i = 1; i <= 50; i++) {
			appearance.material.SetColor("_TintColor", Color.Lerp(currentColor, originalColor, i/50.0f));
			yield return new WaitForEndOfFrame();
		}
	}

	// FadeOut - Makes the wall slowly fade out of existence.
	protected virtual IEnumerator FadeOut() {
		for (int i = 1; i <= 50; i++) {
			appearance.material.SetColor("_TintColor", Color.Lerp(originalColor, Color.clear, i/50.0f));
			yield return new WaitForEndOfFrame();
		}
		if (!physical) appearance.enabled = false;
	}
}
