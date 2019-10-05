using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour {

	private ParticleSystem system;
	public Color[] ColorList = {Color.white};

	// Use this for initialization
	void Start () {
		system = gameObject.GetComponentInParent<ParticleSystem> ();
		var mainModule = system.main;
		mainModule.startColor = ColorList[ Random.Range(0,ColorList.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
