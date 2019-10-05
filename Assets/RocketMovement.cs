using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMovement : MonoBehaviour {
	public GameManager gameManager;
	public float limitation;
	public float accer_spd;
	private float minVelocity = 0.1f;
	public float fuel;
	private float fuel_init;
	private float fuel_scale;
	public float fuel_rate;
	public float fuel_decay_rate;
	public GameObject fuel_obj;
	public GameObject[] planet;	//since the planet is set to the origin, so the position is trivial

	private Rigidbody2D rb;
	private Vector3 velocity3;

	private float[] g_force;
	private Vector2[] g_pos;
	private float[] d2planet;

	public Sprite idle; 
	public Sprite accer;
	private SpriteRenderer spriteRenderer;

	public enum Stages { ready, controlling, finished }
	private bool isBoosting = false;
	public bool isBraking = false;
	public Stages stage;

	public ParticleSystem explosion;
	public ParticleSystem brake;
	public ParticleSystem trail;
 
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
		stage = Stages.ready;
		fuel_init = fuel;
		fuel_scale = fuel_obj.transform.localScale.x;
		spriteRenderer = gameObject.GetComponentInParent<SpriteRenderer> ();
		spriteRenderer.sprite = idle;

	}

	void Update(){
		UpdateInput ();
	}

	void UpdateInput(){
		if (stage == Stages.ready) {
			if (Input.GetKeyDown (KeyCode.Space) || (Input.touchCount==1 && Input.GetTouch(0).phase == TouchPhase.Began) ) {
				gameManager.Launched ();
				gameObject.transform.parent = null;
				stage = Stages.controlling;
			}
		}

		if (stage == Stages.controlling){
			//update sprite
			if (Input.GetKey (KeyCode.Space) || (Input.touchCount ==1 && Input.GetTouch (0).phase == TouchPhase.Stationary)) {
				if (Input.GetKey (KeyCode.S)) {
					if (fuel > 0) {
						rb.AddForce (gameObject.transform.up * accer_spd/10 * Time.deltaTime, ForceMode2D.Impulse);
						fuel -= Time.deltaTime * fuel_rate/10;
					}
				} else {
					if (fuel > 0) {
						rb.AddForce (gameObject.transform.up * accer_spd * Time.deltaTime, ForceMode2D.Impulse);
						fuel -= Time.deltaTime * fuel_rate;
					}
				}
				if (!isBoosting)
					isBoosting = true;
			} else {
				if (isBoosting)
					isBoosting = false;
			}
			if (Input.GetKey (KeyCode.X) || (Input.touchCount ==2 && Input.GetTouch (0).phase == TouchPhase.Stationary) ) {
				if (fuel > 0) {
					if (rb.velocity.magnitude > minVelocity) {
						rb.AddForce (-gameObject.transform.up * accer_spd / 10 * Time.deltaTime, ForceMode2D.Impulse);
					}
					fuel -= Time.deltaTime * fuel_rate / 10;
					isBraking = true;
				}
			} else {
				isBraking = false;
			}
			if (fuel < 0) {
				fuel = 0;
				spriteRenderer.sprite = idle;
				isBoosting = false;
				isBraking = false;
				stage = Stages.finished;
				gameManager.RocketFinished ();
			}
			fuel -= Time.deltaTime * fuel_decay_rate;
			UpdateFuelBar();
			UpdateSpritAndBrake();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		for (int i = 0; i < planet.Length; i++) {
			d2planet[i] = Vector3.Distance (gameObject.transform.position, planet[i].transform.position);
			if (d2planet[i] > limitation)
				RocketDestory ();
		}

		if (stage == Stages.controlling){
			addGravity();
		}

		if (stage == Stages.finished){
			addGravity();
		}

	}

	void UpdateSpritAndBrake(){
		if(isBoosting){
			if(spriteRenderer.sprite == idle) {
				spriteRenderer.sprite = accer;
			}
		}
		else{
			if(spriteRenderer.sprite == accer){
				spriteRenderer.sprite = idle;
			}
		}

		if (isBraking) {
			brake.Play ();
		} else {
			
		}
	}
		

	void UpdateFuelBar(){
		fuel_obj.transform.localScale = fuel / fuel_init * fuel_scale * Vector3.one;
	}

	void LateUpdate(){
	}

	Vector2 V2fromV3(Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}

	//uncomment when needed
//	Vector3 V3fromV2(Vector2 v2){
//		return new Vector3 (v2.x, v2.y, 0);
//	}

	void addGravity(){
		// don't use gravity before using enough fuel
		for (int i = 0; i < planet.Length; i++) {
			if (fuel / fuel_init < 1) {
//			rb.velocity += (g_pos - V2fromV3 (gameObject.transform.position)).normalized * g_force / Mathf.Pow (d2planet, 2) / rb.mass * Time.deltaTime;
//			Vector2 
				g_pos[i] = planet[i].transform.position;
				rb.AddForce ((g_pos [i] - V2fromV3 (gameObject.transform.position)).normalized * g_force [i] / Mathf.Pow (d2planet [i], 2) * Time.deltaTime, ForceMode2D.Impulse);
			}
		}
	}


	void OnCollisionEnter2D(Collision2D collision) {

		//create explosion : May be used later
//		ContactPoint contact = collision.contacts[0];
//		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
//		Vector3 pos = contact.point;
//		Instantiate(explosionPrefab, pos, rot);
		if (collision.gameObject.CompareTag ("Rocket")) {
			RocketDestory ();
		}

	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Planet")){
			if (stage != Stages.ready) {
				RocketDestory ();
			}
		}
	}

	void RocketDestory(){
		if (stage != Stages.ready) {
			gameManager.RocketDestroyed ();
		}

		if (stage == Stages.ready || stage == Stages.controlling) {
			gameManager.RocketFinished ();
		}

		// Unparent the particles from the shell.
		explosion.transform.parent = null;
		trail.transform.parent = null;
		trail.transform.localScale = Vector3.one;
		var emission = trail.emission;
		emission.rateOverTime = 0;
		// Play the particle system.
		explosion.Play();

		// Once the particles have finished, destroy the gameobject they are on.
		ParticleSystem.MainModule mainModule = explosion.main;
		Destroy (explosion.gameObject, mainModule.duration);
		ParticleSystem.MainModule mainModule2 = trail.main;

		Destroy (trail.gameObject, mainModule2.duration);

		//Destroy the rocket.
//		gameObject.SetActive (false);
		Destroy (gameObject);
	}
}
