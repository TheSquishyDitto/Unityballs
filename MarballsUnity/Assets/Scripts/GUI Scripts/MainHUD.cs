/// <summary>
/// MainHUD.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Mar. 11, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class that controls the Heads Up Display (HUD) and associated menus.
/// 
/// TO DO: - Split functionality into multiple parts so the HUD is easier to maintain.
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

	public Canvas panScreen;		// Reference to pan screen canvas.
	public Text levelName;			// Reference to test displaying level name.
	public Text scores;				// Reference to text displaying all high scores.
	public Image countdown;			// Reference to countdown image container.
	public Image deathScreen;		// Reference to death tint.
	public Text deathMessage;		// Reference to death message text.
	public Image winScreen;			// Reference to win tint.
	public Text winMessage;			// Reference to win message text.
	//public Image tintScreen;		// Reference to the panel that tints the screen.
	//public Text statusMessage;	// Reference to the text that displays during certain events.

	public GameObject winOptions; 	// Reference to win options.
	public GameObject debugSet;	  	// Reference to debug buttons and info display.

	//public Color winColor;		// Color to tint the screen when winning.
	//public Color deathColor;		// Color to tint the screen when dying.
	public List<string> deathMessages = new List<string>();	// An array of messages to use when the player dies.
	public List<string> winMessages = new List<string>();	// An array of messages to use when the player wins.

	public Sprite[] nums;		// Easy holder of number textures.
	float remainder;			// Decimal portion of timer.
	public float goLength;		// Desired duration of "GO!" Should be between 0 and 1.

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
	}

	// OnEnable - Called when the HUD is activated.
	void OnEnable() {
		GameMaster.pan += HideHUD;
		GameMaster.start += BeginCountdown;
		GameMaster.play += HideCountdown;
		Marble.die += ShowDeath;
		Marble.respawn += ClearTint;

		Messenger.AddListener("ClearHUDTint", ClearTint);
		Messenger.AddListener("Victory", PlayVictory);
	}

	// OnDisable - Called when the HUD is deactivated.	
	void OnDisable() {
		GameMaster.pan -= HideHUD;
		GameMaster.start -= BeginCountdown;
		GameMaster.play -= HideCountdown;
		Marble.die -= ShowDeath;
		Marble.respawn -= ClearTint;

		Messenger.RemoveListener("ClearHUDTint", ClearTint);
		Messenger.RemoveListener("Victory", PlayVictory);
	}

	// Use this for initialization
	void Start () {

		goLength = Mathf.Clamp(goLength, 0.01f, 0.999f);

		if (gm.levelData != null) {
			levelName.text = gm.levelData.levelName;
			scores.text = "High Scores \n\n";

			// Display high scores.
			if (gm.levelData.bestTimes.Count > 0) {
				//best.text = "Best: " + gm.levelData.bestTimes[0].ToString("F1") + " s";
				for (int i = 0; i < settings.highScoreCount; i++) {
					scores.text = scores.text + (i + 1) + ".) ";
					scores.text = (gm.levelData.bestTimes.Count > i)? scores.text + gm.levelData.bestTimes[i].ToString("F2") + " s" : scores.text + "-----";
					scores.text = scores.text + "\n";
				}
			} else {
				//best.enabled = false;
			}

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
		} else {
			//best.enabled = false;
		}

		if (/*References.panCam == null*/gm.panCam == null)
			gm.OnStart();	// Begins the gameplay sequence when the HUD is ready.

		debugSet.SetActive(settings.debug);
	}
	
	// Update is called once per frame
	void Update () {
		if (!gm.paused) {

			if (gm.state == GameMaster.GameState.Start) {
				// 3 2 1 GO! Countdown
				// Changes number based on current integer portion of the timer.
				Sprite lastFrame = countdown.sprite;
				countdown.sprite = (gm.timer <= nums.Length)? nums[(int)(gm.timer + (1 - goLength))] : nums[nums.Length - 1]; // If timer is longer than the number of sprites, defaults to last.
				countdown.color = Color.white;

				// Sound effect should play at the beginning and when the numbers change.
				if (lastFrame != countdown.sprite) {
					if (countdown.sprite != nums[0])
						AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/countdown"), Camera.main.transform.position, 0.5f * settings.FXScaler);
					else 
						AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/gong"), Camera.main.transform.position, 0.5f * settings.FXScaler);
				}

				// Makes number shrink as the timer goes down to the next number, with special conditions for "GO!".
				remainder = (gm.timer % 1) + (1 - goLength);
				if (remainder >= 1) remainder = remainder % 1;
				countdown.rectTransform.localScale = (gm.timer >= goLength) ? new Vector3(1, 1, 1) + new Vector3(remainder, remainder, remainder) : new Vector3(3, 3, 3);

			}
		}
	}

	// HideHUD - Hides HUD canvas.
	public void HideHUD() {
		hud.enabled = false;
		panScreen.enabled = true;
	}

	// BeginCountdown - Sets up countdown.
	public void BeginCountdown() {
		panScreen.enabled = false;
		hud.enabled = true;
		//timer.text = "0.0 s";
		gm.timer += goLength - .1f;
		countdown.gameObject.SetActive(true);
		countdown.sprite = null;
	}

	// HideCountdown - Hides countdown.
	public void HideCountdown() {
		countdown.gameObject.SetActive(false);
	}

	// ShowDeath - Starts death coroutine.
	public void ShowDeath() {
		StartCoroutine("OnDeath");
	}
	
	// Restart - Reloads the current level.
	public void Restart (){
		gm.LoadLevel(Application.loadedLevel);
	}

	// LevelSelect - Returns to the main menu and immediately goes to the level select section.
	public void LevelSelect (){
		gm.levelSelect = true;
		gm.LoadLevel(0);
	}

	// NextLevel - Loads the next level.
	public void NextLevel (){
		if(Application.loadedLevel + 1 < Application.levelCount)
			gm.LoadLevel(Application.loadedLevel + 1);
		else
			Debug.LogWarning("(MainHUD.cs) NextLevel was called when there's no next level! Next level would be " + (Application.loadedLevel + 1) + " but maximum is " + (Application.levelCount - 1) + "!");
	}

	// Animation Coroutines - Display special GUI animations over time.
	#region Animation Coroutines

	// ClearTint - Clears all tint screens and removes any associated buttons.
	public void ClearTint() {
		deathScreen.color = new Color(1, 0, 0, 0);
		deathMessage.color = new Color(1, 1, 1, 0);
		winMessage.color = new Color(1, 1, 1, 0);
		winScreen.color = new Color(0, 1, 0, 0);
		winOptions.SetActive(false);
	}

	// OnDeath - Called when player falls off the stage or otherwise is killed.
	public IEnumerator OnDeath() {

		deathMessage.text = deathMessages[Random.Range(0, deathMessages.Count)]; // Chooses a random message.

		// Makes the red screen visible.
		for (int i = 0; i < 25; i++) {
			deathScreen.color = new Color(1, 0, 0, i/50.0f);
			yield return new WaitForSeconds(0.05f);
		}

		// Makes the death text visible.
		for (int i = 0; i < 10; i++) {
			deathMessage.color = new Color(1, 1, 1, i/10.0f);
			yield return new WaitForSeconds(0.05f);
		}
	}

	// PlayVictory - Shows victory screen.
	void PlayVictory() {
		StartCoroutine("OnVictory");
	}

	// OnVictory - Displays winning text and buttons.
	public IEnumerator OnVictory() {

		winMessage.text = winMessages[Random.Range(0, winMessages.Count)]; // Chooses a random message.

		// Makes the green screen visible.
		for (int i = 0; i < 25; i++) {
			winScreen.color = new Color(winScreen.color.r, winScreen.color.g, winScreen.color.b, i/50.0f);
			yield return new WaitForSeconds(0.05f);
		}
		
		// Makes the win text visible.
		for (int i = 0; i < 10; i++) {
			winMessage.color = new Color(winMessage.color.r, winMessage.color.g, winMessage.color.b, i/10.0f);
			yield return new WaitForSeconds(0.05f);
		}
		
		yield return new WaitForSeconds(0.75f);

		// Makes sure there's a subsequent level, or else disables the Next Level button.
		GameObject nextLevelButton = winOptions.transform.FindChild("Next Level").gameObject;		
		nextLevelButton.GetComponent<Button>().interactable = (Application.loadedLevel + 1 <= gm.buildLevelCap);

		// Enables victory options.
		winOptions.SetActive(true);

	}

	#endregion
}
