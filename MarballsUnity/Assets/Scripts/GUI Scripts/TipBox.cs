/// <summary>
/// TipBox.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 29, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class that displays the tip box.
/// 
/// NOTES: - Handles only the tip box.
/// 
/// TO DO: - Make fancier?
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TipBox : MonoBehaviour {

	public Image tipBackground;		// Reference to the background panel of tip windows.
	public Text tipMessage;			// Reference to the text in a tip window.

	// OnEnable - Called when object is activated.
	void OnEnable() {
		Messenger<TipBoxInfo>.AddListener("DisplayTip", DisplayTip);
	}

	// OnDisable - Called when object is deactivated.
	void OnDisable() {
		Messenger<TipBoxInfo>.RemoveListener("DisplayTip", DisplayTip);
	}

	// DisplayTip - Displays the tip box.
	void DisplayTip(TipBoxInfo info) {
		CancelInvoke("HideTip");
		
		tipMessage.text = info.text;
		tipMessage.color = info.textColor;
		tipBackground.color = info.boxColor;
		tipBackground.gameObject.SetActive(true);
		
		Invoke("HideTip", info.duration);
	}
	
	// HideTip - Hides tip box.
	void HideTip() { 
		tipBackground.gameObject.SetActive(false); 
	}
}
