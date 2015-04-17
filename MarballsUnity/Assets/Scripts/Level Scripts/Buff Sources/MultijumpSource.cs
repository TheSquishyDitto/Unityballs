/// <summary>
/// MultijumpSource.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Mar. 24, 2015
/// Last Revision: Apr. 16, 2015
/// 
/// Class for multi jump granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class MultijumpSource : BuffSource {

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = MultiJump; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.MultiJump;
	}
	
	// MultiJump - Grants the marble the ability to jump multiple times, even in midair. By default does not expire by time.
	// NOTE: Intensity will be truncated; it is only a float to match the other buff functions.
	public void MultiJump(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.MultiJump;
		marble.jumpFunction = NewJump;

		marble.midairJumps = (int)intensity;
		//marble.maxJumps = (int)intensity;
		//marble.jumpsLeft = (marble.grounded)? marble.maxJumps : marble.maxJumps - 1; // If the marble is in the air, their current jump count will only allow the additional midair jumps.
	}

	// NewJump - Allows marble to jump in midair.
	public void NewJump(){
		if (marble.canJump && marble.midairJumps > 0) {

			Vector3 jumpDir = (marble.grounded)? marble.hit.normal : Vector3.up;

			//marble.marbody.AddForce (marble.jumpHeight * jumpDir);
			marble.marbody.velocity = new Vector3(marble.marbody.velocity.x, 0, marble.marbody.velocity.z) + (jumpDir * (marble.jumpHeight / 100));
			marble.canJump = false;	// This prevents the jump from getting applied multiple times.

			if (!marble.grounded) {
				marble.midairJumps--;

				if (marble.midairJumps == 0) {
					marble.ClearBuffs();
				}
			}

			marble.Invoke("JumpCooldown", 0.25f);	// Forces the marble to wait a moment in midair before allowing it to jump again.
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.midairJumps = 0;
		marble.jumpFunction = null;
	}
	
}
