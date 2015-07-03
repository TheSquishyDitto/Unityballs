/// <summary>
/// SizeChangeSource.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Feb. 23, 2015
/// Last Revision: Jun. 26, 2015
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
	float mass;		// Marble's original mass.

	// BuffFunction - Applies the buff to the marble.
	protected override void BuffFunction() {
		mass = marble.marbody.mass;
		marble.marbody.mass *= Mathf.Pow(intensity, 4);
		marble.marform.localScale *= intensity; // alternative is Vector3.one * intensity, current version multiplies marble form's normal size
		marble.MoveFunction = NewMove;
	}

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		marble.marform.localScale = Vector3.one * marble.data.size;
		marble.marbody.mass = mass;
		marble.MoveFunction = null;
	}
	
	// NewMove - Compensate for larger mass
	public void NewMove(){
		
		marble.marbody.AddTorque(Vector3.Cross(Vector3.up, marble.Direction) * marble.SpeedMultiplier * marble.RevSpeed * marble.Shackle, ForceMode.Acceleration);

		if (marble.Grounded) {
			marble.marbody.drag = 0.5f;
			marble.marbody.AddForce(marble.Direction * marble.SpeedMultiplier * marble.marbody.angularVelocity.magnitude * marble.Shackle, ForceMode.VelocityChange);
		}
		else {
			marble.marbody.drag = 0.1f;
		}
	}

}
