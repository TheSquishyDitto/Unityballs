/// <summary>
/// FollowObject.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 18, 2015
/// Last Revision: Apr. 22, 2015
/// 
/// Class that simply follows a specified object.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
	
	protected Transform myTransform;		// Reference to own transform for caching.
	public Transform trackingObject;		// Reference to transform of the position to be tracked.
	public bool copyRotation = false;		// Whether this object should copy the rotation as well.
	public Vector3 offset = Vector3.zero;	// How much distance there should be between object and follower.

	// Start - Use this for initialization
	protected virtual void Start () {
		myTransform = transform;
	}
	
	// Update - Called once per frame.
	protected virtual void Update () {
		if (trackingObject != null) {
			myTransform.position = trackingObject.position + offset;

			if (copyRotation)
				myTransform.rotation = trackingObject.rotation;
		}
	}
}
