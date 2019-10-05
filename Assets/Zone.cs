using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

	public Transform yellowTransform;
	public SpriteRenderer white;
	public Color normal;
	public Color OnTrigger;

	public GameManager manager;

	private float counter;
	public float timeThres;
	public float decayRate;
	public float plusRate;

	// Use this for initialization
	void Start () {
		counter = 0;
		white.color = normal;
	}
	
	// Update is called once per frame
	void Update () {
//		white.color = normal;
		if (counter > 0) {
			counter -= decayRate * Time.deltaTime;
		}
		if (counter < 0) {
			white.color = normal;
			counter = 0;
		}
		if (counter > timeThres) {
			manager.ZoneIn ();
			counter = 0;
		}

		yellowTransform.localScale = counter / timeThres * Vector3.one;
	}

	void OnTriggerEnter2D(Collider2D other) {
		print ("trigger");
		if (other.CompareTag ("Rocket")) {
			white.color = OnTrigger;
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.CompareTag ("Rocket")) {
			counter += plusRate*Time.deltaTime;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
	}
}
