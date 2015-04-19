using UnityEngine;
using System.Collections;
using System;

public class AICharacterController : MonoBehaviour {

	public Transform player; // The player controlled character, to try and find
	public float chaseDistance = 4.0f; // the distance at which this AI starts chasing the player
	public float rechaseDistance = 2.5f; // the distance we start following again

	public Transform[] waypoints; // patrol locations
	private int currentWaypoint = 0; // how far through the patrol are we

	private Transform goingTo; // if going

	private enum State {
		PATROLLING,
		LOOKING,
		CHASING,
		FOUND,
		GOING
	};

	private State _state; // actual state, this is for debugging
	private State state {
		get { return _state; }
		set {
			_state = value;
			//Debug.Log ("AI state = " + value.ToString ());
		}
	}

	// Use this for initialization
	void Start () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		if (waypoints.Length == 0) {
			state = State.CHASING; // no waypoints, have to chase
		} else {
			state = State.PATROLLING; // somewhere to patrol to
			agent.SetDestination(waypoints[0].position);
		}
		agent.stoppingDistance = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		switch (state) { // do the appropriate action
		case State.PATROLLING:
			DoPatrol ();
			break;
		case State.LOOKING:
			DoLook ();
			break;
		case State.FOUND:
			DoFound ();
			break;
		case State.CHASING:
			DoChase ();
			break;
		case State.GOING:
			DoGoTo ();
			break;
		default:
			Debug.LogError("Unknown state");
			break;
		}

		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		// tell the animator how fast we're going
		Animator anim = GetComponent<Animator> ();
		anim.SetFloat ("speed", agent.velocity.magnitude);
	}

	// call this if you want the agent to go somewhere specific
	public void GoToLocation(Transform location) {
		state = State.GOING;
		goingTo = location;
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.enabled = true;
		agent.Resume ();
		agent.SetDestination (goingTo);
	}

	void DoGoTo () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		if (agent.destination != goingTo) {
			agent.SetDestination(goingTo);
		}
	}

	// checks if the player is close enough to start chasing, if so changes the state appropriately
	void CheckPlayerDistance () {
		if ((transform.position - player.position).magnitude < chaseDistance) {
			state = State.CHASING;
		}
	}

	// returns the nearest pickup object that is within the threshold or null  if there are none.
	GameObject GetNearbyPickup () {
		float minDist = float.PositiveInfinity; 
		GameObject nearest = null;
		Vector3 pos = transform.position;
		float sqrthresh = chaseDistance * chaseDistance; // using square mag still works fine and avoids as many sqrts so may as well

		GameObject[] pickups = GameObject.FindGameObjectsWithTag("pickup");
		foreach (GameObject p in pickups) {
			Vector3 dif = pos - p.transform.position;
			float dist = dif.sqrMagnitude;
			if (dist <= sqrthresh && dist < minDist) {
				minDist = dist;
				nearest = p;
			}
		}
		return nearest; 
	}

	// run through all the waypoints
	void DoPatrol () {
		// assumes there is at least one waypoint (if there isn't we should never get to PATROLLING)
		Transform dest = waypoints [currentWaypoint];
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		//Debug.Log (agent.remainingDistance);
		if ((dest.position - transform.position).sqrMagnitude < 0.25) { // there yet?
			//Debug.Log(String.Format("Agent reached waypoint{0}", currentWaypoint+1));
			currentWaypoint = (currentWaypoint + 1) % waypoints.Length; // cycle through
			dest = waypoints [currentWaypoint];
		}

		agent.SetDestination (dest.position);

		CheckPlayerDistance ();
		// check for nearby pickups SECOND
		// this means the AI (when patrolling) will prefer to stare at pickups rather than chase
		if (GetNearbyPickup () != null) {
			state = State.LOOKING;
			agent.Stop ();
			agent.enabled = false;
		}
	}

	// look at a pickup while it is close enough
	// DOES NOT check player distance
	// idea being AI is distracted while looking at a pickup
	void DoLook () {
		GameObject near = GetNearbyPickup ();
		if (near != null) {
			transform.LookAt (near.transform.position);
		} else { // not nearby anymore
			state = State.PATROLLING; // back to the route
			GetComponent<NavMeshAgent> ().enabled = true;;
		}
	}

	void DoChase () {
		NavMeshAgent agent = GetComponent<NavMeshAgent> ();
		agent.destination = player.position; // just try and get to the player
		// changing from CHASING to FOUND is handled in OnTriggerEnter
		if (GetNearbyPickup () != null) { // but it would be cool to be able to distract it
			state = State.LOOKING;
			agent.Stop ();
			agent.enabled = false;
		}
	}

	void DoFound () {
		// if state is FOUND, we must have stopped moving (handled during the state transition in OnTriggerEnter)
		transform.LookAt (player.position); // just watch...
		if ((player.position - transform.position).sqrMagnitude > (rechaseDistance * rechaseDistance)) {
			state = State.CHASING; // it's getting away!
			GetComponent<NavMeshAgent> ().enabled = true; // restart navigation
		}
	}

	void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "Player") {
			NavMeshAgent agent = GetComponent<NavMeshAgent> ();
				state = State.FOUND;
				agent.Stop (); // stop moving
				agent.enabled = false;
		}
	}
}
