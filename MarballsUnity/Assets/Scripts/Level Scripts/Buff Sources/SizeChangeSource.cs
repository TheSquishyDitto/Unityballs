/// <summary>
/// SizeChangeSource.cs
/// Authors: Kyle Dawson
/// Date Created:  Feb. 23, 2015
/// Last Revision: Mar. 31, 2015
/// 
/// Class for size change granting entities.
/// 
/// NOTES: - See buff source for implementation information.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class SizeChangeSource : BuffSource {

	//float size;	// Marble's original size.

	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = SizeChange; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.SizeChange;
	}

	// SizeChange - Modifies marble's size.
	public void SizeChange(float newSize, float duration) {
		marble.buff = Marble.PowerUp.SizeChange;
		//size = marble.defSize;
		marble.transform.localScale *= newSize; //new Vector3(newSize, newSize, newSize);
		marble.marbody.mass *= intensity * 10;
		marble.moveFunction = NewMove;
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.transform.localScale = Vector3.one * marble.defSize;
		marble.marbody.mass = marble.defMass;
		marble.moveFunction = null;
	}
	
	// NewMove - Compensate for larger mass
	public void NewMove(){
		
		marble.marbody.AddTorque(Vector3.Cross(Vector3.up, marble.inputDirection) * marble.speedMultiplier * marble.revSpeed * marble.shackle, ForceMode.Acceleration);
		
		if (marble.grounded) {
			marble.marbody.drag = 0.5f;
			marble.marbody.AddForce(marble.inputDirection * marble.speedMultiplier * marble.marbody.angularVelocity.magnitude * marble.shackle, ForceMode.VelocityChange);
			marble.inputDirection = Vector3.zero; // Clears direction so force doesn't accumulate even faster.
		}
		else {
			marble.marbody.drag = 0.1f;
		}
	}

}
