using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BangGUI : MonoBehaviour {

	public float startScale = 3;
	private float endScale;
	private float scale;

	public float m_DampTime = 0.2f;
	private float m_ScaleSpeed;

	private bool isBanging = false;

	// Use this for initialization
	void Start () {
		endScale = gameObject.transform.localScale.x;
		gameObject.SetActive (false);
//		Bang();	//for text, delete later

	}
	
	// Update is called once per frame
	void Update () {
		if(isBanging) Banging ();
	}

	public void turnOff(){
		gameObject.SetActive (false);
	}

	public void Bang(){
		gameObject.SetActive (true);
		gameObject.transform.localScale = startScale * Vector3.one;
		scale = startScale;
		isBanging = true;
	}

	private void Banging(){
//		transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
//		m_Camera.orthographicSize = Mathf.SmoothDamp (m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
		scale = Mathf.SmoothDamp(scale, endScale, ref m_ScaleSpeed, m_DampTime);
		gameObject.transform.localScale = scale * Vector3.one;
		if (Mathf.Abs (scale - endScale) < 0.01) {
			scale = endScale;
			isBanging = false;
		}
	}
}
