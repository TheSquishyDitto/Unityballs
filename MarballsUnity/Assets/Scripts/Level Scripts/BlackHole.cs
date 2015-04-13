/// <summary>
/// BlackHole.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr. 13, 2015
/// 
/// Class for deadly sphere that sucks in other objects.
///  
/// NOTES: - May become laggy if multiple objects are stuck colliding in the center constantly.
/// 	   - As it is currently, this script is usable in any project, not just Marballs.
/// 
/// TO DO: - Tweak behavior until desired.
/// 	   - Make non-marble objects get destroyed when they touch it.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BlackHole : GravityZone {
	
	public SphereCollider eventHorizon;		// Reference to point where marble is stuck in center.

	// Gravity - Drags everything into the event horizon.
	protected override void Gravity(Rigidbody body) {
		if (distance > eventHorizon.radius * transform.localScale.x + 1) {
			body.AddForce((dir.normalized
			               / Mathf.Max(1f, Mathf.Pow(distance, attenuation)))
			              * gravityStrength
			              * Mathf.Pow(body.mass, massFactor)
			              * (int)force);
		} else {
			body.transform.position = transform.position;
			body.velocity = Vector3.zero;
		}
	}
}
