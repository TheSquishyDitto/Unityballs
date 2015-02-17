/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Feb. 16, 2015
/// 
/// Class that lets the player win at the finish line.
/// 
/// NOTES: - Should tell the GameMaster the player has won.
/// 
/// TO DO: - Tweak behavior until desired.
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	public GameMaster gm;

	void Awake () {
		gm = GameMaster.CreateGM ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble"))
			gm.OnWin(); // When player gets to finish they win!
	}
}
