/// <summary>
/// GodSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr. 18, 2015
/// Last Revision: Apr. 18, 2015
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
		gameObject.SetActive(gm.debug);
	}

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = GodBall; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.God;
	}
	
	// SuperJump - Modifies marble's jumping height.
	public void GodBall(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.God;

		marble.jumpFunction = NewJump;
		marble.moveFunction = NewMove;
	}
	
	// NewJump - Allows marble to float in midair.
	public void NewJump(){			
		Vector3 jumpDir = (marble.grounded)? marble.hit.normal : Vector3.up;

		marble.marbody.velocity = new Vector3(marble.marbody.velocity.x, 0, marble.marbody.velocity.z) + (jumpDir * (marble.jumpHeight / 100));
	}
	
	
	// NewMove - Should allow mid-air movement.
	public void NewMove(){

		// Spins marble to appropriate amount of spin speed.
		marble.marbody.AddTorque(Vector3.Cross(Vector3.up, marble.inputDirection) * marble.speedMultiplier * marble.revSpeed * marble.shackle);
		
		// Applies force if marble is on the ground.
		marble.marbody.drag = 0.5f;
		marble.marbody.AddForce(marble.inputDirection * marble.speedMultiplier * marble.marbody.angularVelocity.magnitude * marble.shackle, ForceMode.Impulse);
		marble.inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
	}
	
	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.jumpFunction = null;
		marble.moveFunction = null;
	}

}
