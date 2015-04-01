/// <summary>
/// SuperJumpSource.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 31, 2015
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

	float jumpHeight;	// Marble's original jump height.

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = SuperJump; // Basically gives the marble the SuperJump function to use.
		marble.heldBuff = Marble.PowerUp.SuperJump;
	}

	// SuperJump - Modifies marble's jumping height.
	public void SuperJump(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.SuperJump;
		jumpHeight = marble.jumpHeight;
		marble.jumpHeight *= intensity;
		marble.canJump = true;
		marble.jumpFunction = NewJump;
	}

	// NewJump - Allows the marble to perform a single powerful jump regardless of conditions.
	public void NewJump(){
		if (marble.canJump) {
			Vector3 jumpDir = (marble.grounded)? marble.hit.normal : Vector3.up;
			
			marble.marbody.AddForce (marble.jumpHeight * jumpDir);
			marble.canJump = false;	// This prevents the jump from getting applied multiple times.

			marble.ClearBuffs();
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.jumpHeight = jumpHeight;
		marble.jumpFunction = null;
	}
}
