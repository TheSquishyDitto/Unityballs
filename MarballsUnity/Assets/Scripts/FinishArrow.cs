/// <summary>
/// FinishArrow.cs
/// Authors: Kyle Dawson
/// Date Created:  Mar. 23, 2015
/// Last Revision: Mar. 23, 2015
/// 
/// Class that controls the finish line indicator's behavior.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FinishArrow : MonoBehaviour {

	[Tooltip("Higher numbers restrict movement and vice versa.")] // <- This lets you add tooltips to the Unity inspector!
	public float heightDampen = 1;	// Movement range limiter.
	[Tooltip("How fast the indicator should move.")]
	public float speed = 3;			// Speed of arrow.

	Vector3 startPos;	// Reference to starting position.

	// Start - Use this for initialization.
	void Start () {
		startPos = transform.position;
	}
	
	// Update - Called once per frame.
	void Update () {
		// Gently bob the arrow up and down.
		transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * speed) / Mathf.Max(0.01f, heightDampen), 0);
	}
}
