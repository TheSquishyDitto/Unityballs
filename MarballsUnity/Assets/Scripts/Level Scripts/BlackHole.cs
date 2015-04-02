/// <summary>
/// BlackHole.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  1, 2015
/// Last Revision: Apr.  1, 2015
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

public class BlackHole : MonoBehaviour {

	public float gravityPull = 1000;		// How strong the gravitational pull should be.
	public float attenuation = 1;			// How much weaker the gravity should be at a distance.
	public SphereCollider deathSphere;		// Reference to killzone trigger collider.
	float distance;							// Distance between object being pulled and the black hole.

	// Start - Use this for initialization.
	void Start () {

	}
	
	// Update - Called once per frame.
	void Update () {
	
	}

	// OnTriggerStay - Called every frame that an object is inside the black hole's range of influence.
	void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody) {
			distance = Vector3.Distance(transform.position, other.transform.position);	// Measure distance between object and hole.

			// If outside of the black hole, draw objects towards it.
			if (distance > deathSphere.radius * transform.localScale.x + 1) {
				other.attachedRigidbody.AddForce(((transform.position - other.transform.position).normalized
				                                 / Mathf.Pow(distance, attenuation))
				                                 * gravityPull);

			// Otherwise, stick them inside.
			} else {
				other.transform.position = transform.position;
				other.attachedRigidbody.velocity = Vector3.zero;
			}
		}
	}
}
