using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildText : MonoBehaviour {

	public KeyCode key;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (key) || (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began))
			gameObject.SetActive (false);
	}
}
