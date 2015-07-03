/// <summary>
/// SuperJumpSource.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 23, 2015
/// Last Revision: Jun. 26, 2015
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

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
		jumpHeight = marble.JumpHeight;
		marble.JumpHeight *= intensity;
		marble.CanJump = true;
		marble.JumpFunction = NewJump;
	}

	// NewJump - Allows the marble to perform a single powerful jump regardless of conditions.
	public void NewJump(){
		if (marble.CanJump) {
			Vector3 jumpDir = (marble.Grounded)? marble.GroundInfo.normal * 2 : Vector3.up * 2;
			jumpDir += marble.Direction * Mathf.Clamp(1 / (marble.marbody.velocity.magnitude + 0.001f), 0, 1);	// Allows jumps in this state to be more directionally influenced.
			jumpDir = jumpDir.normalized;

			marble.marbody.AddForce (marble.JumpHeight * jumpDir);
			marble.CanJump = false;	// This prevents the marble from immediately using its original jump afterwards.
			marble.mover.Invoke("JumpCooldown", 1f);
			marble.ClearBuffs();
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.JumpHeight = jumpHeight;
		marble.JumpFunction = null;
	}
}
