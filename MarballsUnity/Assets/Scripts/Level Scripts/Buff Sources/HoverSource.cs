﻿using UnityEngine;
using System.Collections;

public class HoverSource : BuffSource {

	// Initialize - Any initialization the given source should have should be done here.
	protected override void Initialize() {
		duration = Mathf.Infinity;
	}
	
	// GiveBuff - Gives a specific buff to the specified marble.
	protected override void GiveBuff(Marble marble) {
		base.GiveBuff(marble);
		marble.buffFunction = HeliBall; // Basically gives the marble the SuperJump function to use.
		marble.heldBuff = Marble.PowerUp.HeliBall;
	}
	
	// SuperJump - Modifies marble's jumping height.
	public void HeliBall(float intensity, float duration = Mathf.Infinity) {
		marble.buff = Marble.PowerUp.HeliBall;
		
		ConstantForce hover = marble.gameObject.AddComponent<ConstantForce>();
		hover.force = new Vector3(0,20,0);
	}
}
