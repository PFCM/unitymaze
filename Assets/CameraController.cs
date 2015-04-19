using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public GameObject player;
	private Vector3 offset;
	private float xzDistance; // distance from player along the round plane
	private float height; // distance up
	
	// Use this for initialization
	void Start () {
		offset = transform.position - player.transform.position;
		height = offset.y;
		xzDistance = Mathf.Sqrt (offset.x * offset.x + offset.z * offset.z);
	}
	
	// LateUpdate is called once per frame
	void LateUpdate () {
		Vector3 newPos = player.transform.position - (player.transform.forward * xzDistance);
		newPos.y = height;
		transform.position = newPos;
		transform.LookAt (player.transform);
	}
}