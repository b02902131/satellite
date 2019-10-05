using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BangText : MonoBehaviour {

	public float startScale = 3;
	private float endScale;
	private float scale;

	public float m_DampTime = 0.2f;
	private float m_ScaleSpeed;

	private bool isBanging = false;

	// Use this for initialization
	void Start () {
		endScale = gameObject.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
		if(isBanging) UpdateBang ();
	}

	public void Bang(){
		gameObject.transform.localScale = startScale * Vector3.one;
		scale = startScale;
		isBanging = true;
	}

	private void UpdateBang(){
		scale = Mathf.SmoothDamp(scale, endScale, ref m_ScaleSpeed, m_DampTime);
		gameObject.transform.localScale = scale * Vector3.one;
		if (Mathf.Abs (scale - endScale) < 0.01) {
			scale = endScale;
			isBanging = false;
		}
	}
}
