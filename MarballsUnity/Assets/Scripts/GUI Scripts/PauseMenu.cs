/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Mar.  4, 2015
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
	public GameMaster gm;

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
}
