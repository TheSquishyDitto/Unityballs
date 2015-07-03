/// <summary>
/// Charm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 27, 2015
/// Last Revision: Jun. 28, 2015
/// 
/// Class for base charm data and functionality.
/// 
/// NOTES: - Different charms should inherit from this class.
/// 	   - Ability class is currently located at the bottom.
/// 
/// TO DO: - Implement magnitude.
/// 
/// </summary>

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Charm {
	public string type;				// The name of this type of charm.
	/*public int cost = 0; 			// The CP cost of equipping this type of charm.
	public bool visible = true;		// Whether this charm shows up to the player.
	public Sprite icon;				// The icon displayed in the charm management window/ability box.
	public string description;		// A description of what this charm does/offers.
	public Color tint;				// The color to tint the icon.*/

	protected string dataPath;		// Where in this project the scriptableobject charm assets are.
	public CharmData data;			// Reference to the data used by this charm.
	
	public bool equipped = false;	// Whether this charm is equipped to the player.

	// Constructor
	public Charm() {
		dataPath = "Data/Charm Data/";
		Initialize();
		type = data.type;
	}

	// Initialize - Sets up charm's attributes.
	public virtual void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "BasicCharm");
	}

	// Equip - Equips an unequipped charm.
	public void Equip() {
		Marble marble = GameMaster.CreateGM().marble;

		if (marble.charmPoints >= data.cost) {

			if (equipped)
				Debug.LogWarning(type + " charm was already equipped?");

			marble.charmPoints -= data.cost;
			equipped = true;
			Effect();
		} else { 
			Debug.Log("Not enough charm points to equip charm!"); 
		}
	}

	// Unequip - Unequips an equipped charm.
	public void Unequip() {
		if (equipped) {
			Marble marble = GameMaster.CreateGM().marble;
			UnEffect();
			equipped = false;
			marble.charmPoints += data.cost;
		} else {
			Debug.Log("This charm was not equipped!");
		}
	}

	// ToggleEquip - Equips or unequips the charm.
	public void ToggleEquip() {
		if (!equipped)
			Equip();
		else
			Unequip();
	}

	// Effect - What the charm does.
	public virtual void Effect() {
		Debug.Log("Equipped " + type + "!");
	}

	// UnEffect - Reverses what the charm does.
	public virtual void UnEffect() {
		Debug.Log("Unequipped " + type + "!");
	}
	
}

[System.Serializable]
public class Ability {
	// MAGNITUDE IS NOT YET FUNCTIONAL, EQUIPPING MULTIPLE COPIES JUST MAKES THE ABILITY SHOW UP MULTIPLE TIMES

	public string type;				// Name of ability.
	public int id = 0;				// ID of this ability. Abilities with the same ID are the same ability.
	public int cost = 0;			// MP cost of using this ability.
	public float cooldown = 0;		// How long after using this ability can you use it, or others, again.
	public int magnitude = 0;		// How strong this ability is. Wearing copies of the same charm increases the magnitude.
	public Sprite icon;				// The icon displayed in the ability box.
	public UnityAction ability;		// The actual functionality of the ability.

	public Ability(string type, int id, int cost, float cooldown, Sprite icon, UnityAction ability) {
		this.type = type;
		this.id = id;
		this.cost = cost;
		this.cooldown = cooldown;
		this.icon = icon;
		this.ability = ability;
	}
}