﻿/// <summary>
/// OverviewCam.cs
/// Authors: Kyle Dawson, Chris Viqueira
/// Date Created:  Apr. 22, 2015
/// Last Revision: Apr. 22, 2015
/// 
/// Class that controls overview cam adjustments
/// 
/// TO DO: TBD
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class OverviewCam : MonoBehaviour {

	GameMaster gm; 			// Reference to the game master.
	Transform myTransform;	// Reference to object's own transform.
	Transform marble;		// Reference to marble
	Vector3 startOffset;	// Initial distance from marble.

	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {
		myTransform = transform;
		marble = gm.marble.marform;
		startOffset = myTransform.position - marble.position;
	}
	
	// Update is called once per frame
	void Update () {

		// Checks if marble can "see" the camera currently.
		Debug.DrawRay(marble.position, myTransform.position - marble.position, Color.red); // DEBUG
		RaycastHit hit;

		// If marble cannot "see" the camera, moves the camera to a point on the radius that it CAN be seen.
		if (Physics.Raycast (marble.position, (myTransform.position - marble.position).normalized, out hit, 100)) 
			myTransform.position = hit.point - (myTransform.position - marble.position).normalized;
		else {
			myTransform.position = marble.position + startOffset; // default value in editor
		}

	}
}
