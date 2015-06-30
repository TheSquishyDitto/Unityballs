/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira
/// Date Created:  Feb. 13, 2015
/// Last Revision: Jun. 25, 2015
/// 
/// Class that lets the player win at the finish line.
/// 
/// NOTES: - Should tell the GameMaster the player has won.
/// 
/// TO DO: - Tweak behavior until desired.
/// 	   - Add gravity zone as an option for the victory animation!
/// 
/// </summary>

using UnityEngine;
using System.Collections;

public class FinishLine : MonoBehaviour {

	// Variables
	#region Variables
	GameMaster gm;					// Reference to Game Master.
	Settings settings;				// Reference to game settings.
	public static Transform finish;	// Global reference to the location of the active finish line.
	
	public GameObject flames;		// Reference to parent object of flame particles.
	public Transform swirlPoint;	// Reference to the point the marble should swirl around.
	public GameObject arrows;		// Reference to the indicator arrow.
	public AudioClip explodeSound;	// Reference to a sound to use when exploding.
	//public bool gravityFinish;		// Whether crazy Christmas swirl or a black hole-like thing is used.

	Vector3 impactVelocity;			// Holds which direction the marble hit the finish line from.

	#endregion

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM ();
		settings = GameMaster.LoadSettings();
		finish = transform;
		//gm.finishLine = this.transform;
	}

	// OnEnable - Called when the object is enabled.
	void OnEnable() {
		GameMaster.start += FlameOff;
		GameMaster.play += FlameOff;
		GameMaster.win += FlameOn;

		Messenger.AddListener("ToggleArrows", ToggleArrows);
	}

	// OnDisable - Called when the object is disabled.
	void OnDisable() {
		GameMaster.start -= FlameOff;
		GameMaster.play -= FlameOff;
		GameMaster.win -= FlameOn;

		Messenger.RemoveListener("ToggleArrows", ToggleArrows);
	}

	// Start - Use this for initialization
	void Start () {
		if (settings.guideArrows != arrows.activeSelf)
			ToggleArrows();
	}

	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble") && gm.state != GameMaster.GameState.Win) {
			impactVelocity = other.attachedRigidbody.velocity;	// Stores marble's velocity at the time of impact.
			other.attachedRigidbody.isKinematic = (!settings.gravityFinish || settings.simpleAnim);	// Hands total control of marble position to scripts.
			StartCoroutine("SwirlFinish");	// Starts the swirly animation.
			gm.OnWin(); // When player gets to finish they win!
		}	
	}

	// ToggleArrows - Enables/disables helper arrows.
	public void ToggleArrows() {
		arrows.SetActive(!arrows.activeSelf);
	}

	// FlameOn - Spews flames.
	public void FlameOn() {
		flames.SetActive(true);
	}

	// FlameOff - Ceases spewing flames.
	public void FlameOff() {
		flames.SetActive(false);
	}

	// SwirlFinish - Coroutine that makes the marble spiral after crossing the finish line.
	public IEnumerator SwirlFinish () {

		// NOTE: TO CHANGE WHERE THE MARBLE SWIRLS AROUND, MOVE THE "SWIRLPOINT"

		ParticleSystem explosion = null; 	// Reference to explosion particles.

		// [insert code to smoothly get timer into position, maybe separate coroutine? ]

		if (!settings.simpleAnim) {
			if (!settings.gravityFinish) {
				// CRAZY WIN ANIMATION

				// [insert code to smoothly get the marble into the starting position of swirl without teleporting, maybe separate coroutine? ]

				// Spickler Magic
				float theta = Mathf.PI / 50.0f;	// Some random constant that Spickler came up with.
				float spiconstant = 0.1f;		// Another random constant Spickler came up with.

				int swirls = 200;	// Factors into how many full revolutions/swirls the marble does. Bigger number means more swirling.
				int width = 4;		// Factors into radius of swirls. The smaller this number, the bigger the radius.
				int height = 4;		// Factors into how high the marble travels. The bigger this number, the less height.
				int speed = 2;		// Factors into how fast the marble swirls. Bigger numbers = faster. Double digits = Earthquake.
				float gap = 0;		// Factors into the gap between bottom cone and top cone. The bigger the number the bigger the gap.

				// Inverse Christmas Tree Swirl
				for (int i = 0; i < swirls; i++) {
					gm.marble.transform.position = new Vector3 ((theta * i / width) * Mathf.Cos (speed * theta * i),	// The smaller the number being divided by, the bigger the circle.
					                                         (((gap * Mathf.PI + theta * i) / height) + spiconstant), // The greater the number multiplied by pi, the higher the marble goes.
					                                         (theta * i / width) * Mathf.Sin (speed * theta * i))   // The smaller the number being divided by, the bigger the circle.
															+ swirlPoint.transform.position - new Vector3(0, (((gap * Mathf.PI + theta * swirls) / height) + spiconstant), 0);	// Makes this all happen around a given point.
					
					yield return new WaitForFixedUpdate(); // Waits for next fixed update before calling the next loop iteration.
				}

				// Christmas Tree Swirl
				for (int i = swirls; i > 0; i--) {
					gm.marble.transform.position = new Vector3 ((theta * i / width) * Mathf.Cos (speed * theta * -i),	// The smaller the number being divided by, the bigger the circle.
					                                         (((gap * Mathf.PI - theta * i) / height) + spiconstant), // The greater the number multiplied by pi, the higher the marble goes.
					                                         (theta * i / width) * Mathf.Sin (speed * theta * -i))   // The smaller the number being divided by, the bigger the circle.
															+ swirlPoint.transform.position + new Vector3(0, (((gap * Mathf.PI + theta * swirls) / height) + spiconstant), 0);	// Makes this all happen around a given point.

					yield return new WaitForFixedUpdate(); // Waits for next fixed update before calling the next loop iteration.
				}
			
				// Shrinks the marble into oblivion.
				int shrinks = 25; // How many shrink iterations there should be.
				for (int i = 0; i < shrinks; i++) {
					gm.marble.transform.localScale -= new Vector3(1.0f/shrinks, 1.0f/shrinks, 1.0f/shrinks);
					yield return new WaitForFixedUpdate(); // Waits for next fixed update before calling the next iteration.
				}

				// Insane explosion.
				explosion = ((GameObject)Instantiate(Resources.Load ("Prefabs/Particle Prefabs/Explosion"))).GetComponent<ParticleSystem>();
				AudioSource.PlayClipAtPoint(explodeSound, Camera.main.transform.position, 2.0f);
				yield return new WaitForSeconds(2f); // Wait until explosion is partially finished.
			}

		} else {
			// SIMPLE WIN ANIMATION

			// Summons and sets up simple dissipating animation.
			explosion = ((GameObject)Instantiate(Resources.Load ("Prefabs/Particle Prefabs/SimplePoof"))).GetComponent<ParticleSystem>();
			explosion.transform.position = gm.marble.transform.position;	// Puts particle at impact location.
			explosion.startSpeed = impactVelocity.magnitude / 5;	// Makes particles as fast as impact.
			explosion.transform.rotation = Quaternion.LookRotation(impactVelocity);	// Rotates particles in direction of impact.
			explosion.Play();	// Plays the poof.

			// Shrinks the marble into oblivion.
			int shrinks = 100; // How many shrink iterations there should be.
			for (int i = 0; i < shrinks; i++) {
				gm.marble.transform.localScale -= new Vector3(1.0f/shrinks, 1.0f/shrinks, 1.0f/shrinks);
				yield return new WaitForFixedUpdate(); 
			}

			yield return new WaitForSeconds(0.3f);
		}

		//References.hud.StartCoroutine("OnVictory");	// Shows victory screen.
		Messenger.Broadcast("Victory");

		// Destroys particles once they're done.
		if (explosion) {
			yield return new WaitForSeconds(5);
			Destroy(explosion.gameObject);
		}

	}

	// OnDrawGizmos - Used to draw things in scene view exclusively: does not affect gameplay.
	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(swirlPoint.position, new Vector3(1, 1, 1));	// Allows the swirl epicenter to be seen.
	}
}
