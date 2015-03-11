/// <summary>
/// FinishLine.cs
/// Authors: Charlie Sun, Kyle Dawson, Chris Viqueira,[ANYONE ELSE WHO MODIFIES CODE PUT YOUR NAME HERE]
/// Date Created:  Feb. 13, 2015
/// Last Revision: Mar. 11, 2015
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

	}

	// FixedUpdate is called every physics frame
	void FixedUpdate () {


	}
	
	// OnTriggerEnter - Called when an object enters the trigger collider.
	void OnTriggerEnter (Collider other) {
		if (other.CompareTag("Marble") && gm.state != GameMaster.GameState.Win) {
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

		// NOTE: TO CHANGE WHERE THE MARBLE SWIRLS AROUND, MOVE THE "SWIRLPOINT"

		// [insert code to smoothly get the timer into position without teleporting, maybe separate coroutine? ]
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
			marble.transform.position = new Vector3 ((theta * i / width) * Mathf.Cos (speed * theta * i),	// The smaller the number being divided by, the bigger the circle.
			                                         (((gap * Mathf.PI + theta * i) / height) + spiconstant), // The greater the number multiplied by pi, the higher the marble goes.
			                                         (theta * i / width) * Mathf.Sin (speed * theta * i))   // The smaller the number being divided by, the bigger the circle.
													+ swirlPoint.transform.position - new Vector3(0, (((gap * Mathf.PI + theta * swirls) / height) + spiconstant), 0);	// Makes this all happen around a given point.
			
			yield return new WaitForSeconds(0.005f);	// How long in seconds before calling the next loop iteration.
		}

		// Christmas Tree Swirl
		for (int i = swirls; i > 0; i--) {
			marble.transform.position = new Vector3 ((theta * i / width) * Mathf.Cos (speed * theta * -i),	// The smaller the number being divided by, the bigger the circle.
			                                         (((gap * Mathf.PI - theta * i) / height) + spiconstant), // The greater the number multiplied by pi, the higher the marble goes.
			                                         (theta * i / width) * Mathf.Sin (speed * theta * -i))   // The smaller the number being divided by, the bigger the circle.
													+ swirlPoint.transform.position + new Vector3(0, (((gap * Mathf.PI + theta * swirls) / height) + spiconstant), 0);	// Makes this all happen around a given point.

			yield return new WaitForSeconds(0.005f);	// How long in seconds before calling the next loop iteration.
		}

		// [insert pop animation]
		// yield return new WaitForSeconds(/*length of pop animation*/);
		// [show menu for loading next level or go back to main menu]

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
