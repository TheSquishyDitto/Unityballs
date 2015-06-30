/// <summary>
/// ArrowCam.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Mar. 23, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Class that controls the camera that can see the finish line indicator.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class ArrowCam : MonoBehaviour {

	//GameMaster gm;	// Reference to Game Master
	Transform cam;	// Reference to marble camera.

	// Awake - Called before anything else.
	void Awake () {
		//gm = GameMaster.CreateGM();	// Reference to Game Master.
		cam = Camera.main.transform;
	}
	
	// Update - Called once per frame.
	void Update () {
		// Mirrors main camera.
		if (cam) {
			transform.rotation = cam.rotation;
			transform.position = cam.position;
		}
	}
}
