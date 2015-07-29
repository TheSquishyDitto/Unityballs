/// <summary>
/// PanScreen.cs
/// Authors: Kyle Dawson
/// Date Created:  July 22, 2015
/// Last Revision: July 23, 2015
/// 
/// Class that displays GUI on the panning screen.
/// 
/// NOTES: - Shows high scores and level name.
/// 
/// TO DO: - Make things fancier?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanScreen : MonoBehaviour {

	GameMaster gm;				// Reference to Game Master.
	Settings settings;			// Reference to game settings.

	public Canvas panScreen;	// Reference to canvas for the pan screen.
	public Text levelName;		// Reference to level name text box.
	public Text scores;			// Reference to scores text box.

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
		settings = GameMaster.LoadSettings();
	}

	// Start - Use this for initialization
	void Start () {
		if (gm.levelData != null) {
			levelName.text = gm.levelData.levelName;
			scores.text = "High Scores \n\n";
			
			// Display high scores.
			if (gm.levelData.bestTimes.Count > 0) {
				for (int i = 0; i < settings.highScoreCount; i++) {
					scores.text = scores.text + (i + 1) + ".) ";
					scores.text = (gm.levelData.bestTimes.Count > i)? scores.text + gm.levelData.bestTimes[i].ToString("F2") + " s" : scores.text + "-----";
					scores.text = scores.text + "\n";
				}
			}
		}
	}
}
