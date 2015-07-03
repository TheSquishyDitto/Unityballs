/// <summary>
/// BuffBox.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 27, 2015
/// Last Revision: Jun. 27, 2015
/// 
/// Class that displays held and active buffs on the HUD.
/// 
/// NOTES: - Handles only the buff box.
/// 
/// TO DO: - Tweak.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuffBox : MonoBehaviour {
	// Variables
	#region Variables
	GameMaster gm;					// Reference to game master.

	public Image activePowerup;		// Reference to image of currently active powerup.
	public Image heldPowerup;		// Reference to held powerup picture.

	public GameObject buffBox;	  	// Reference to the active buff indicator.

	#endregion

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// OnEnable - Called when object is enabled.
	void OnEnable() {
		Messenger<Sprite, Color>.AddListener("SetHeldSprite", SetHeldSprite);
		Messenger<bool>.AddListener("ShowActiveBuff", ShowActiveBuff);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger<Sprite, Color>.RemoveListener("SetHeldSprite", SetHeldSprite);
		Messenger<bool>.RemoveListener("ShowActiveBuff", ShowActiveBuff);
	}

	// Start - Use this for initialization.
	void Start () {
		buffBox.SetActive((gm.marble.buffs[0].buff != BuffSource.PowerUp.None));
	}
	
	// Update - Called once per frame.
	void Update () {
		// Updates fill amount of active buff.
		if (!gm.paused && gm.state == GameMaster.GameState.Playing) {
			if (buffBox.activeSelf)
				activePowerup.fillAmount = (gm.marble.buffTimer != Mathf.Infinity)? gm.marble.buffTimer / gm.marble.buffs[0].duration : 1;
		}
	}

	// SetHeldSprite - Shows held buff.
	void SetHeldSprite(Sprite icon, Color iconTint) {
		heldPowerup.sprite = icon;
		heldPowerup.color = iconTint;
	}

	// ShowActiveBuff - Shows the active buff box and places the previously "held" buff into the active slot.
	void ShowActiveBuff(bool show) {
		buffBox.SetActive(show);

		// Places held buff icon into the active one and sets it up to gradually disappear.
		if (show) {
			activePowerup.sprite = heldPowerup.sprite;
			activePowerup.color = heldPowerup.color;
			activePowerup.type = UnityEngine.UI.Image.Type.Filled;
			activePowerup.fillMethod = UnityEngine.UI.Image.FillMethod.Radial360;
			heldPowerup.sprite = null;
			heldPowerup.color = Color.clear;
		}
	}
}
