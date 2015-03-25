/// <summary>
/// SpeedBoostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 24, 2015
/// 
/// Class for speed boost granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SpeedBoostSource : BuffSource {

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = SpeedBoost; // Basically gives the marble the SpeedBoost function to use.
		marble.heldBuff = Marble.PowerUp.SpeedBoost;
	}

	// SpeedBoost - Modifies the marble's speed for a while.
	public void SpeedBoost(float intensity, float duration) {
		marble.buff = Marble.PowerUp.SpeedBoost;
		marble.speedMultiplier = intensity;
	}
}
