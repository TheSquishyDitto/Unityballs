/// <summary>
/// ProximityEnable.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 15, 2015
/// Last Revision: Apr. 15, 2015
/// 
/// Class for areas where objects should be disabled or enabled.
///
/// NOTES: - Strange behavior in cases of overlap.
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(BoxCollider))]
public class ProximityEnable : MonoBehaviour {

	public GameObject[] objects;		// Array of objects to be enabled/disabled.
	public bool startDisabled = true;	// Whether objects should be disabled at beginning of scene.
	public bool disableOnExit = true;	// Whether objects should be disabled after leaving.
	Collider proximityZone;				// Reference to collider that (de)activates object.

	// Start - Use this for initialization.
	void Start () {
		proximityZone = GetComponent<Collider>();
		proximityZone.isTrigger = true;

		if (startDisabled) {
			IterateActive(false);
		}
	}

	// OnTriggerEnter - Called when object enters trigger, even if this is disabled.
	void OnTriggerEnter () {
		IterateActive(true);
	}

	// OnTriggerExit - Called when object leaves trigger, even if this is disabled.
	void OnTriggerExit() {
		if (disableOnExit) {
			IterateActive(false);
		}
	}

	// IterateActive - Enables/disables all objects in the array.
	void IterateActive(bool active) {
		for (int i = 0; i < objects.Length; i++) {
			objects[i].SetActive(active);
		}
	}
}
