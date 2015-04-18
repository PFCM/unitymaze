using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

// mostly due to http://docs.unity3d.com/460/Documentation/ScriptReference/CharacterController.Move.html
public class SpiderController : MonoBehaviour {
	public float speed = 6.0f;
	public float jumpSpeed = 8.0f;
	public float gravity = 20.0f;
	public GameObject countText;

	public float interactionDistance = 200.0f;

	private Vector3 moveDirection = Vector3.zero;

	// the stuff we've picked up
	private List<GameObject> inventory = new List<GameObject>();
	private GameObject touching; // a pickup object we are touching (ray cast doesn't get these ones)


	private bool grounded = false;
	
	// Update is called once per frame
	void Update () {
	
		transform.Rotate(0, Input.GetAxis("Horizontal") * 2.0f, 0);
		Vector3 forward = Vector3.forward;//transform.TransformDirection(Vector3.forward);
		float curSpeed = speed * Input.GetAxis("Vertical"); 

		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("speed", curSpeed);

		// using GetButtonDown rather than GetButton so they only fire once
		if (Input.GetButtonDown ("Jump") && grounded) {
			GetComponent<Rigidbody> ().AddForce (new Vector3 (0, jumpSpeed * 3000, 0));
			grounded = false;
		} else if (Input.GetButtonDown ("Bump")) {
			// if a pickup is nearby, bump it
			Debug.Log("bump");

			Bump(GetObjectInFront());

		} else if (Input.GetButtonDown ("PickUp")) {
			// if a pickup is nearby pick it up
			Debug.Log("pickup");
			GameObject item = GetObjectInFront ();
			if (item == null) { // chuck the last one
				Chuck();
			} else if (item != null) {
				Pickup(item);
			}
			UpdateGUI ();
		}

		transform.Translate (forward * curSpeed * Time.deltaTime);
	}

	// chucks the last item in the inventory, if there is one
	void Chuck() {
		if (inventory.Count > 0) {
			GameObject item = inventory[0];
			inventory.RemoveAt (0);
			item.transform.position = transform.position + 
				transform.TransformDirection (Vector3.forward)*(interactionDistance/2.0f);
			item.SetActive(true);
			Bump(item);
		}
	}

	// updates the gui, handy to call when altering the inventory one way or another
	void UpdateGUI() {
		countText.GetComponent<Text> ().text = String.Format("{0}",inventory.Count);
	}

	void Pickup(GameObject item) {
		if (item != null) {
			item.SetActive(false);
			inventory.Add(item);
		}
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.collider.gameObject.tag.Equals("pickup")) {
			touching = collision.collider.gameObject;
		}
		grounded = true;
	}

	/*void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals ("pickup")) {
			++pickups;
			other.gameObject.SetActive(false);
			countText.GetComponent<Text> ().text = String.Format("{0}",pickups);
		}
	}*/

	// bumps an object
	private void Bump(GameObject toBump) {
		if (toBump != null) {
			Vector3 force = Quaternion.AngleAxis(75, Vector3.left) * Vector3.forward;
			force = transform.TransformDirection(force) * 300;
			toBump.GetComponent<Rigidbody> ().AddForce(force);
		}
	}

	// gets an object in front that has the tag "pickup" and is within the threshold 
	// or if we are touching it and it is close enough to being in front
	private GameObject GetObjectInFront() {
		RaycastHit hit;
		GameObject inFront = null;
		if (Physics.Raycast (new Vector3 (transform.position.x, 0.25f, transform.position.z),
		                     -Vector3.forward, out hit, interactionDistance)) {
			if (hit.collider.gameObject.tag.Equals("pickup")) {
				inFront = hit.collider.gameObject;
				Debug.Log ("found one");
			} 
		}
		if (touching != null && touching.activeInHierarchy && inFront == null) { // didn't find one in front but are touching an active one
			if (Vector3.Distance(transform.position, 
			                     touching.GetComponent<Transform> ().position)
			    <= interactionDistance) {
				// close enough, what about the angle?
				inFront = touching;
			}
			touching = null;
		}
		return inFront;
	}
}
