﻿using UnityEngine;
using System.Collections;

public class LoadNewMaze : MonoBehaviour {
	void OnTriggerEnter(Collider other) {
		GameObject newMaze = (GameObject)Instantiate (Resources.Load ("maze"), 
		            								  this.transform.parent.position + new Vector3(45,0,-50), 
		             								  Quaternion.identity);

	}
}