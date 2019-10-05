using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFollowObject : MonoBehaviour {

	public Transform m_Target;
	public Camera cam;
	public Vector3 offset;

	private Text text;

	// Use this for initialization
	void Start () {
		text = gameObject.GetComponentInParent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		text.transform.position = cam.WorldToScreenPoint (m_Target.position + offset);
	}
}
