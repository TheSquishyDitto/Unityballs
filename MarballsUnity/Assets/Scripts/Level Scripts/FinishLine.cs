/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Mar.  9, 2015
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

	// Variables
	#region Variables
	public GameMaster gm;			// Reference to Game Master.
	public ParticleSystem flame1;	// Reference to first flame particles.
	public ParticleSystem flame2;	// Reference to second flame particles.
	public Transform marble;		// Reference to the marble.
	public Transform swirlPoint;	// Reference to the point the marble should swirl around.

	//int blah = 5;	// Overly cheap counter to call something only every few frames. Should be replaced.
	//int posUpdate = 600;	// One attempt at getting swirl to work.
	//float timeSinceWin = 500; 	// Modified time since player won. Currently counts backwards.
	//public const float GRAV_CONST = .00667f;	// A gravitational constant. May not be needed now.

	#endregion

	// Awake - Called before anything else.
	void Awake () {
		gm = GameMaster.CreateGM ();
		gm.finishLine = this.transform;
	}

	// Start - Use this for initialization
	void Start () {
		marble = gm.marble;
	}

	// Update is called once per frame
	void Update () {

		// CHRIS, I was messing with this a bit and figured out that Spickler's code creates the Christmas tree
		// from top to bottom, rather than vice versa. I changed where we started from to make it do what we want.
		// I made a coroutine and moved it into its own function. We can tweak it how we want it from there.

		//Debug.Log (blah);
		//if (gm.state == GameMaster.GameState.Win) {
			//if (timeSinceWin > 0)
				//timeSinceWin -= Time.deltaTime * 100;

			/*blah--;
			if(blah == 0){
				blah = 5;
				float theta = Mathf.PI / 50.0f;
				
				// Spickler Magic
				marble.transform.position = new Vector3 ((theta * timeSinceWin / 5) * Mathf.Cos (5 * theta * timeSinceWin),
				                                        (((2 * Mathf.PI - theta * timeSinceWin) / 2.0f) + (1 / 10.0f)),
				                                        (theta * timeSinceWin / 5) * Mathf.Sin (5 * theta * timeSinceWin)) + swirlPoint.transform.position;

				//Debug.Log("TimeSinceWin: " + timeSinceWin);
				//posUpdate -= 1;
				//Debug.Log ("PosUpdate: " + posUpdate);
				//SwirlFinish ();
			}*/
		//}
	}

	// FixedUpdate is called every physics frame
	void FixedUpdate () {


	}
	
	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble")) {
			marble.GetComponent<Rigidbody> ().isKinematic = true;	// Hands total control of marble position to scripts.
			StartCoroutine("SwirlFinish");	// Starts the swirly animation.
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

	// SwirlFinish - Coroutine that makes the marble spiral after crossing the finish line.
	public IEnumerator SwirlFinish () {

		// [insert code to smoothly get the marble into the starting position of swirl without teleporting]

		// Spickler Magic
		float theta = Mathf.PI / 50.0f;	// Some random constant that Spickler came up with.
		for (int i = 500; i > 0; i -= 1) {
			marble.transform.position = new Vector3 ((theta * i / 4) * Mathf.Cos (5 * theta * i),	// The smaller the number being divided by, the bigger the circle.
			                                         (((1 * Mathf.PI - theta * i) / 2.0f) + (1 / 10.0f)), // The greater the number multiplied by pi, the higher the marble goes.
			                                         (theta * i / 4) * Mathf.Sin (5 * theta * i))   // The smaller the number being divided by, the bigger the circle.
													  + swirlPoint.transform.position;	// Makes this all happen around a given point.

			yield return new WaitForSeconds(0.005f);	// How long in seconds before calling the next loop iteration.
		}

		// [insert pop animation]
		// yield return new WaitForSeconds(/*length of pop animation*/);
		// [load next level or go back to main menu]

	}

	// OnDrawGizmos - Used to draw things in scene view exclusively: does not affect gameplay.
	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawCube(swirlPoint.position, new Vector3(1, 1, 1));	// Allows the swirl epicenter to be seen.
	}

	// SwirlFinish - makes the marble swirl near the finish line upon crossing
	/*public void SwirlFinish () {
		float sqMag = marble.transform.position.sqrMagnitude;	// radius of marble from the point mass
		float gravEq = ((GRAV_CONST * pointMass.GetComponent<Rigidbody> ().mass) / sqMag); //Universal Law of Gravitation
		float xi = marble.transform.position.x / Mathf.Sqrt (sqMag);	// i component of marble's position vector
		float yj = marble.transform.position.y / Mathf.Sqrt (sqMag);	// j component of marble's position vector
		float zk = marble.transform.position.z / Mathf.Sqrt (sqMag);	// k component of marble's position vector
	
		//The updated position after the inwards force has been applied to the ball
		Vector3 gravitySwirl = new Vector3 (-gravEq * xi, -gravEq * yj, -gravEq * zk);

		//Adding the force as an acceleration to the ball
		marble.GetComponent<Rigidbody>().AddForce (gravitySwirl, ForceMode.Acceleration);
	}*/
}
