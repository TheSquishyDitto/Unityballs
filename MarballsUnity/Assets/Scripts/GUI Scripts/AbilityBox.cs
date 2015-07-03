/// <summary>
/// AbilityBox.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class that displays available abilities on the HUD.
/// 
/// NOTES: - Handles only the ability box.
/// 
/// TO DO: - Tweak and make fancier. (Add swapping animation.)
/// 
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityBox : MonoBehaviour {

	GameMaster gm;					// Reference to game master.
	public Image abilityIcon;		// Container for ability icons.
	public Image cooldownFilter;	// Container for visual representation of cooldown.

	// Awake - Called before anything else.
	void Awake() {
		gm = GameMaster.CreateGM();
	}

	// OnEnable - Called when object becomes active.
	void OnEnable() {
		Messenger.AddListener("UpdateAbility", UpdateAbility);
	}

	// OnDisable - Called when object is disabled.
	void OnDisable() {
		Messenger.RemoveListener("UpdateAbility", UpdateAbility);
	}

	// Update - Called once per frame.
	void Update () {
		// Updates cooldown fill amount.
		if (abilityIcon.gameObject.activeSelf && gm.marble.abilityIndex < gm.marble.abilities.Count) {
			cooldownFilter.fillAmount = Mathf.Max(gm.marble.timeStamp - Time.time, 0) / gm.marble.abilities[gm.marble.abilityIndex].cooldown;
		}
	}

	// UpdateAbility - Updates the picture in the ability box.
	public void UpdateAbility() {
		// If the marble has any abilities...
		if (gm.marble.abilities.Count > 0) {
			// Sets icon.
			abilityIcon.gameObject.SetActive(true);
			abilityIcon.sprite = gm.marble.abilities[gm.marble.abilityIndex].icon;

			// Makes cooldown image the same as the sprite.
			cooldownFilter.sprite = abilityIcon.sprite;
			cooldownFilter.type = Image.Type.Filled;
			cooldownFilter.fillMethod = Image.FillMethod.Vertical;
			cooldownFilter.fillAmount = 0;
		} else {
			// Disable image if there are no active abilities.
			abilityIcon.gameObject.SetActive(false);
		}
	}
}
