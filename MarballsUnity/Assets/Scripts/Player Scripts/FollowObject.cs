/// <summary>
/// FollowObject.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 18, 2015
/// Last Revision: Apr. 18, 2015
/// 
/// Class that simply follows a specified object.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {
	
	protected Transform myTransform;	// Reference to own transform for caching.
	public Transform trackingObject;	// Reference to transform of the position to be tracked.

	// Start - Use this for initialization
	protected virtual void Start () {
		myTransform = transform;
	}
	
	// Update - Called once per frame.
	protected virtual void Update () {
		if (trackingObject != null)
			myTransform.position = trackingObject.position;
	}
}
