/// <summary>
/// DebugMenu.cs
/// Authors: Kyle Dawson
/// Date Created:  Mar. 22, 2015
/// Last Revision: July 19, 2015
/// 
/// Class that controls our debug menu.
/// 
/// NOTES: - Changing the FPS only works if VSync is disabled in quality settings!
/// 	   - FPS slider value may not be correct if other classes modify the target framerate.
/// 	   - FPS slider will be useful as an actual menu option.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;

public class DebugMenu : MonoBehaviour {

	GameMaster gm;				// Reference to GameMaster.
	Settings settings;			// Reference to game settings.
	
	public Slider fpsSlider;	// Reference to target frame setting slider.
	public Text fpsText;		// Reference to accompanying text for target FPS slider.
	public Toggle start;		// Reference to start toggle.
	public Toggle play;			// Reference to play toggle.
	public Toggle simple;		// Reference to simple animations toggle.
	public RectTransform excon;	// Reference to expand/contact button.
	public GameObject content;	// Reference to GameObject containing contents of debug menu.

	bool manual;				// Whether a toggle was activated manually or not.
	bool expanded = true;		// Whether menu is expanded or not.

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
		settings = GameMaster.LoadSettings();
	}

	// Start - Use this for initialization.
	void Start () {
		simple.isOn = settings.simpleAnim;//gm.simpleAnim;

		Application.targetFrameRate = settings.targetFPS;//60;

		if (fpsSlider) fpsSlider.value = Application.targetFrameRate;

		ToggleExpansion();

		// RESET TOGGLE BUTTONS BASED ON SETTINGSS AS WELL
	}
	
	// Update - Called once per frame.
	void Update () {

		if (fpsSlider) fpsText.text = "Target FPS: " + fpsSlider.value;	// Updates the text to the current target value.

		//RefreshState();
	}

	// DEBUG - ChangeFPS - Changes application's preferred FPS. Acts as a soft ceiling.
	public void ChangeFPS (float targetFPS) {
		Application.targetFrameRate = (int)targetFPS;
	}

	// DEBUG - ChangeFPS - Changes application's preferred FPS. Acts as a soft ceiling.
	public void ChangeFPS (string targetFPS) {
		Application.targetFrameRate = int.Parse(targetFPS);
	}

	/*// DEBUG - ForceStart - Sets game state to the starting state.
	public void ForceStart (bool start = true){
		if (start && manual) {
			gm.CancelCoroutines();
			gm.OnStart();
		}
	}
	
	// DEBUG - ForcePlay - Sets game state to the playing state.
	public void ForcePlay (bool play = true){
		if (play && manual) {
			gm.CancelCoroutines();
			gm.OnPlay();
		}
	}

	// DEBUG - ForcePan - Sets game state to the panning state.
	public void ForcePan (bool pan = true){
		if (pan && manual) {
			gm.CancelCoroutines();
			gm.OnPreStart();
		}
	}

	// DEBUG - ForceWin - Sets game state to the winning state.
	public void ForceWin (bool win = true){
		if (win && manual) {
			gm.CancelCoroutines();
			gm.OnWin();
		}
	}

	// RefreshState - Keeps state toggle group up to date.
	void RefreshState() {
		manual = false;
		if (gm.state == GameMaster.GameState.Start && !start.isOn) {
			start.isOn = true;
			play.isOn = false;
		} 
		if (gm.state == GameMaster.GameState.Playing && !play.isOn) {
			start.isOn = false;
			play.isOn = true;
		}
		manual = true;
	}*/
	
	// DEBUG - SimpleWin - Turns simple animations on or off.
	public void SimpleAnim (bool simple) {
		//gm.simpleAnim = simple;
		settings.simpleAnim = simple;
	}

	// DEBUG - GravFinish - Turns gravity finish on or off.
	public void GravFinish(bool grav) {
		settings.gravityFinish = grav;
	}

	// DEBUG - UseOnGrab - Automatically use picked up buffs.
	public void UseOnGrab (bool use) {
		settings.useOnGrab = use;
	}

	// DEBUG - FreezeTimer - Stops the level timer.
	public void FreezeTimer(bool freeze) {
		settings.freezeTimer = freeze;
	}

	// DEBUG - Flashlight - Enables marble's kinetic-powered flashlight.
	public void Flashlight(bool on) {
		gm.marble.flashLight = on;

		if (!on) gm.marble.GetComponent<Light>().enabled = false;
	}

	// DEBUG - CheatFinish - Wins the level for you.
	public void CheatFinish() {
		gm.marble.transform.position = FinishLine.finish.position;
	}

	// DEBUG - EraseSave - Permanently deletes saved level data.
	// NOTE: Data can be reclaimed if an old high score is beaten before the level data object resets.
	public void EraseSave() {
		if (gm.levelData != null) {
			if(File.Exists(gm.GetFilePath())) {
				File.Delete(gm.GetFilePath());
				Debug.LogWarning("(DebugMenu.cs) Deleted level data at " + gm.GetFilePath());
			} else {
				Debug.LogWarning("(DebugMenu.cs) No saved data for this level found!");
			}
		}
	}

	// DEBUG - ToggleExpansion
	public void ToggleExpansion() {
		Vector3 buttonRot = (expanded)? new Vector3(0, 0, 90) : new Vector3(0, 0, -90);
		excon.Rotate(buttonRot);
		expanded = !expanded;
		content.SetActive(expanded);

		//if (expanded) RefreshState();
	}
}
