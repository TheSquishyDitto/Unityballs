/// <summary>
/// SuperJumpSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 24, 2015
/// 
/// Class for super jump granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SuperJumpSource : BuffSource {

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = SuperJump; // Basically gives the marble the SuperJump function to use.
		marble.heldBuff = Marble.PowerUp.SpeedBoost;
	}

	// SuperJump - Modifies marble's jumping height.
	public void SuperJump(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.SuperJump;
		marble.jumpHeight *= intensity;
	}
}
