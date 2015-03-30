/// <summary>
/// MultijumpSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Mar. 24, 2015
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

public class MultijumpSource : BuffSource {

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = MultiJump; // Basically gives the marble the SuperJump function to use.
		marble.heldBuff = Marble.PowerUp.MultiJump;
	}
	
	// MultiJump - Grants the marble the ability to jump multiple times, even in midair. By default does not expire by time.
	// NOTE: Intensity will be truncated; it is only a float to match the other buff functions.
	public void MultiJump(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.MultiJump;
		marble.jumpFunction = NewJump;
		marble.maxJumps = (int)intensity;
		marble.jumpsLeft = (marble.grounded)? marble.maxJumps : marble.maxJumps - 1; // If the marble is in the air, their current jump count will only allow the additional midair jumps.
	}
	
	public void NewJump(){
		if (marble.jumpsLeft > 0 && !marble.hasJumped)
			marble.hasJumped = true;
	}
	
}
