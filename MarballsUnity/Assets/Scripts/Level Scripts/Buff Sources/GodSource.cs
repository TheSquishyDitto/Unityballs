/// <summary>
/// GodSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 18, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class for debug god granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>


using UnityEngine;
using System.Collections;

public class GodSource : BuffSource {

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
		//gameObject.SetActive(gm.debug);
	}

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction () {
		marble.JumpFunction = NewJump;
		marble.MoveFunction = NewMove;
	}
	
	// NewJump - Allows marble to float in midair.
	public void NewJump(){			
		Vector3 jumpDir = (marble.Grounded)? marble.GroundInfo.normal : Vector3.up;

		marble.marbody.velocity = new Vector3(marble.marbody.velocity.x, 0, marble.marbody.velocity.z) + (jumpDir * (marble.JumpHeight / 100));
	}
	
	
	// NewMove - Should allow mid-air movement.
	public void NewMove(){

		// Spins marble to appropriate amount of spin speed.
		marble.marbody.AddTorque(Vector3.Cross(Vector3.up, marble.Direction) * marble.SpeedMultiplier * marble.RevSpeed * marble.Shackle);

		// Applies force if marble is on the ground.
		marble.marbody.drag = 0.5f;
		marble.marbody.AddForce(marble.Direction * marble.SpeedMultiplier * marble.marbody.angularVelocity.magnitude * marble.Shackle, ForceMode.Impulse);
	}
	
	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();

		marble.JumpFunction = null;
		marble.MoveFunction = null;
	}

}
