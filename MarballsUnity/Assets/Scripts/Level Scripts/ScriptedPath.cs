/// <summary>
/// ScriptedPath.cs
/// Authors: Kyle Dawson, Charlie Sun
/// Date Created:  Apr. 19, 2015
/// Last Revision: July 24, 2015
/// 
/// Class for moving and rotating any object over time to specific locations.
/// 
/// NOTES: - Point generation breaks prefab instance!
/// 
/// TO DO: - Tweak until behaves as desired.
/// 	   - Add more options to make path creation nicer.
/// 
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptedPath : MonoBehaviour {
	
	// Variables
	#region Variables
	[Range(0, 2)]
	public float speed = 0.2f;		// How fast the object should go between these points.
	public float waitTime = 0;		// How long object should dwell at a point before moving on.
	public bool loop;				// Whether this object should loop around to its first position.
	public Transform pointParent;	// Optional parent of points for organization.

	[Tooltip("Only applies to ring generation function.")]
	public float ringSize = 100;	// The size of a generated ring.
	public int ringPointCount = 45;	// Number of points to generate for ring.
	
	public List<Transform> points = new List<Transform>();	// The points to go between in order.
	
	Transform myTransform;			// Cached transform.

	#endregion

	// Awake - Called before anything else.
	protected virtual void Awake() {
		myTransform = transform;
	}

	// Start - Use this for initialization.
	protected virtual void Start () {		
		if (points.Count > 0 && !points.Contains(null)) {
			myTransform.position = points[0].position;
			StartCoroutine("Move");
		} else {
			Debug.LogWarning("(ScriptedPath.cs) Point list is empty or contains null elements!");
		}
	}
	
	// Move - Coroutine to consistently move between points.
	protected IEnumerator Move() {
		Transform lastPoint = myTransform;	// Sets the last visited point to be the camera's position.
		lastPoint.rotation = Quaternion.identity;

		// For each point as a target destination,
		for (int i = 0; i < points.Count; i++) {

			// Keep moving towards the next point while there is distance to be covered.
			while(myTransform.position != points[i].position) {
				myTransform.position = Vector3.MoveTowards(myTransform.position, points[i].position, speed);

				// Matches rotation of point gradually as well.
				//Debug.Log(myTransform.position);
				//Debug.Log(myTransform.rotation);
				//Debug.Log("LastPoint Pos: " + lastPoint.position + " - Rot: " + lastPoint.rotation);
				//Debug.Log("NextPoint Pos: " + points[i].position + " - Rot: " + points[i].rotation);
				myTransform.rotation = Quaternion.Lerp(lastPoint.rotation, points[i].rotation, 
				                                       Vector3.Distance(myTransform.position, lastPoint.position) / Vector3.Distance(lastPoint.position, points[i].position));

				yield return new WaitForFixedUpdate();
			}

			yield return new WaitForSeconds(waitTime);

			lastPoint = points[i]; // Refreshes the last visited point once movement is done.

			// Otherwise, if looping is enabled, set the distance from the last point to the first and restart the for loop.
			if (loop && i == points.Count - 1) {
				i = -1;
				yield return new WaitForFixedUpdate(); // Prevent infinite loops without any pause.
			}

			//Debug.Log("Reached a point!");
		}
	}

#if UNITY_EDITOR
	// CreatePoint - Adds a point with the object's current position and rotation.
	[ContextMenu("Add Point")]
	protected void CreatePoint() {
		GameObject newPoint = new GameObject("Point " + (points.Count + 1));
		newPoint.transform.position = transform.position;
		newPoint.transform.rotation = transform.rotation;

		if (pointParent) newPoint.transform.parent = pointParent;

		points.Add(newPoint.transform);
	}

	// GenerateRing - Creates a circle of points around the origin, all facing the origin.
	[ContextMenu("Generate Ring")]
	protected void CreateRing() {
		for (int i = 0; i < ringPointCount; i++) {
			transform.position = new Vector3(ringSize * Mathf.Cos((i * 2 * Mathf.PI)/ringPointCount), transform.position.y, ringSize * Mathf.Sin((i * 2 * Mathf.PI)/ringPointCount));
			transform.LookAt(Vector3.zero);
			CreatePoint();
		}
	}

	// ClearPoints - Clears all points.
	[ContextMenu("Clear All Points")]
	protected void ClearPonts() {
		foreach (Transform obj in points) {
			if (Application.isPlaying)
				Destroy(obj.gameObject);
			else
				DestroyImmediate(obj.gameObject, false);
		}

		points.Clear();
	}
#endif

	// OnDrawGizmos - Draws the points to be pathed between in scene view.
	protected void OnDrawGizmos() {

		if (points.Count > 0) {

			//if (points[0] != null && !Application.isPlaying) transform.position = points[0].position;

			// Draw cyan lines to show point rotations.
			Gizmos.color = Color.cyan;
			for (int i = 0; i < points.Count; i++) {
				if (points[i] != null) {
					Gizmos.DrawLine(points[i].position, points[i].position + (points[i].forward * 2));
				}
			}

			// Draw points and connecting lines; redder near beginning, blue near end.
			for (int i = 0; i < points.Count; i++) {
				Gizmos.color = Color.Lerp(Color.red, Color.blue, ((float)i)/points.Count);
				if (points[i] != null) {

					// Draws lines between the points.
					if (i + 1 < points.Count && points[i + 1] != null) {
						Gizmos.DrawLine(points[i].position, points[i+1].position);
					} else if (loop && points[0] != null) {
						Gizmos.DrawLine(points[i].position, points[0].position);
					}

					// Draws the actual points.
					Gizmos.DrawSphere(points[i].position, 0.5f);
				}
			}
		}
	}
}
