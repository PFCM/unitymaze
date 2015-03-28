using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	public GameObject player;
	private Vector3 offset;
	private Vector3 rotOffset;
	
	// Use this for initialization
	void Start () {
		offset = GetComponent<Transform> ().position - player.transform.position;
		rotOffset = GetComponent<Transform> ().rotation.eulerAngles - player.transform.rotation.eulerAngles;
	}
	
	// LateUpdate is called once per frame
	void LateUpdate () {
		GetComponent<Transform> ().position = player.GetComponent<Transform> ().position + offset;
		Vector3 playerAngles = player.GetComponent<Transform> ().rotation.eulerAngles;

		//GetComponent<Transform> ().rotation = Quaternion.Euler (new Vector3(rotOffset.x, rotOffset.y + playerAngles.y, rotOffset.z));
	}
}