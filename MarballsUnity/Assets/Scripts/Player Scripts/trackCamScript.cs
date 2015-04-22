/// <summary>
/// CameraController.cs
/// Authors: Kyle Dawson, Chris Viqueira
/// Date Created:  Apr. 24, 2015
/// Last Revision: Apr. 24, 2015
/// 
/// Class that controls overview cam adjustments
/// 
/// 
/// TO DO: TBD
/// 
/// </summary>
/// 
using UnityEngine;
using System.Collections;

public class trackCamScript : MonoBehaviour {

	public GameMaster gm; 	// Reference to the game master.
	Transform marble;		// Reference to marble

	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {
		marble = gm.marble.marform;
	}
	
	// Update is called once per frame
	void Update () {

		// Checks if marble can "see" the camera currently.
		Debug.DrawRay(marble.position, transform.position - marble.position, Color.blue); // DEBUG
		RaycastHit hit;

		// If marble cannot "see" the camera, moves the camera to a point on the radius that it CAN be seen.
		if (Physics.Raycast (marble.position, (transform.position - marble.position).normalized, out hit, 20)) 
			transform.position = hit.point - (transform.position - marble.position).normalized;
		else {
			transform.position = marble.position + new Vector3 (0, 20, 0); // default value in editor
		}

	}
}
