/// <summary>
/// DefenseCharm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for a defense boosting charm.
/// 
/// NOTES: - Increases defense by 1.
/// 
/// TO DO: - Tweak and/or optimize.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class DefenseCharm : Charm {
	
	Marble marble;	// Reference to marble.
	
	// Initialize - Sets up charm's attributes.
	public override void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "DefenseCharm");
		
		marble = GameMaster.CreateGM().marble;
	}
	
	// Effect - What the charm does.
	public override void Effect() {
		Debug.Log("Equipped " + type + "!");
		marble.defense += 1;
		Messenger<int>.Broadcast("UpdateDefense", marble.defense);
	}
	
	// UnEffect - Reverses what the charm does.
	public override void UnEffect() {
		Debug.Log("Unequipped " + type + "!");
		marble.defense -= 1;
		Messenger<int>.Broadcast("UpdateDefense", marble.defense);
	}
}
