/// <summary>
/// HoverSource.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Mar. 24, 2015
/// Last Revision: May  10, 2015
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
	public float maxHeight = 100f;	// Maximum height the ball can travel
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = HeliBall; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.HeliBall;
	}
	
	// SuperJump - Modifies marble's jumping height.
	public void HeliBall(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.HeliBall;
		
		//ConstantForce hover = marble.gameObject.AddComponent<ConstantForce>();
		//hover.force = new Vector3(0,20,0);
		marble.jumpFunction = NewJump;
		marble.moveFunction = NewMove;
	}
	
	// NewJump - Allows marble to float in midair.
	public void NewJump(){			
			Vector3 jumpDir = Vector3.up;
			
			if(marble.transform.position.y <= maxHeight)
				marble.marbody.AddForce (-Physics.gravity.y * jumpDir);
	}
	

	// NewMove - Should allow mid-air movement.
	public void NewMove(){
		// Constant force.
		marble.marbody.AddForce((Vector3.up * -Physics.gravity.y * 2) / 3.0f);

		// Applies force if marble is in the air
		if (!marble.grounded) {
			marble.marbody.drag = 0.5f;
			marble.marbody.AddForce(15 * marble.inputDirection * marble.speedMultiplier * marble.shackle, ForceMode.Impulse);
			marble.inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
		}
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		//if (marble.GetComponent<ConstantForce>())
		//	Destroy (marble.GetComponent<ConstantForce>());

		marble.jumpFunction = null;
		marble.moveFunction = null;
	}
}
