/// <summary>
/// MainHUD.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Mar. 11, 2015
/// Last Revision: July 29, 2015
/// 
/// Class that controls the Heads Up Display (HUD) and associated menus.
/// 
/// TO DO: - Split functionality into multiple parts so the HUD isn't a god class.
/// 			+ Remaining to be split: Win Options?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MainHUD : MonoBehaviour {

	// Variables
	#region Variables
	GameMaster gm;					// Reference to Game Master.
	Settings settings;				// Reference to game settings.
	Canvas hud;						// Reference to own canvas.

	public GameObject winOptions; 	// Reference to win options.
	public GameObject debugSet;	  	// Reference to debug buttons and info display.

	public Color winColor;			// Color to tint the screen when winning.
	public Color deathColor;		// Color to tint the screen when dying.
	public List<string> deathMessages = new List<string>();	// An array of messages to use when the player dies.
	public List<string> winMessages = new List<string>();	// An array of messages to use when the player wins.

	//public TintScreen tintScreen;	// Reference to the tint screen.
	//public CountdownGUI countdown;// Reference to countdown manager.
	//public StatUpdater statBar;	// Reference to the bar on top of the screen.
	//public BuffBox buffBox;		// Reference to the buff box on the star bar. Unnecessary?
	//public TipBox tipBox;			// Reference to the tip box?
	//public AbilityBox abilityBox;	// Reference to the ability box?

	#endregion
	
	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		settings = GameMaster.LoadSettings();
		hud = GetComponent<Canvas>();

		GameMaster.sequence.AddSequence(new SequenceSlot(199, () => { hud.enabled = true; }));
	}

	// OnEnable - Called when the HUD is activated.
	void OnEnable() {
		Marble.die += ShowDeath;
		Marble.respawn += ClearTint;

		Messenger.AddListener("ClearHUDTint", ClearTint);
		Messenger.AddListener("Victory", PlayVictory);
	}

	// OnDisable - Called when the HUD is deactivated.	
	void OnDisable() {
		Marble.die -= ShowDeath;
		Marble.respawn -= ClearTint;

		Messenger.RemoveListener("ClearHUDTint", ClearTint);
		Messenger.RemoveListener("Victory", PlayVictory);
	}

	// Use this for initialization
	void Start () {

		GameMaster.sequence.StartSequence(gm, true);

		// Add level specific death and win messages to the mix.
		if (gm.levelData != null) {
			// Use level message settings.
			if (gm.levelData.messageMode == LevelDataObject.MessageMode.Append) {
				for (int i = 0; i < gm.levelData.deathMessages.Count; i++)
					deathMessages.Add(gm.levelData.deathMessages[i]);

				for (int i = 0; i < gm.levelData.winMessages.Count; i++)
					winMessages.Add(gm.levelData.winMessages[i]);

			} else if (gm.levelData.messageMode == LevelDataObject.MessageMode.Replace) {
				deathMessages = gm.levelData.deathMessages;
				winMessages = gm.levelData.winMessages;
			}
		}

		debugSet.SetActive(settings.debug);
	}

	// HideHUD - Hides HUD canvas.
	public void HideHUD() {
		hud.enabled = false;
	}

	// LevelSelect - Returns to the main menu and immediately goes to the level select section.
	public void LevelSelect (){
		gm.levelSelect = true;
		gm.LoadLevel(0);
	}

	// NextLevel - Loads the next level.
	// IN PROCESS OF CHANGING
	public void NextLevel (){
		gm.LoadLevel("HubWorld");
		/*
		if(Application.loadedLevel + 1 < Application.levelCount)
			gm.LoadLevel(Application.loadedLevel + 1);
		else
			Debug.LogWarning("(MainHUD.cs) NextLevel was called when there's no next level! Next level would be " + (Application.loadedLevel + 1) + " but maximum is " + (Application.levelCount - 1) + "!");
		*/
	}

	// Animation Coroutines - Display special GUI animations over time.
	#region Animation Coroutines

	// ClearTint - Clears all tint screens and removes any associated buttons.
	public void ClearTint() {
		Messenger.Broadcast("ClearTint");
		winOptions.SetActive(false);
	}

	// ShowDeath - Displays death tint screen.
	public void ShowDeath() {
		string deathMessage = deathMessages[Random.Range(0, deathMessages.Count)]; // Chooses a random message.
		TintInfo deathTint = new TintInfo(deathColor, Color.white, deathMessage, 1.25f, 0.5f);
		Messenger<TintInfo>.Broadcast("Tint", deathTint);
	}

	// PlayVictory - Shows victory screen.
	void PlayVictory() {
		StartCoroutine("OnVictory");
	}

	// OnVictory - Displays winning text and buttons.
	public IEnumerator OnVictory() {
		// Displays a victory tint screen.
		string winMessage = winMessages[Random.Range(0, winMessages.Count)]; // Chooses a random message.
		TintInfo winTint = new TintInfo(winColor, Color.white, winMessage, 0.75f, 0.5f);
		Messenger<TintInfo>.Broadcast("Tint", winTint);

		yield return new WaitForSeconds(0.75f); // Wait until tint is fully faded in before showing buttons.

		// Makes sure there's a subsequent level, or else disables the Next Level button.
		GameObject nextLevelButton = winOptions.transform.FindChild("Next").gameObject;		
		nextLevelButton.GetComponent<Button>().interactable = true; //(Application.loadedLevel + 1 <= gm.buildLevelCap);

		// Enables victory options.
		winOptions.SetActive(true);

	}

	#endregion
}
