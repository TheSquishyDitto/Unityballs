/// <summary>
/// ArrowCam.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Mar. 23, 2015
/// Last Revision: Mar. 23, 2015
/// 
/// Class that controls the camera that can see the finish line indicator.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class ArrowCam : MonoBehaviour {

	GameMaster gm;	// Reference to Game Master

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();	// Reference to Game Master.
	}
	
	// Update - Called once per frame.
	void Update () {
		// Mirrors main camera.
		if (gm.cam) {
			transform.rotation = gm.cam.rotation;
			transform.position = gm.cam.position;
		}
	}
}
