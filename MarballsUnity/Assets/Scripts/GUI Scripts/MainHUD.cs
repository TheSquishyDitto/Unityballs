/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Mar. 11, 2015
/// Last Revision: Mar. 11, 2015
/// 
/// Class that controls the Heads Up Display (HUD) and associated menus.
/// 
/// TO DO: - Tweak countdown until it behaves as desired.
/// 	   - Add other features.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour {

	// Variables
	#region Variables
	public GameMaster gm;		// Reference to Game Master.
	public Transform marble;	// Reference to currently active marble.
	public Text timer;			// Reference to timer text.
	public Text speed;			// Reference to speed gauge text.
	public Image countdown;		// Reference to countdown image container.
	public GameObject debugSet;	// Reference to debug buttons and info display.

	public Sprite[] nums;		// Easy holder of number textures.
	float remainder;			// Decimal portion of timer.
	#endregion
	
	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM();
	}
	
	// Use this for initialization
	void Start () {
		marble = gm.marble;

		debugSet.SetActive(gm.debug);
	}
	
	// Update is called once per frame
	void Update () {
		if (!gm.paused) {

			if (gm.state == GameMaster.GameState.Start) {
				// 3 2 1 Countdown
				countdown.gameObject.SetActive(true);	// Should be moved to GM's OnStart function.
				timer.gameObject.SetActive(false);		// Should be moved to GM's OnStart function.

				// Changes number based on current integer portion of the timer.
				countdown.sprite = (gm.timer <= nums.Length)? nums[Mathf.FloorToInt(gm.timer)] : nums[nums.Length - 1]; // If timer is longer than the number of sprites, defaults to last.
				// [sound effect for timer changing should go here]

				// Makes number shrink as the timer goes down to the next number.
				remainder = gm.timer % 1;
				countdown.rectTransform.localScale = (gm.timer >= 1) ? new Vector3(1, 1, 1) + new Vector3(remainder, remainder, remainder) : new Vector3(2, 2, 2);

			} else {
				timer.gameObject.SetActive(true);		// Should be moved to GM's OnPlay function.
				countdown.gameObject.SetActive(false);	// Should be moved to GM's OnPlay function.

				timer.text = (Mathf.Round(gm.timer * 10) / 10.0) + " s"; // Timer
			}
		
			if (marble != null) {
				// Speed gauge.
				speed.text = Mathf.Round(marble.GetComponent<Rigidbody>().velocity.magnitude) + " m/s";

				// [ maybe consider an RPM counter as well ]
			}
		}
	}
	
	// Debug buttons
	public void StartButton (){
		gm.OnStart();
	}
	
	public void PlayButton (){
		gm.OnPlay();
	}
}
