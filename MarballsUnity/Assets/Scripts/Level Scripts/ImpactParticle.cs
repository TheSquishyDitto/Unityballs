/// <summary>
/// ImpactParticle.cs
/// Authors: Kyle Dawson
/// Date Created:  May   6, 2015
/// Last Revision: May   6, 2015
/// 
/// Class for creating particles when hitting something.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class ImpactParticle : MonoBehaviour {

	public GameObject impact;	// Particle to be used.

	// OnCollisionEnter - Called when a collision is detected.
	void OnCollisionEnter(Collision collision) {
		Destroy(Instantiate(impact, collision.transform.position, Quaternion.identity), 5);
	}
}
