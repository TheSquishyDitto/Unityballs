/// <summary>
/// LevelGUI.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 11, 2015
/// Last Revision: Feb. 16, 2015
/// 
/// Class that handles the GUI when a gameplay level scene is loaded.
/// 
/// NOTES: - With Unity UI this will be phased out before it even becomes very relevant.
/// 	   - Currently only used for pause menu and debug things.
/// 
/// TO DO: - Phase out in favor of Unity UI canvases and such.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class LevelGUI : MonoBehaviour {

	public GameMaster gm;		// Reference to Game Master.
	public Transform marble;	// Reference to currently active marble.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.gui = this;	// Tells the Game Master that this is the current level GUI.
	}

	// Use this for initialization
	void Start () {
		marble = gm.marble;
	}

	// OnGUI - Used for GUIs. This is a draw call so order, presence, or absence of code can be fairly important.
	// NOTE: Currently using a whole lotta magic numbers, feel free to change those to constants or public variables and experiment.
	void OnGUI() {

		// Debug menus, buttons, and info.
		if (gm.debug) {
			if (!gm.paused)
				GUI.Label (new Rect(Screen.width / 2 - 25, 10, 100, 25), "Timer: " + (Mathf.Round(gm.timer * 10) / 10.0) + " s");

			// Puts game into start state.
			if (GUI.Button(new Rect(10, 20, 80, 30), "Start")) {
				gm.OnStart();
			}

			// Puts game into playing state.
			if (GUI.Button(new Rect(10, 50, 80, 30), "Playing")) {
				gm.OnPlay();
			}
		}

		if (marble != null) {
			// Speed gauge.
			GUI.Label (new Rect(10, Screen.height - 25, 150, 25), "Speed: " + Mathf.Round(marble.rigidbody.velocity.magnitude) + " m/s");
		}


		// Pause screen
		if (gm.paused) {
			GUI.Box(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), "");
			GUI.Label(new Rect(Screen.width / 2 - 25, 10, 100, 25), "PAUSED");

			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 40, 80, 30), "Resume")) {
				gm.TogglePause();
			}

			if (GUI.Button(new Rect(Screen.width / 2 - 40, Screen.height / 2 - 10, 80, 30), "Main Menu")) {
				gm.LoadLevel("MainMenu");
			}
		}
	}

	// Update is called once per frame
	void Update () {

	}
}
