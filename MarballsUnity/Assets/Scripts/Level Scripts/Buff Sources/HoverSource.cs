/// <summary>
/// HoverSource.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Mar. 24, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class for hover/heliball granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class HoverSource : BuffSource {
	public float maxHeight = 100f;	// Maximum height the marble can travel.

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
		marble.JumpFunction = NewJump;
		marble.MoveFunction = NewMove;
	}
	
	// NewJump - Allows marble to float in midair.
	public void NewJump(){			
			Vector3 jumpDir = Vector3.up;
			
			if(marble.marform.position.y <= maxHeight)
				marble.marbody.AddForce (-Physics.gravity.y * jumpDir);
	}
	

	// NewMove - Should allow mid-air movement only (no ground movement).
	public void NewMove(){
		// Constant force.
		marble.marbody.AddForce((Vector3.up * -Physics.gravity.y * 2) / 3.0f);

		// Applies force if marble is in the air
		if (!marble.Grounded) {
			marble.marbody.drag = 0.5f;
			marble.marbody.AddForce(15 * marble.Direction * marble.SpeedMultiplier * marble.Shackle, ForceMode.Impulse);
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.JumpFunction = null;
		marble.MoveFunction = null;
	}
}
