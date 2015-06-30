/// <summary>
/// BombCharm.cs
/// Authors: Kyle Dawson
/// Date Created:  Jun. 28, 2015
/// Last Revision: Jun. 29, 2015
/// 
/// Class for the bomb granting charm.
/// 
/// NOTES: - Grants Explosion ability.
/// 
/// TO DO: - Tweak and/or optimize.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BombCharm : Charm {

	Marble marble;	// Reference to marble.
	Ability bomb;	// Dash ability.
	
	// Initialize - Sets up charm's attributes.
	public override void Initialize() {
		data = (CharmData)Resources.Load(dataPath + "BombCharm");
		
		marble = GameMaster.CreateGM().marble;
		bomb = new Ability("Explode", 2, 2, 8, data.icon, () => { marble.StartCoroutine(Explode()); });
	}
	
	// Effect - What the charm does.
	public override void Effect() {
		Debug.Log("Equipped " + type + "!");
		marble.abilities.Add(bomb);
		Messenger.Broadcast("UpdateAbility");
	}
	
	// UnEffect - Reverses what the charm does.
	public override void UnEffect() {
		Debug.Log("Unequipped " + type + "!");
		marble.abilities.Remove(bomb);
		marble.abilityIndex = Mathf.Min(marble.abilityIndex, Mathf.Max(marble.abilities.Count - 1, 0));
		Messenger.Broadcast("UpdateAbility");
	}
	
	// Explode - Makes the marble explode.
	public IEnumerator Explode() {
		float radius = 5;

		Object.Instantiate(Resources.Load("Prefabs/Particle Prefabs/Explosion"), marble.marform.position, Quaternion.identity);

		Collider[] colliders = Physics.OverlapSphere(marble.marform.position, radius); // Find every collider within range.

		yield return new WaitForSeconds(1);	// Length of energy buildup.

		// For every collider in range...
		for (int i = 0; i < colliders.Length; i++) {
			// If it isn't the marble,
			if (colliders[i].gameObject != marble.gameObject) {
				// Blow it the hell up, yeehaw.
				if (colliders[i].GetComponent<Rigidbody>() != null) {
					colliders[i].GetComponent<Rigidbody>().AddExplosionForce(50, marble.marform.position, radius, 1, ForceMode.Impulse);
				}

				if (colliders[i].GetComponent<IDamageable>() != null) {
					colliders[i].GetComponent<IDamageable>().TakeDamage(6);
				}
			}
		}
	}
}
