using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable {

	public int health = 5;
	public int defense = 0;

	public bool invulnerable;

	public void TakeDamage(int damage) {
		if (!invulnerable) {
			if (damage - defense > 0) {
				health -= damage - defense;
				
				if (health <= 0)
					gameObject.SetActive(false);
				else {
					GetComponent<Renderer>().material.color = Color.red;
					invulnerable = true;
					
					Invoke("RevokeArmor", 0.5f);
				}
			}
		}
	}

	public void RevokeArmor() {
		invulnerable = false;
		GetComponent<Renderer>().material.color = Color.white;
	}

}
