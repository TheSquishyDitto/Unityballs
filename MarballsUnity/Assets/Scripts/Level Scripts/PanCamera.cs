/// <summary>
/// PanCamera.cs
/// Authors: Kyle Dawson, Chris Viqueira, Charlie Sun
/// Date Created:  Apr. 21, 2015
/// Last Revision: July 22, 2015
/// 
/// Class for Marballs' specific camera panning action.
/// 
/// NOTES: - Pans the level at the beginning if it exists. (PRIORITY 100)
/// 	   - Pressing the Jump key should allow you to move on.
/// 	   - Pressing Pause should exit the level.
/// 
/// TO DO: - Tweak until behaves as desired.
/// 	   - Look into enabling/disabling pan cam rather than the main camera.
/// 	   - Make this class use InputManager keys rather than hardcoded ones.
/// 	   - Add more options to make path creation nicer.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanCamera : ScriptedPath {

	GameMaster gm;		// Reference to GameMaster.
	Transform mainCam;	// Reference to main camera.
	Camera cam;			// Reference to attached camera component.

	public GameObject panPrefab;	// Reference to canvas prefab.
	public Canvas panScreen; 		// The screen displayed when panning.

	// Awake - Called before anything else.
	protected override void Awake () {
		base.Awake();
		gm = GameMaster.CreateGM();
		mainCam = Camera.main.transform;
		cam = GetComponent<Camera>();
		panScreen = Instantiate(panPrefab).GetComponent<Canvas>();

		GameMaster.sequence.AddSequence(new SequenceSlot(100, Pan()));
	}

	// Start - Use this for initialization.
	protected override void Start () {
		base.Start();
		panScreen.transform.SetParent(gm.guiContainer);

		GameMaster.sequence.StartSequence(gm, true);
		/*if (gm.levelData != null && gm.levelData.firstTime) 
			gm.OnPreStart();
		else
			gm.OnStart();*/
	}

	// Pan - Coroutine that causes panning to occur.
	public IEnumerator Pan() {
		Activate();
		
		while(!(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
			yield return null;

		//if (Input.GetKeyDown(KeyCode.Escape))
		//	gm.LoadLevel("HubWorld");
		
		Deactivate();
	}

	// Activate - Enables camera component.
	void Activate() {
		StopCoroutine("Move");
		StartCoroutine("Move");
		cam.enabled = true;
		mainCam.GetComponent<Camera>().enabled = false;
		panScreen.enabled = true;
	}

	// Deactivate - Disables camera component.
	void Deactivate() {
		StopCoroutine("Move");
		mainCam.GetComponent<Camera>().enabled = true;
		cam.enabled = false;
		panScreen.enabled = false;
	}
}
