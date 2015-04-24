using UnityEngine;
using System.Collections;

public class SecretLevel : MonoBehaviour {

	void OnTriggerEnter () {
		Application.LoadLevel("SecretLevel");
	}
}
