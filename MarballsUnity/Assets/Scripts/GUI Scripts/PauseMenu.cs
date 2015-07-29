/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Feb. 13, 2015
/// Last Revision: Jun. 28, 2015
/// 
/// Class that controls the pause menu canvas.
/// 
/// TO DO: - More functional settings menus. (AUDIO, VIDEO, GAME SETTINGS)
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	GameMaster gm;					// Reference to Game Master.
	public Canvas canvas;			// Reference to the pause canvas.
	public GameObject pauseSet;		// Reference to pause button menu set.
	public GameObject optionSet;	// Reference to option button submenu.
	public GameObject controlSet;	// Reference to controls submenu.

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		canvas = GetComponent<Canvas>();
		//gm.pauseMenu = this;
		canvas.enabled = false;
	}

	// OnEnable - Called when object is enabled.
	void OnEnable() {
		Messenger<bool>.AddListener("SetPauseActive", SetPauseActive);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger<bool>.RemoveListener("SetPauseActive", SetPauseActive);
	}

	// SetPauseActive - Enables/disables pause menu.
	void SetPauseActive(bool active) {
		canvas.enabled = active;
	}

	// Resume - Resumes gameplay.
	public void Resume(){
		gm.TogglePause();
	}

	// MainMenu - Goes to the main menu.
	public void MainMenu(){
		gm.LoadLevel(0);
	}

	// Restart - Reloads the current level.
	public void Restart (){
		gm.LoadLevel(Application.loadedLevel);
	}

	// ResetMenu - Returns to initial pause menu state.
	public void ResetMenu() {
		pauseSet.SetActive(true);
		optionSet.SetActive(false);
		controlSet.SetActive(false);
	}

	// ToggleOptions - Goes to the option submenu and back.
	public void ToggleOptions() {
		pauseSet.SetActive (!pauseSet.activeSelf);
		optionSet.SetActive (!optionSet.activeSelf);
	}

	// ToggleControls - Activates control menu.
	public void ToggleControls() {
		gm.input.allowInput = !gm.input.allowInput;
		optionSet.SetActive (!optionSet.activeSelf);
		controlSet.SetActive(!controlSet.activeSelf);
	}

	// QuitRequest - Quits the game.
	public void QuitRequest() {
		Application.Quit ();
	}
}
