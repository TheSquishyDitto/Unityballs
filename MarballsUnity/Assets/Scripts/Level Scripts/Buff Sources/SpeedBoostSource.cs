/// <summary>
/// SpeedBoostSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Jun. 26, 2015
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

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
		speedMultiplier = marble.SpeedMultiplier;
		marble.SpeedMultiplier = intensity;
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.SpeedMultiplier = speedMultiplier;
	}
}
