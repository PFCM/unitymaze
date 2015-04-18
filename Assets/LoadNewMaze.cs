using UnityEngine;
using System.Collections;
using Escape;

public class LoadNewMaze : MonoBehaviour {

	private bool generated = false;

	void OnTriggerEnter(Collider other) {
		if (!generated) { // only do this once
			GameObject newMaze = (GameObject)Instantiate (Resources.Load ("maze"), 
		            								  this.transform.parent.position + new Vector3 (45, 0, -50), 
		             								  Quaternion.identity);
			BaseRoom newMazeBase = newMaze.GetComponent<BaseRoom> ();
			Transform newEntrance = newMazeBase.GetEntrance (); // hurray?
			// TODO line it up (and make random entrances)


			generated = true;	
		}
	}
}
