using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMotion : MonoBehaviour {

	public Vector2 init_speed;
	public GameObject[] planet;	//since the planet is set to the origin, so the position is trivial

	private Rigidbody2D rb;
	private Vector3 velocity3;

	private float[] g_force;
	private Vector2[] g_pos;
	private float[] d2planet;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		g_force = new float[planet.Length + 1];
		g_pos = new Vector2[planet.Length + 1];
		d2planet = new float[planet.Length + 1];
		for(int i = 0; i < planet.Length; i++) {
			g_force[i] =  planet[i].GetComponent<Planet> ().gravitiy;
			g_pos[i] = planet[i].transform.position;
		}
		rb.velocity = init_speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		addGravity ();
	}

	void addGravity(){
		// don't use gravity before using enough fuel
		for (int i = 0; i < planet.Length; i++) {
			d2planet[i] = Vector3.Distance (gameObject.transform.position, planet[i].transform.position);
			//			rb.velocity += (g_pos - V2fromV3 (gameObject.transform.position)).normalized * g_force / Mathf.Pow (d2planet, 2) / rb.mass * Time.deltaTime;
			//			Vector2 
			g_pos[i] = planet[i].transform.position;

			rb.AddForce ((g_pos [i] - V2fromV3 (gameObject.transform.position)).normalized * g_force [i] / Mathf.Pow (d2planet [i], 2) * Time.deltaTime, ForceMode2D.Impulse);
		}
	}

	Vector2 V2fromV3(Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}
}
