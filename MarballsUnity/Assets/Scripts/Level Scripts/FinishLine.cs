/// <summary>
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
	public Transform marble;
	public Transform rotPlaceHolder;
	public Vector3 finishPos; // The position of the marble once it crosses the finish line
	
	void Awake () {
		gm = GameMaster.CreateGM ();
		gm.finishLine = this.transform;
	}

	// Use this for initialization
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
		if (other.CompareTag("Marble")) {
			marble.GetComponent<Rigidbody> ().isKinematic = true;
			finishPos = marble.position;
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

	// SwirlFinish - make the marble spiral after crossing the finish line
	/*public void SwirlFinish () {
		float theta = Mathf.PI / 50.0f;

		// Spickler Magic
		marble.transform.position = new Vector3 (finishPos.x + (theta * Time.deltaTime / 20) * Mathf.Cos (5 * theta * Time.deltaTime),
		                                        finishPos.y + (((2 * Mathf.PI - theta * Time.deltaTime) / 2.0f) + (1 / 10.0f)),
		                                        finishPos.z + (theta * Time.deltaTime / 20) * Mathf.Sin (5 * theta * Time.deltaTime));
	}*/

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
