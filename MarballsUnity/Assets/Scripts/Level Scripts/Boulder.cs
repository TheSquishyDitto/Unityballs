using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour, IKillable {

	public Vector2 sizeRange = new Vector2(3, 7);	// Range of boulder sizes.

	// Use this for initialization
	void OnEnable () {
		if (GetComponent<Rigidbody>() != null) GetComponent<Rigidbody>().velocity = Vector3.zero;
		transform.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
	}

	// Die - Object turns itself off.
	public void Die() {
		gameObject.SetActive(false);
	}
}
