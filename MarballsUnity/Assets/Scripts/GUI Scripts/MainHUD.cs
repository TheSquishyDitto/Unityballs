/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Mar. 11, 2015
/// Last Revision: Apr. 19, 2015
/// 
/// Class that controls the Heads Up Display (HUD) and associated menus.
/// 
/// TO DO: - Add other features.
/// 	   - Refactor to use events, and possibly only one tint screen and text display!
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

	public Text timer;				// Reference to timer text.
	public Text best;				// Reference to high score text.
	public Text speed;				// Reference to speed gauge text.
	public Text rps;				// Reference to rps (rotations per second) gauge text.
	public Image countdown;			// Reference to countdown image container.
	public Image powerup;			// Reference to image of currently active powerup.
	public Image heldPowerup;		// Reference to held powerup picture.
	public Image deathScreen;		// Reference to death tint.
	public Text deathMessage;		// Reference to death message text.
	public Image winScreen;			// Reference to win tint.
	public Text winMessage;			// Reference to win message text.
	//public Image tintScreen;		// Reference to the panel that tints the screen.
	//public Text statusMessage;	// Reference to the text that displays during certain events.
	public Image tipBox;			// Reference to the background panel of tip windows.
	public Text tipMessage;			// Reference to the text in a tip window.

	public GameObject tipWindow;  	// Reference to the active tip window.
	public GameObject buffBox;	  	// Reference to the active buff indicator.
	public GameObject winOptions; 	// Reference to win options.
	public GameObject debugSet;	  	// Reference to debug buttons and info display.

	//public Color winColor;		// Color to tint the screen when winning.
	//public Color deathColor;		// Color to tint the screen when dying.
	public List<string> deathMessages = new List<string>();	// An array of messages to use when the player dies.
	public List<string> winMessages = new List<string>();	// An array of messages to use when the player wins.

	public Sprite[] nums;		// Easy holder of number textures.
	float remainder;			// Decimal portion of timer.
	public float goLength;		// Desired duration of "GO!" Should be between 0 and 1.

	#endregion
	
	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.hud = this;
	}

	// OnEnable - Called when the HUD is activated.
	void OnEnable() {
		GameMaster.start += BeginCountdown;
		GameMaster.play += HideCountdown;
	}

	// OnDisable - Called when the HUD is deactivated.	
	void OnDisable() {
		GameMaster.start -= BeginCountdown;
		GameMaster.play -= HideCountdown;
	}

	// Use this for initialization
	void Start () {
		//marble = gm.marble.transform;

		goLength = Mathf.Clamp(goLength, 0.01f, 0.999f);

		gm.OnStart();	// Begins the gameplay sequence when the HUD is ready.

		if (gm.levelData != null) {
			// Display absolute best high score.
			if (gm.levelData.bestTimes.Count > 0) 
				best.text = "Best: " + gm.levelData.bestTimes[0].ToString("F1") + " s";
			else
				best.enabled = false;

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

		buffBox.SetActive((gm.marble.buff != Marble.PowerUp.None));
		debugSet.SetActive(gm.debug);
	}
	
	// Update is called once per frame
	void Update () {
		if (!gm.paused) {

			if (gm.state == GameMaster.GameState.Start) {
				// 3 2 1 GO! Countdown
				// Changes number based on current integer portion of the timer.
				Sprite lastFrame = countdown.sprite;
				countdown.sprite = (gm.timer <= nums.Length)? nums[(int)(gm.timer + (1 - goLength))] : nums[nums.Length - 1]; // If timer is longer than the number of sprites, defaults to last.

				// Sound effect should play at the beginning and when the numbers change.
				if (lastFrame != countdown.sprite) {
					if (countdown.sprite != nums[0])
						AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/countdown"), gm.cam.position);
					else 
						AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/gong"), gm.cam.position);
				}

				// Makes number shrink as the timer goes down to the next number, with special conditions for "GO!".
				remainder = (gm.timer % 1) + (1 - goLength);
				if (remainder >= 1) remainder = remainder % 1;
				countdown.rectTransform.localScale = (gm.timer >= goLength) ? new Vector3(1, 1, 1) + new Vector3(remainder, remainder, remainder) : new Vector3(3, 3, 3);

			} else if (gm.state == GameMaster.GameState.Playing) {
				//countdown.enabled = false;
				timer.text = gm.timer.ToString("F1") + " s";	// Displays timer to one decimal place.

				// Cuts away active buff icon based on time remaining.
				if (buffBox.activeSelf)
					powerup.fillAmount = (gm.marble.buffTimer != Mathf.Infinity)? gm.marble.buffTimer / gm.marble.buffTimeMax : 1;
			}
		
			if (gm.marble != null) {
				// Speed gauges.
				speed.text = Mathf.Round(gm.marble.marbody.velocity.magnitude) + " m/s";
				rps.text = Mathf.Round(gm.marble.marbody.angularVelocity.magnitude) + " rps";
			}
		}
	}

	// BeginCountdown - Sets up countdown.
	public void BeginCountdown() {
		timer.text = "0.0 s";
		gm.timer += goLength;
		countdown.gameObject.SetActive(true);
		countdown.sprite = null;
	}

	// HideCountdown - Hides countdown.
	public void HideCountdown() {
		countdown.gameObject.SetActive(false);
	}

	// ShowActiveBuff - Shows the active buff box and places the previously "held" buff into the active slot.
	public void ShowActiveBuff() {
		buffBox.SetActive(true);
		powerup.sprite = heldPowerup.sprite;
		powerup.color = heldPowerup.color;
		powerup.type = UnityEngine.UI.Image.Type.Filled;
		powerup.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
		heldPowerup.sprite = null;
		heldPowerup.color = Color.clear;
	}

	// HideActiveBuff - Hides and clears the active buff box.
	public void HideActiveBuff() {
		buffBox.SetActive(false);
		// If we need to clear it, we can do so here.
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
	// OnDeath - Called when player falls off the stage or otherwise is killed.
	public IEnumerator OnDeath() {

		deathMessage.text = /*(Application.loadedLevel > 0 && gm.data[Application.loadedLevel - 1].customMessages)?
			gm.data[Application.loadedLevel - 1].deathMessages[Random.Range(0, gm.data[Application.loadedLevel - 1].deathMessages.Length)] :*/
			deathMessages[Random.Range(0, deathMessages.Count)]; // Chooses a random message.

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

		yield return new WaitForSeconds(2); // Punishment waiting.

		// Clears death screens.
		deathScreen.color = new Color(1, 0, 0, 0);
		deathMessage.color = new Color(1, 1, 1, 0);

		gm.marble.Respawn(); // Finally respawns player.
	}

	// OnVictory - Displays winning text and buttons.
	public IEnumerator OnVictory() {

		winMessage.text = /*(Application.loadedLevel > 0 && gm.data[Application.loadedLevel - 1].customMessages)?
			gm.data[Application.loadedLevel - 1].winMessages[Random.Range(0, gm.data[Application.loadedLevel - 1].winMessages.Length)] :*/
			winMessages[Random.Range(0, winMessages.Count)]; // Chooses a random message.

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
		nextLevelButton.GetComponent<Button>().interactable = (Application.loadedLevel + 1 < Application.levelCount);

		// Enables victory options.
		winOptions.SetActive(true);

	}

	#endregion
}
