﻿/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Feb. 23, 2015
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
	public ParticleSystem flame1;
	public ParticleSystem flame2;
	public const float GRAV_CONST = .00667f;
	public Transform marble;
	public Transform pointMass;
	
	void Awake () {
		gm = GameMaster.CreateGM ();
		gm.finishLine = this.transform;
	}

	// Use this for initialization
	void Start () {
		marble = gm.marble;
		//pointMass = pointMass.transform.position.sqrMagnitude;
	}

	// Update is called once per frame
	void Update () {
		
	}
	
	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {
			gm.OnWin(); // When player gets to finish they win!
		}	
	}

	// FlameOn - Spews flames.
	public void FlameOn() {
		flame1.Play();
		flame2.Play();
	}

	// FlameOff - Ceases spewing flames.
	public void FlameOff() {
		flame1.Stop();
		flame2.Stop();
	}

	// SwirlFinish - makes the marble swirl near the finish line upon crossing
	public void SwirlFinish () {
		float sqMag = marble.transform.position.sqrMagnitude;
		//float gravEq = ((GRAV_CONST * 
	}
}
