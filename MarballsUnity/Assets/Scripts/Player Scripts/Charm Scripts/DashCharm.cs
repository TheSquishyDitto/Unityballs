/// <summary>
/// DashCharm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for the dash granting charm.
/// 
/// NOTES: - Grants Power Dash ability.
/// 
/// TO DO: - Tweak and/or optimize.
/// 
/// </summary>


using UnityEngine;
using System.Collections;

public class DashCharm : Charm {

	Marble marble;	// Reference to marble.
	Ability dash;	// Dash ability.

	// Initialize - Sets up charm's attributes.
	public override void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "DashCharm");

		marble = GameMaster.CreateGM().marble;
		dash = new Ability("Power Dash", 1, 1, 1.5f, data.icon, Dash);
	}

	// Effect - What the charm does.
	public override void Effect() {
		Debug.Log("Equipped " + type + "!");
		marble.abilities.Add(dash);
		Messenger.Broadcast("UpdateAbility");
	}
	
	// UnEffect - Reverses what the charm does.
	public override void UnEffect() {
		Debug.Log("Unequipped " + type + "!");
		marble.abilities.Remove(dash);
		marble.abilityIndex = Mathf.Min(marble.abilityIndex, Mathf.Max(marble.abilities.Count - 1, 0));
		Messenger.Broadcast("UpdateAbility");
	}

	// Dash - Makes the marble dash.
	public void Dash() {
		marble.marbody.AddForce(Vector3.Scale(marble.cam.forward, new Vector3(1, 0, 1)) * 50, ForceMode.VelocityChange);
	}
}
