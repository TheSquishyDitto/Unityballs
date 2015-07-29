/// <summary>
/// CountdownGUI.cs
/// Authors: Kyle Dawson
/// Date Created:  Mar. 23, 2015
/// Last Revision: July 23, 2015
/// 
/// Class that handles the 3, 2, 1, GO! countdown at level beginnings.
/// 
/// NOTES: - Countdown length can be set in Settings, but can be modified for other reasons here (such as go length)
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownGUI : MonoBehaviour {
	// Variables
	#region Variables
	public Image countdown;	// Reference to image container.
	public Sprite[] nums;	// Reference to 3, 2, 1, GO! images.
	public float goLength;	// How long "GO!" should last.

	TimeEvent timer;		// Actual timer.
	float remainder;		// Decimal portion of timer.
	Settings settings;		// Reference to game settings.

	#endregion

	// OnEnable - Called when object is enabled.
	void OnEnable() {
		Messenger<TimeEvent>.AddListener("Countdown", BeginCountdown);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger<TimeEvent>.RemoveListener("Countdown", BeginCountdown);
	}

	// Start - Use this for initialization.
	void Start() {
		settings = GameMaster.LoadSettings();
		goLength = Mathf.Clamp(goLength, 0.01f, 0.999f);
	}

	// BeginCountdown - Sets up countdown.
	public void BeginCountdown(TimeEvent clock) {
		timer = clock;
		timer.duration += goLength - .1f;
		countdown.gameObject.SetActive(true);
		countdown.sprite = null;
		StartCoroutine(Countdown());
	}

	// Countdown - Coroutine that displays images while counting down.
	public IEnumerator Countdown() {
		// 3 2 1 GO! Countdown
		
		while(timer.duration > 0) {
			// Changes number based on current integer portion of the timer.
			Sprite lastFrame = countdown.sprite;
			countdown.sprite = (timer.duration <= nums.Length)? nums[(int)(timer.duration + (1 - goLength))] : nums[nums.Length - 1]; // If timer is longer than the number of sprites, defaults to last.
			countdown.color = Color.white;
			
			// Sound effect should play at the beginning and when the numbers change.
			if (lastFrame != countdown.sprite) {
				if (countdown.sprite != nums[0])
					AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/countdown"), Vector3.zero, 0.5f * settings.FXScaler);
				else 
					AudioSource.PlayClipAtPoint((AudioClip)Resources.Load("Sounds/gong"), Vector3.zero, 0.5f * settings.FXScaler);
			}
			
			// Makes number shrink as the timer goes down to the next number, with special conditions for "GO!".
			remainder = (timer.duration % 1) + (1 - goLength);
			if (remainder >= 1) remainder = remainder % 1;
			countdown.rectTransform.localScale = (timer.duration >= goLength) ? new Vector3(1, 1, 1) + new Vector3(remainder, remainder, remainder) : new Vector3(3, 3, 3);
			
			yield return null;
		}

		countdown.gameObject.SetActive(false);	// Hides countdown when finished.
	}
}
