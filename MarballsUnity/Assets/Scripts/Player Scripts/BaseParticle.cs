/// <summary>
/// BaseParticle.cs
/// Authors: Kyle Dawson, [ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 11, 2015
/// Last Revision: Feb. 11, 2015
/// 
/// Class that simply follows the marble and plays particle effects as deemed appropriate.
/// 
/// NOTES: - If you want particles to trail behind the marble, change the Particle System "Simulation Space" to World.
/// 	   - This will likely be the base class for special particles to inherit from.
/// 
/// TO DO: - Add inheriting particle classes and the conditions that trigger them.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class BaseParticle : MonoBehaviour {

	public GameMaster gm;		// Reference to Game Master.
	public Transform marble;	// Reference to child marble.

	// Awake - Called before anything else. Use this to find the Game Master and tell it this exists.
	void Awake () {
		gm = GameMaster.CreateGM();
	}

	// Start - Use this for initialization
	void Start () {
		marble = gm.marble;
	}
	
	// Update - Called once per frame.
	void Update () {
		transform.position = marble.position;
	}
}
