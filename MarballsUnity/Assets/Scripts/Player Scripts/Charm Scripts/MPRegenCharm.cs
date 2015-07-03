/// <summary>
/// MPRegenCharm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for MP regenerating charm.
/// 
/// NOTES: - Slowly restores MP over time.
/// 
/// TO DO: - Tweak and/or optimize.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MPRegenCharm : Charm {

	Marble marble;	// Reference to marble.
	
	// Initialize - Sets up charm's attributes.
	public override void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "MPRegenCharm");
		
		marble = GameMaster.CreateGM().marble;
	}
	
	// Effect - What the charm does.
	public override void Effect() {
		Debug.Log("Equipped " + type + "!");
		marble.StartCoroutine(RestoreMP());
	}

	// RestoreMP - Gradually restores MP as long as the charm is equipped.
	public IEnumerator RestoreMP() {
		while (equipped) {
			if (marble.marblePower < marble.maxMP) {
				marble.marblePower++;
				Messenger<int, int>.Broadcast("UpdateMarpower", marble.marblePower, marble.maxMP);
			}

			yield return new WaitForSeconds(7);
		}
	}
}
