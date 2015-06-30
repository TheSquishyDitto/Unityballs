/// <summary>
/// HealthCharm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for the maximum health boosting charm.
/// 
/// NOTES: - Increases maximum health by one level. (ex. 5 currently)
/// 
/// TO DO: - Tweak and/or optimize.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class HealthCharm : Charm {

	Marble marble;	// Reference to marble.
	
	// Initialize - Sets up charm's attributes.
	public override void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "HealthCharm");
		
		marble = GameMaster.CreateGM().marble;
	}
	
	// Effect - What the charm does.
	public override void Effect() {
		Debug.Log("Equipped " + type + "!");
		marble.maxHP += 5;
		Messenger<int, int>.Broadcast("UpdateHealth", marble.health, marble.maxHP);
	}
	
	// UnEffect - Reverses what the charm does.
	public override void UnEffect() {
		Debug.Log("Unequipped " + type + "!");
		marble.maxHP -= 5;
		marble.health = Mathf.Min(marble.health, marble.maxHP);
		Messenger<int, int>.Broadcast("UpdateHealth", marble.health, marble.maxHP);
	}
}
