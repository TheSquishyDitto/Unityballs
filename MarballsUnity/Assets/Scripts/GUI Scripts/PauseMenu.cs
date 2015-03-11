/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Mar.  9, 2015
/// 
/// Class that controls the pause menu canvas.
/// 
/// TO DO: - Actual functional buttons and submenus.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	public GameMaster gm;			// Reference to Game Master.
	public GameObject pauseSet;		// Reference to pause button menu set.
	public GameObject optionSet;	// Reference to option button submenu.
	public GameObject controlSet;	// Reference to controls submenu.

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.pauseMenu = this;
		gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Resume - Resumes gameplay.
	public void Resume(){
		gm.TogglePause();
	}

	// MainMenu - Goes to the main menu.
	public void MainMenu(){
		gm.LoadLevel(0);
	}

	// ToggleOptions - Goes to the option submenu and back.
	public void ToggleOptions() {
		pauseSet.SetActive (!pauseSet.activeSelf);
		optionSet.SetActive (!optionSet.activeSelf);
	}

	// QuitRequest - Quits the game.
	public void QuitRequest()
	{
		Application.Quit ();
	}
}
