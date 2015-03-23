/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson
/// Date Created:  Mar. 11, 2015
/// Last Revision: Mar. 22, 2015
/// 
/// Class that controls the Heads Up Display (HUD) and associated menus.
/// 
/// TO DO: - Tweak countdown until it behaves as desired.
/// 	   - Add other features.
/// 	   - Deprecate nextLevel variable.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainHUD : MonoBehaviour {

		// Variables
	#region Variables
	public GameMaster gm;		// Reference to Game Master.
	//public Transform marble;	// Reference to currently active marble.
	public Text timer;			// Reference to timer text.
	public Text speed;			// Reference to speed gauge text.
	public Text rps;			// Reference to rps (rotations per second) gauge text.
	public Image countdown;		// Reference to countdown image container.
	public Image powerup;		// Reference to powerup picture.
	public Image deathScreen;	// Reference to death tint.
	public Text deathMessage;	// Reference to death message text.
	public Image winScreen;		// Reference to win tint.
	public Text winMessage;		// Reference to win message text.
	
	public GameObject winOptions; // Reference to win options.
	public GameObject debugSet;	// Reference to debug buttons and info display.

	public Sprite[] nums;		// Easy holder of number textures.
	float remainder;			// Decimal portion of timer.
	public float goLength;		// Desired duration of "GO!" Should be between 0 and 1.
	//int nextLevel;			// Stores value of next level
	#endregion
	
	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
		gm.hud = this;
	}
	
	// Use this for initialization
	void Start () {
		//marble = gm.marble.transform;

		goLength = Mathf.Clamp(goLength, 0.01f, 0.999f);

		gm.OnStart();	// Begins the gameplay sequence when the HUD is ready.

		debugSet.SetActive(gm.debug);
		//nextLevel = Application.loadedLevel + 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (!gm.paused) {

			if (gm.state == GameMaster.GameState.Start) {
				timer.text = "0.0 s";

				// 3 2 1 GO! Countdown
				// Changes number based on current integer portion of the timer.
				countdown.sprite = (gm.timer <= nums.Length)? nums[(int)(gm.timer + (1 - goLength))] : nums[nums.Length - 1]; // If timer is longer than the number of sprites, defaults to last.
				// [sound effect for timer changing should go here]

				// Makes number shrink as the timer goes down to the next number, with special conditions for "GO!".
				remainder = (gm.timer % 1) + (1 - goLength);
				if (remainder >= 1) remainder = remainder % 1;
				countdown.rectTransform.localScale = (gm.timer >= goLength) ? new Vector3(1, 1, 1) + new Vector3(remainder, remainder, remainder) : new Vector3(3, 3, 3);

			} else {
				timer.text = gm.timer.ToString("F1") + " s";	// Displays timer to one decimal place.
			}
		
			if (gm.marble != null) {
				// Speed gauge.
				speed.text = Mathf.Round(gm.marble.marbody.velocity.magnitude) + " m/s";
				rps.text = Mathf.Round(gm.marble.marbody.angularVelocity.magnitude) + " rps";

				// [ maybe consider an RPM counter as well ]
			}
		}
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

	// OnDeath - Called when player falls off the stage or otherwise is killed.
	public IEnumerator OnDeath() {

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
}
