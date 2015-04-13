/// <summary>
/// GravityZone.cs
/// Authors: Kyle Dawson
/// Date Created:  Apr.  9, 2015
/// Last Revision: Apr. 12, 2015
/// 
/// Generic class for areas that have modified gravity.
///  
/// NOTES: - May become laggy if multiple objects are stuck colliding in the center constantly.
/// 	   - As it is currently, this script is usable in any project, not just Marballs.
/// 
/// TO DO: - Tweak behavior until desired.
/// 	   - Finetune behavior when object has reached the center of the gravity source.
/// 	   - Add editor script so it performs more nicely in the inspector.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class GravityZone : MonoBehaviour {

	// Enum for how gravity should affect rigidbody.
	public enum GravityType {
		Linear,		// Gravity will add force to object in a single direction.
		Radial		// Gravity will add force to object around a point.
	 }
	// Use the following link to figure out how to make different things show up based on enum choice.
	// http://answers.unity3d.com/questions/417837/change-inspector-variables-depending-on-enum.html

	// Enum for whether the "gravity" should push or pull.
	public enum ForceType {
		Push = -1,
		Pull = 1
	}

	// Variables
	#region Variables
	public GravityType type;		// What type of gravity zone this is.
	[Tooltip("Direction of linear force. Linear only.")]
	public Vector3 direction;		// Direction of linear force. Should only show up if linear is chosen.
	[Tooltip("Center of radial force. Radial only.")]
	public Vector3 center;			// Center of radial force. Should only show up if radial is chosen.
	public bool usePosition = true;	// Whether object's transform position should be the radial center.

	public Collider trigger;			// Reference to the trigger zone that's being used.
	public float gravityStrength = 50;	// How strong the force is.
	public float attenuation = 1;		// How much distance affects the strength of the force.

	[Range(0, 1)]
	public float massFactor = 1;		// How much mass should factor into the calculation.
	public ForceType force = ForceType.Pull; // Whether the gravity should pull or push. Attractive vs. Repulsive.

	public float distance;	// Distance between object being pulled and the source of the gravity. Only public for debugging.
	protected Vector3 dir;			// Which direction the force is currently going in.
	protected Ray ray;				// Ray used to determine linear gravity collider end.
	protected RaycastHit hit;			// Raycast storage.

	#endregion

	// Use this for initialization
	void Start () {

	}
	
	// Update - Called once per frame
	protected void Update () {
		if (usePosition) center = transform.position;	// Updates position of center if zone is moving.
	}

	// OnTriggerStay - Called every frame an object is inside the gravity zone.
	protected void OnTriggerStay(Collider other) {
		if (other.attachedRigidbody) {

			// If the force changes with distance, performs calculations to find the distance.
			if (attenuation > 0) {
				// If linear, finds a point on the collider to use to compare distance.
				if (type == GravityType.Linear) {
					ray = new Ray(other.transform.position, direction); // Creates a ray from the inside-out.
					ray.origin = ray.GetPoint(3000);		// Finds a point somewhere far out and then,
					ray.direction = -ray.direction;			// Reverses the ray so it can actually hit something.
					trigger.Raycast(ray, out hit, 9001);	// Then casts the ray to find the point.
				}

				// Measures distance.
				distance = (type == GravityType.Radial)? Vector3.Distance(center, other.transform.position) :
														 Vector3.Distance(hit.point, other.transform.position);
			} else {
				distance = 1;
			}

			// Determines direction of next force application.
			dir = (type == GravityType.Linear)? direction : center - other.transform.position;

			// Applies gravity.
			Gravity(other.attachedRigidbody);
		}
	}

	// Gravity - Applies force to object in desired direction.
	protected virtual void Gravity(Rigidbody body) {
		if (distance > 0)
			body.AddForce((dir.normalized
			                / Mathf.Max(1f, Mathf.Pow(distance, attenuation)))
			                * gravityStrength
			              	* Mathf.Pow(body.mass, massFactor)
			              	* (int)force);
	}
}
