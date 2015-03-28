using UnityEngine;
using System.Collections;
using Escape;

public class LoadNewMaze : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		GameObject newMaze = (GameObject)Instantiate (Resources.Load ("maze"), 
		            								  this.transform.parent.position + new Vector3(45,0,-50), 
		             								  Quaternion.identity);
		BaseRoom newMazeBase = newMaze.GetComponent<BaseRoom> ();
		Vector3 v = newMazeBase.GetDoorPosition (); // hurray?	
	}
}
