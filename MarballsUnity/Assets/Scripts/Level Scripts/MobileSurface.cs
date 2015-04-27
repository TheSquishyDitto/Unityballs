/// <summary>
/// MobileSurface.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 26, 2015
/// Last Revision: Apr. 26, 2015
/// 
/// Class that allows objects resting on a moving object to move with the object.
/// 
/// NOTES: - Momentum is not preserved if the trigger is left.
/// 
/// TO DO: - Tweak until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MobileSurface : MonoBehaviour {

	Transform myTransform;					// Cached transform reference.
	Vector3 lastPos = Vector3.zero;			// Last position of surface.
	public Vector3 deltaPos = Vector3.zero;	// Change in position since last checked.

	// Use this for initialization
	void Start () {
		myTransform = transform;
	}

	// FixedUpdate - Updates delta position every physics frame.
	void FixedUpdate() {
		deltaPos = myTransform.position - lastPos;
		lastPos = myTransform.position;
	}

	// OnTriggerStay - Applies position change to anything resting on the platform.
	void OnTriggerStay(Collider other) {
		other.transform.position += deltaPos;
	}
}
