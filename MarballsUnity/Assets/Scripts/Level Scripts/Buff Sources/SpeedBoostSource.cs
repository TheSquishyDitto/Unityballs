/// <summary>
/// SpeedBoostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 31, 2015
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

	float speedMultiplier;	// Original speed of marble.

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = SpeedBoost; // Basically gives the marble the SpeedBoost function to use.
		marble.heldBuff = Marble.PowerUp.SpeedBoost;
	}

	// SpeedBoost - Modifies the marble's speed for a while.
	public void SpeedBoost(float intensity, float duration) {
		marble.buff = Marble.PowerUp.SpeedBoost;
		speedMultiplier = marble.speedMultiplier;
		marble.speedMultiplier = intensity;
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.speedMultiplier = speedMultiplier;
		marble.jumpFunction = null;
	}
}
