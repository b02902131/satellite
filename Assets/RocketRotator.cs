using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RocketRotator : MonoBehaviour {

	public LineRenderer lineRenderer;
	public RocketMovement moveMent;
	private Rigidbody2D rb; 
	public bool showAngle;

	public GameObject[] planet;
	public Camera cam;
	public Text angleText;
	public Vector3 angleTextOffset;

	// Use this for initialization
	void Start () {
		if(!showAngle){
			angleText.gameObject.SetActive (false);
		}
		moveMent = gameObject.GetComponentInParent<RocketMovement> ();
		rb = gameObject.GetComponentInParent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		

		if (moveMent.stage == RocketMovement.Stages.controlling){
			rotate();
		}

		if (moveMent.stage == RocketMovement.Stages.finished){
			if (showAngle) {
				showAngle = false;
				LineRenderer[] line = gameObject.GetComponentsInChildren<LineRenderer> ();
				foreach(LineRenderer l in line)
				{
					l.enabled = false;
				}
			}
			rotate();
		}
		lineRenderer.SetPosition (1, planet[0].transform.position - Vector3.forward);

		//drwe line when vertical
		if (showAngle) {
			lineRenderer.SetPosition (0, gameObject.transform.position - Vector3.forward);
			angleText.transform.position = cam.WorldToScreenPoint (gameObject.transform.position + angleTextOffset);
			float angle = gameObject.transform.rotation.eulerAngles.z + 90;		// this start from up, so should be minus 90
			Vector3 v = planet[0].transform.position - gameObject.transform.position;
			float angle2planet = Mathf.Rad2Deg * Mathf.Atan2 (v.y, v.x);
			angle = angle2planet - angle;
			while (angle < 0)
				angle += 360;
			if (angle > 180)
				angle = 360 - angle;
			angleText.text = angle.ToString ("F2") + "°";
			Color color = new Color (1, 1, 1,  Mathf.Clamp(1-Mathf.Abs(angle-90)/10 , 0.3f, 1f));
			angleText.color = color;
			Vector3 desiredSize = Mathf.Clamp (3 * (0.8f - Mathf.Abs (angle - 90) / 10), 1, 3) * Vector3.one;
			angleText.transform.localScale += (desiredSize - angleText.transform.localScale) / 10;
		}
	}

	void UpdateAngle(){
		
	}

	void rotate(){
		Vector3 curAngle = gameObject.transform.rotation.eulerAngles;
		Vector2 v = rb.velocity;
		Vector3 angle = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2 (v.y, v.x) + 270) % 360);

		if (Vector3.Distance (angle, curAngle) > 0) {
			float angle_d = angle.z - curAngle.z;
			angle_d %= 360;
			if (angle_d > 180)
				angle_d -= 360;
			if (angle_d < -180)
				angle_d += 360;
			gameObject.transform.rotation = Quaternion.Euler (0, 0, curAngle.z + angle_d / 10);
		}


	}


}
