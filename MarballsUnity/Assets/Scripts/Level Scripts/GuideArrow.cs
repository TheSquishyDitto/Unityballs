using UnityEngine;
using System.Collections;

public class GuideArrow : MonoBehaviour {

	GameMaster gm;	// Reference to the Game Master.

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization.
	void Start () {
	
	}
	
	// Update - Called once per frame.
	void Update () {
		GetComponent<MeshRenderer>().enabled = (gm.guides && gm.finishLine);	// Lazy way of dealing with it at the moment.
	}

	// LateUpdate - Called after update.
	void LateUpdate() {
		if (gm.finishLine) {
			transform.rotation = Quaternion.LookRotation(-gm.finishLine.position + transform.position);
		}
	}
}
