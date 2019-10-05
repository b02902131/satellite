using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour {

	public Camera cam;
	public Transform m_target;
	public float edge_offset;
	public float bound_offset;
	public Transform arrow;

	public bool bouncing;
	public Transform target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cam_pos = cam.WorldToScreenPoint (m_target.position);
		Vector3 screen_mid = new Vector3 (Screen.width / 2, Screen.height / 2, 0);
		Vector3 cam_pos_m = cam_pos - screen_mid;
//		cam_pos -= screen_mid;
		if (isOutOfBound (cam_pos_m)) {		
			Vector3 pos;
			pos.x = myClamp (cam_pos.x, edge_offset, Screen.width-edge_offset);
			pos.y = myClamp (cam_pos.y, edge_offset, Screen.height-edge_offset);
			pos.z = 0; 

			this.transform.position = pos;
			arrow.rotation = Quaternion.Euler (new Vector3 (0, 0, Mathf.Atan2(cam_pos_m.y, cam_pos_m.x) * Mathf.Rad2Deg));
		} else {
			this.transform.position = new Vector3 (2*Screen.width, 2*Screen.height, 0);
		}
	}

	private float myClamp(float value, float min, float max){
		if (value < min)
			return min;
		else if (value > max)
			return max;
		else
			return value;
	}

	private bool isOutOfBound(Vector3 pos){
		if(Mathf.Abs( pos.x ) > Screen.width/2 + bound_offset || Mathf.Abs(pos.y)> Screen.height/2 + bound_offset){
			return true;
		}
		else {
			return false;
		}
	}
}
