using UnityEngine;
using System.Collections;

public class GuideArrow : MonoBehaviour {
	
	Transform myTransform;	// Cached reference to transform.

	// Awake - Called before anything else.
	void Awake() {
		myTransform = transform;
	}

	// LateUpdate - Called after update.
	void LateUpdate() {
		if (FinishLine.finish != null) {
			myTransform.rotation = Quaternion.LookRotation(-FinishLine.finish.position + myTransform.position);
		}
	}
}
