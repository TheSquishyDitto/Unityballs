/// <summary>
/// PanCamera.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Apr. 21, 2015
/// Last Revision: Apr. 30, 2015
/// 
/// Class for Marballs' specific camera panning action.
/// 
/// NOTES: - This is vital for PreStart state.
/// 
/// TO DO: - Tweak until behaves as desired.
/// 	   - Add more options to make path creation nicer.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class PanCamera : ScriptedPath {

	GameMaster gm;	// Reference to GameMaster.
	Camera cam;		// Reference to attached camera component.

	// OnEnable - Called when script is enabled. Used to subscribe to events.
	void OnEnable() {
		GameMaster.pan += Activate;
		GameMaster.start += Deactivate;
	}

	// OnDisable - Called when script is disabled. Used to unsubscribe from events to prevent memory leaks.
	void OnDisable() {
		GameMaster.pan -= Activate;
		GameMaster.start -= Deactivate;
	}

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		cam = GetComponent<Camera>();
		gm.panCam = cam;
	}

	// Start - Use this for initialization.
	protected override void Start () {
		base.Start();
		if (gm.levelData != null && gm.levelData.firstTime) 
			gm.OnPreStart();
		else
			gm.OnStart();
	}

	// Activate - Enables camera component.
	void Activate() {
		StopCoroutine("Move");
		StartCoroutine("Move");
		cam.enabled = true;
		gm.cam.GetComponent<Camera>().enabled = false;
	}

	// Deactivate - Disables camera component.
	void Deactivate() {
		StopCoroutine("Move");
		gm.cam.GetComponent<Camera>().enabled = true;
		cam.enabled = false;
	}
}
