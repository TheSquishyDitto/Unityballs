﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ScriptedPath : MonoBehaviour {

	// Variables
	#region Variables
	[Range(0, 2)]
	public float speed;				// How fast the object should go between these points.
	public bool loop;				// Whether this object should loop around to its first position.
	public Transform pointParent;	// Optional parent of points for organization.

	[Tooltip("Only applies to ring generation function.")]
	public float ringSize = 1;		// The size of a generated ring.
	public int ringPointCount = 45;	// Number of points to generate for ring.
	
	public List<Transform> points = new List<Transform>();	// The points to go between in order.

	float distance;				// Distance between position and next point.
	Transform myTransform;		// Cached transform.
	Vector3 startPos;			// Initial position.

	#endregion

	// Use this for initialization
	void Start () {
		myTransform = transform;

		if (points.Count > 0 && !points.Contains(null)) {
			myTransform.position = points[0].position;
			startPos = myTransform.position;
			StartCoroutine("Move");
		} else {
			Debug.LogWarning("(ScriptedPath.cs) Point list is empty or contains null elements!");
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	// Move - Coroutine to consistently move between points.
	IEnumerator Move() {
		distance = Vector3.Distance(startPos, points[0].position);	// Get distance between where camera starts at and first point.
		Transform lastPoint = myTransform;	// Sets the last visited point to be the camera's position.

		// For each point as a target destination,
		for (int i = 0; i < points.Count; i++) {

			// Keep moving towards the next point while there is distance to be covered.
			while(distance > speed) {

				myTransform.position += (points[i].position - myTransform.position).normalized * speed;

				// Matches rotation of point gradually as well.
				myTransform.rotation = Quaternion.Lerp(lastPoint.rotation, points[i].rotation, 
				                                       Vector3.Distance(myTransform.position, lastPoint.position) / Vector3.Distance(lastPoint.position, points[i].position));

				distance = Vector3.Distance(myTransform.position, points[i].position); // Refreshes distance.
				yield return new WaitForFixedUpdate();
			}

			lastPoint = points[i]; // Refreshes the last visited point once movement is done.

			// If there are more points, set the distance to the next point.
			if (i < points.Count - 1)
				distance = Vector3.Distance(myTransform.position, points[i + 1].position);

			// Otherwise, if looping is enabled, set the distance from the last point to the first and restart the for loop.
			else if (loop) {
				distance = Vector3.Distance(myTransform.position, points[0].position);
				i = -1;
			}

			//Debug.Log("Reached a point!");
		}
	}

	// CreatePoint - Adds a point with the object's current position and rotation.
	[ContextMenu("Add Point")]
	void CreatePoint() {
		GameObject newPoint = new GameObject("Point " + (points.Count + 1));
		newPoint.transform.position = transform.position;
		newPoint.transform.rotation = transform.rotation;

		if (pointParent) newPoint.transform.parent = pointParent;

		points.Add(newPoint.transform);
	}

	// GenerateRing - Creates a circle of points around the origin, all facing the origin.
	[ContextMenu("Generate Ring")]
	void CreateRing() {
		for (int i = 0; i < ringPointCount; i++) {
			transform.position = new Vector3(ringSize * Mathf.Cos((i * 2 * Mathf.PI)/ringPointCount), transform.position.y, ringSize * Mathf.Sin((i * 2 * Mathf.PI)/ringPointCount));
			transform.LookAt(Vector3.zero);
			CreatePoint();
		}
	}

	// ClearPoints - Clears all points.
	[ContextMenu("Clear All Points")]
	void ClearPonts() {
		foreach (Transform obj in points) {
			if (Application.isPlaying)
				Destroy(obj.gameObject);
			else
				DestroyImmediate(obj.gameObject, false);
		}

		points.Clear();
	}

	// OnDrawGizmos - Draws the points to be pathed between in scene view.
	void OnDrawGizmos() {

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
