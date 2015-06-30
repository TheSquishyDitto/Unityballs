/// <summary>
/// MultijumpSource.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Mar. 24, 2015
/// Last Revision: Jun. 26, 2015
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

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
		marble.JumpFunction = NewJump;
		marble.MidairJumps = (int)intensity;
	}

	// NewJump - Allows marble to jump in midair.
	// NOTE - Current implementation gives marble a boost of speed when jumping with directional input.
	public void NewJump(){
		if (marble.CanJump && marble.MidairJumps > 0) {

			Vector3 jumpDir = (marble.Grounded)? marble.GroundInfo.normal : Vector3.up * 2;
			if (!marble.Grounded) jumpDir += marble.Direction;	// Allows jumps in this state to be more directionally influenced.
			jumpDir = jumpDir.normalized;

			//marble.marbody.AddForce (marble.jumpHeight * jumpDir);
			marble.marbody.velocity = new Vector3(marble.marbody.velocity.x, 0, marble.marbody.velocity.z) + (jumpDir * (marble.JumpHeight / 100));
			marble.CanJump = false;	// This prevents the jump from getting applied multiple times.

			if (!marble.Grounded) {
				marble.MidairJumps--;

				if (marble.MidairJumps == 0) {
					marble.ClearBuffs();
				}
			}

			marble.mover.Invoke("JumpCooldown", 0.3f);	// Forces the marble to wait a moment in midair before allowing it to jump again.
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.MidairJumps = 0;
		marble.JumpFunction = null;
	}
	
}
