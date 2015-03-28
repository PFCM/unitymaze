using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Called just before physics
	void FixedUpdate() {
		float moveHorizontal = -Input.GetAxis ("Horizontal");
		float moveVertical = -Input.GetAxis ("Vertical");
		
		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		
		GetComponent<Rigidbody> ().AddForce (movement * 500 * Time.deltaTime);
		
		//rigidbody.AddForce (movement);
	}
}
