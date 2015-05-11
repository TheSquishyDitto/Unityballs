using UnityEngine;
using System.Collections;

public class RaveLight : MonoBehaviour {

	Light rave;	// Reference to light component to rave-ify.

	public float speed = 0.1f;	// Speed of color transition.
	public bool loop = true;	// Whether the cycle should loop.
	public Color[] colors;		// Array of colors to cycle through.


	// Use this for initialization
	void Start () {
		rave = GetComponent<Light>();
		StartCoroutine("Transition");
	}

	// Transition - Slowly cycles through all colors.
	IEnumerator Transition() {
		for (int i = 0; i < colors.Length; i++) {
			while (rave.color != colors[i]) {
				rave.color = Vector4.MoveTowards(rave.color, colors[i], speed);
				yield return null;
			}

			if (loop && i >= colors.Length - 1)
				i = 0;
		}
	}
}
