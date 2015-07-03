/// <summary>
/// HitBox.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 26, 2015
/// Last Revision: Jun. 26, 2015
/// 
/// Class that handles behavior of damaging triggers and colliders.
/// 
/// NOTES: - Can be attached to anything with any collider.
/// 	   - IDamageable interface is at bottom of this class.
/// 
/// TO DO: - Add way to inflict piercing damage (ignores defense). Inherit from this class?
/// 	   - Refine.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class HitBox : MonoBehaviour {

	public int damage = 1;	// How much damage this hitbox inflicts.

	// OnTriggerEnter - Called when an object collides with the trigger.
	void OnTriggerEnter(Collider other) {
		Damage(other);
	}
	
	// OnCollisionEnter - Called when an object collides with the collider.
	void OnCollisionEnter(Collision collision) {
		Damage(collision.collider);
	}

	// Damage - Inflicts damage on another thing. Returns true if it tried to damage something.
	protected virtual bool Damage(Collider other) {
		// Check if what fell into the killzone was a marble, since you wouldn't want a falling box to reset the player.
		if (other.GetComponent<IDamageable>() != null) {
			other.GetComponent<IDamageable>().TakeDamage(damage);
			return true;
		}
		
		return false;
	}
}

// Interface for anything that can take damage.
public interface IDamageable {
	void TakeDamage(int damage);
}
