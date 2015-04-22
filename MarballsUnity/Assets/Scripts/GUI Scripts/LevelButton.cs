using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelButton : MonoBehaviour {

	public LevelDataObject levelData;	// Data of level to pull from.
	public int buildLevel = 0;			// Index of which level to load.
	public Text highScore;				// Reference to high score text.
	// NOTE: To get high scores dynamically, we will likely need to standardize the level data naming conventions for added levels.
	// 		 ex. Level1, Level2, Level3, Level4, Level5, just so we can load them even without having a reference.

	Button self;				// Reference to button component.

	// Start - Use this for initialization.
	void Start () {
		self = GetComponent<Button>();
		self.onClick.AddListener(ButtonLoadLevel);
		if (levelData != null) highScore.text = "High Score: " + levelData.bestTimes[0].ToString("F2") + " s";
	}

	void ButtonLoadLevel() {
		//Debug.Log("It worked!");
		Application.LoadLevel(buildLevel);
	}
}
