using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
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
			for (int i = 0; i < objects.Length; i++) {
				objects[i].SetActive(false);
			}
		}
	}
	
	// Update - Called once per frame.
	void Update () {
	
	}

	// OnTriggerEnter - Called when object enters trigger, even if this is disabled.
	void OnTriggerEnter () {
		for (int i = 0; i < objects.Length; i++) {
			if (!objects[i].activeSelf) {
				objects[i].SetActive(true);
				//Debug.Log("Activated! " + Time.time);
			}
		}
	}

	// OnTriggerExit - Called when object leaves trigger, even if this is disabled.
	void OnTriggerExit() {
		if (disableOnExit) {
			for (int i = 0; i < objects.Length; i++) {
				if (objects[i].activeSelf) {
					objects[i].SetActive(false);
					//Debug.Log("Deactivated! " + Time.time);
				}
			}
		}
	}
}
