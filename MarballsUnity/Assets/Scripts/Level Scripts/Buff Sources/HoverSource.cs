/// <summary>
/// HoverSource.cs
/// Authors: Kyle Dawson, Charlie Sun, Chris Viqueira
/// Date Created:  Mar. 24, 2015
/// Last Revision: Apr.  3, 2015
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

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = HeliBall; // Basically gives the marble the buff function to use.
		marble.heldBuff = Marble.PowerUp.HeliBall;
	}
	
	// SuperJump - Modifies marble's jumping height.
	public void HeliBall(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.HeliBall;
		
		ConstantForce hover = marble.gameObject.AddComponent<ConstantForce>();
		hover.force = new Vector3(0,20,0);
	}

	// NewJump - Should allow continuous ascension while jump is held.

	// NewMove - Should allow mid-air movement.

	// TakeBuff - Any special conditions that must be fixed to remove the buff.
	protected override void TakeBuff() {
		base.TakeBuff();
		
		if (marble.GetComponent<ConstantForce>())
			Destroy (marble.GetComponent<ConstantForce>());

		marble.jumpFunction = null;
	}
}
