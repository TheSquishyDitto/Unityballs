/// <summary>
/// TintScreen.cs
/// Authors: Kyle Dawson
/// Date Created:  July 29, 2015
/// Last Revision: July 29, 2015
/// 
/// Class for multipurpose screen tinter.
/// 
/// NOTES: - Cannot currently flexibly reposition tint elements.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TintScreen : MonoBehaviour {

	TimeManager tm;					// Reference to time manager.
	public Image tint;				// Reference to the color overlayed on the screen.
	public Text message;			// Reference to the text displayed on the screen.
	public Coroutine fade;			// Reference to fading routine.

	// Awake - Called before anything else.
	void Awake() {
		tm = TimeManager.CreateTimer();
	}

	// OnEnable - Called when enabled.
	void OnEnable() {
		Messenger<TintInfo>.AddListener("Tint", StartFade);
		Messenger.AddListener("ClearTint", Clear);
	}

	// OnDisable - Unsubscribe.
	void OnDisable() {
		Messenger<TintInfo>.RemoveListener("Tint", StartFade);
		Messenger.RemoveListener("ClearTint", Clear);
	}

	// StartFade - Initiates fading coroutine.
	void StartFade(TintInfo info) {
		Clear();
		info.routine = StartCoroutine(Fade(info));
	}

	// Fade - Gradually displays tint screen.
	IEnumerator Fade(TintInfo info) {
		// Initial tint colors are the desired colors, but transparent.
		Color initialTint = info.tintColor;
		initialTint.a = 0;
		Color initialText = info.textColor;
		initialText.a = 0;

		// Fades in the full screen tint over the desired time interval.
		TimeEvent tintFade = new TimeEvent(info.tintFadeLength);
		tm.StartStopwatch(tintFade);
		while(tintFade.duration > 0) {
			tint.color = Color.Lerp(initialTint, info.tintColor, tintFade.completion);
			yield return null;
		}

		// Does the same for the text.
		message.text = info.message;
		TimeEvent textFade = new TimeEvent(info.textFadeLength);
		tm.StartStopwatch(textFade);
		while(textFade.duration > 0) {
			message.color = Color.Lerp(initialText, info.textColor, textFade.completion);
			yield return null;
		}
	}

	// Clear - Clears the tint screen and removes it from view.
	public void Clear() {
		if (fade != null)
			StopCoroutine(fade);

		tint.color = Color.clear;
		message.color = Color.clear;
	}
}

public class TintInfo {
	public Color tintColor;			// Desired color of tint screen.
	public Color textColor;			// Desired color of text.
	public string message;			// Desired message of text.
	public float tintFadeLength;	// How long the tint fade in should be.
	public float textFadeLength;	// How long the text/message fade in should be.
	public Coroutine routine;		// Reference to coroutine utilizing this object.

	// Constructors
	public TintInfo(Color tintColor, Color textColor, string message, float tintFadeLength) {
		this.tintColor = tintColor;
		this.textColor = textColor;
		this.message = message;
		this.tintFadeLength = tintFadeLength;
		this.textFadeLength = 1;
	}

	public TintInfo(Color tintColor, Color textColor, string message, float tintFadeLength, float textFadeLength) {
		this.tintColor = tintColor;
		this.textColor = textColor;
		this.message = message;
		this.tintFadeLength = tintFadeLength;
		this.textFadeLength = textFadeLength;
	}
}