using UnityEngine;
using System.Collections;

namespace Escape {
	public abstract class BaseRoom : MonoBehaviour {

		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		void Update () {
	
		}

		// Override this!
		abstract public Transform GetEntrance ();
	}
}