/// <summary>
/// DebugMenu.cs
/// Authors: Kyle Dawson
/// Date Created:  Mar. 22, 2015
/// Last Revision: Apr. 11, 2015
/// 
/// Class that controls our debug menu.
/// 
/// NOTES: - Changing the FPS only works if VSync is disabled in quality settings!
/// 	   - FPS slider value may not be correct if other classes modify the target framerate.
/// 	   - FPS slider may be useful as an actual menu option.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugMenu : MonoBehaviour {

	GameMaster gm;				// Reference to GameMaster.

	public Slider fpsSlider;	// Reference to target frame setting slider.
	public Text fpsText;		// Reference to accompanying text for target FPS slider.
	public Toggle start;		// Reference to start toggle.
	public Toggle play;			// Reference to play toggle.
	public Toggle simple;		// Reference to simple animations toggle.

	bool manual;				// Whether a toggle was activated manually or not.

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization.
	void Start () {
		simple.isOn = gm.simpleAnim;

		Application.targetFrameRate = 60;

		if (fpsSlider) fpsSlider.value = Application.targetFrameRate;
	}
	
	// Update - Called once per frame.
	void Update () {

		if (fpsSlider) fpsText.text = "Target FPS: " + fpsSlider.value;	// Updates the text to the current target value.

		if (gm.state == GameMaster.GameState.Start && !start.isOn) {
			manual = false;
			start.isOn = true;
			manual = true;
		}
		if (gm.state == GameMaster.GameState.Playing && !play.isOn) {
			manual = false;
			play.isOn = true;
			manual = true;
		}
	}

	// DEBUG - ChangeFPS - Changes application's preferred FPS. Acts as a soft ceiling.
	public void ChangeFPS (float targetFPS) {
		Application.targetFrameRate = (int)targetFPS;
	}

	// DEBUG - ChangeFPS - Changes application's preferred FPS. Acts as a soft ceiling.
	public void ChangeFPS (string targetFPS) {
		Application.targetFrameRate = int.Parse(targetFPS);
	}

	// DEBUG - StartButton - Sets game state to the starting state.
	public void ForceStart (bool start = true){
		if (start && manual) {
			gm.CancelCoroutines();
			gm.OnStart();
		}
	}
	
	// DEBUG - PlayButton - Sets game state to the playing state.
	public void ForcePlay (bool play = true){
		if (play && manual) {
			gm.CancelCoroutines();
			gm.OnPlay();
		}
	}
	
	// DEBUG - SimpleWin - Turns simple animations on or off.
	public void SimpleAnim (bool simple) {
		gm.simpleAnim = simple;
	}

	// DEBUG - UseOnGrab - Automatically use picked up buffs.
	public void UseOnGrab (bool use) {
		gm.useOnGrab = use;
	}

	// DEBUG - CheatFinish - Wins the level for you.
	public void CheatFinish() {
		gm.marble.transform.position = gm.finishLine.position;
	}
}
