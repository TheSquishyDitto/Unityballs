using UnityEngine;
using System.Collections;

public class HauntingEntity : Ghostable {

	public Transform destination;	// Where the object should move to.

	SphereCollider col;			// Reference to collider.
	Component[] appearances;	// Array of all renderer components of this object.
	Transform myTransform;		// Cached transform.
	bool visible;				// Whether object's renderers as a whole are enabled or not.
	bool shouldTeleport;		// Whether object should teleport.

	// Start - Use this for initialization.
	protected override void Start () {
		myTransform = transform;
		appearances = GetComponentsInChildren<Renderer>();

		foreach (Renderer appearance in appearances)
			appearance.enabled = physical;

		visible = physical;

		marble = gm.marble;
	}
	
	// Update - Called once per frame.
	protected override void Update () {
		// If visible and distant enough away...
		if (!((Renderer)appearances[0]).isVisible && Vector3.Distance(myTransform.position, destination.position) > 0) {
			myTransform.position = new Vector3(myTransform.position.x, destination.position.y + 2, myTransform.position.z); // Stay on player's y-level.
			myTransform.position += (destination.position - myTransform.position).normalized * Time.deltaTime * (gm.timer / 60); // Move towards player, quickly as time goes on.
			myTransform.LookAt(gm.cam); // Face the camera.
			myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, 0, myTransform.eulerAngles.z); // Try to keep y-axis unrotated (doesn't work?)
		}
	}

	// OnTriggerEnter - Called when object enters trigger radius.
	void OnTriggerEnter() {
		shouldTeleport = true;

		// If entity can be seen, fade away.
		if (visible) 
			StartCoroutine("FadeOut");

		// Otherwise, do something spooky.
		else {
			Teleport();
			int effect = Random.Range(0, 2);
			switch (effect) {
			case 0: AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/SpookyWind"), gm.cam.position); break;
			case 1: marble.marbody.AddForce(Random.insideUnitSphere.normalized * 2000); break;
			default: break;
			}
		}
	}

	// GhostMode - Makes physical walls passable, and spiritual walls solid (only to the marble).
	protected override void GhostMode() {
		StopAllCoroutines();
		if (!physical) StartCoroutine("FadeIn");
	}
	
	// NormalMode - The opposite of GhostMode.
	protected override void NormalMode() {
		StopAllCoroutines();
		if (!physical && visible) {
			StartCoroutine("FadeOut");
			visible = false;
		}
	}

	// FadeIn - Makes the object slowly fade in to existence.
	protected override IEnumerator FadeIn() {
		visible = true;
		foreach (Renderer appearance in appearances)
			appearance.enabled = true;

		for (int i = 1; i <= 50; i++) {
			foreach (Renderer appearance in appearances)
				appearance.material.SetColor("_Color", Color.Lerp(Color.clear, Color.white, i/50.0f));
			yield return new WaitForEndOfFrame();
		}
	}

	// FadeOut - Makes the object slowly fade out of existence.
	protected override IEnumerator FadeOut() {
		for (int i = 1; i <= 50; i++) {
			foreach (Renderer appearance in appearances)
				appearance.material.SetColor("_Color", Color.Lerp(Color.white, Color.clear, i/50.0f));
			yield return new WaitForEndOfFrame();
		}

		foreach (Renderer appearance in appearances)
			appearance.enabled = false;

		if (shouldTeleport) Teleport();
	}

	// Teleport - Move to another location.
	void Teleport() {
		shouldTeleport = false;

		myTransform.position = new Vector3(Random.Range(-100, 100), 2, Random.Range(-100, 100)) + destination.position;

		if (visible) StartCoroutine("FadeIn");
	}
}
