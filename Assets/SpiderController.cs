using UnityEngine;
using System.Collections;

// mostly due to http://docs.unity3d.com/460/Documentation/ScriptReference/CharacterController.Move.html
public class SpiderController : MonoBehaviour {
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	private Vector3 moveDirection = Vector3.zero;

	private bool grounded = false;
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(0, Input.GetAxis("Horizontal") * 2.0f, 0);
		Vector3 forward = Vector3.forward;//transform.TransformDirection(Vector3.forward);
		float curSpeed = speed * Input.GetAxis("Vertical"); 

		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("speed", (int)curSpeed);

		if (Input.GetButton ("Jump") && grounded) {
			GetComponent<Rigidbody>().AddForce(new Vector3(0,jumpSpeed*20,0));
			grounded = false;
		} else {
		//	forward.y = -gravity;
		}

		transform.Translate (forward * curSpeed * Time.deltaTime);
	}

	void OnCollisionEnter(Collision collision) {
		grounded = true;
	}
}
