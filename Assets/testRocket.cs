using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testRocket : MonoBehaviour {

	public Vector2 init_velocity;
	public float limitation;
	public float accer_spd;
	public float fuel;
	private float fuel_init;
	private float fuel_scale;
	public float fuel_rate;
	public float fuel_decay_rate;
	public GameObject fuel_obj;
	public GameObject planet;	//since the planet is set to the origin, so the position is trivial
	public Vector2 displacement;
	private Vector2 pre_pos;
	private Vector2 cur_pos;

	private Rigidbody2D rb;
	private Vector3 velocity3;

	private float g_force;
	private Vector2 g_pos;
	public float d2planet;

	public Sprite idle; 
	public Sprite accer;
	private SpriteRenderer spriteRenderer;

	enum Stages { ready, controlling, finished }
	private Stages stage;

	public ParticleSystem explosion;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		g_force = planet.GetComponent<Planet> ().gravitiy;
		g_pos = planet.transform.position;
		stage = Stages.ready;
		cur_pos = V2fromV3(gameObject.transform.position);
		pre_pos = cur_pos;
		fuel_init = fuel;
		fuel_scale = fuel_obj.transform.localScale.x;
		spriteRenderer = gameObject.GetComponentInParent<SpriteRenderer> ();
		spriteRenderer.sprite = idle;
	}

	// Update is called once per frame
	void FixedUpdate () {

		d2planet = Vector3.Distance (gameObject.transform.position, planet.transform.position);
		if(d2planet > limitation) RocketDestory();

		if (stage == Stages.ready) {
//			if (Input.GetKeyDown (KeyCode.Space) || (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began) ) {
//				stage = Stages.controlling;
//			}

			rb.velocity = init_velocity;
			stage = Stages.finished;
		}

		cur_pos = V2fromV3(gameObject.transform.position);
		if (Vector2.Distance(cur_pos, pre_pos) > 0) {
			displacement = cur_pos - pre_pos;
		}

		if (stage == Stages.controlling){
			//update sprite
			UpdateSprit();
			if (Input.GetKey (KeyCode.Space) || (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Stationary) ) {
				if (fuel>0){
					rb.AddForce(gameObject.transform.up * accer_spd * Time.deltaTime);
					fuel -= Time.deltaTime * fuel_rate;
				}
			}
			if (fuel < 0) {
				fuel = 0;
				stage = Stages.finished;
				spriteRenderer.sprite = idle;
			}
			fuel -= Time.deltaTime * fuel_decay_rate;
			UpdateFuelBar();
			addGravity();
		}

		if (stage == Stages.finished){
			addGravity();
		}

		pre_pos = cur_pos;
	}

	void UpdateSprit(){
		if (Input.GetKey (KeyCode.Space) || (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Stationary) ){
			if(spriteRenderer.sprite == idle) {
				spriteRenderer.sprite = accer;
			}
		}
		if (Input.GetKeyUp(KeyCode.Space) || (Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Ended) ){
			spriteRenderer.sprite = idle;
		}
	}

	void UpdateFuelBar(){
		fuel_obj.transform.localScale = fuel / fuel_init * fuel_scale * Vector3.one;
	}

	void LateUpdate(){
		if (stage == Stages.controlling){
			rotate();
		}

		if (stage == Stages.finished){
			rotate();
		}
	}

	Vector2 V2fromV3(Vector3 v3){
		return new Vector2 (v3.x, v3.y);
	}

	//uncomment when needed
	//	Vector3 V3fromV2(Vector2 v2){
	//		return new Vector3 (v2.x, v2.y, 0);
	//	}

	void addGravity(){
		rb.AddForce (( g_pos - V2fromV3 (gameObject.transform.position)).normalized * g_force / Mathf.Pow (d2planet, 2));
	}

	void rotate(){
		Vector3 curAngle = gameObject.transform.rotation.eulerAngles;
		Vector3 angle = curAngle;
//		if (displacement.x * displacement.y != 0) {
//			angle = new Vector3 (0, 0, (Mathf.Rad2Deg * Mathf.Atan2 (displacement.y, displacement.x) + 270) % 360);
//		}
		Vector2 v = rb.velocity;
		angle = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2 (v.y, v.x) + 270) % 360);

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
	void OnCollisionEnter2D(Collision2D collision) {

		//create explosion : May be used later
		//		ContactPoint contact = collision.contacts[0];
		//		Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
		//		Vector3 pos = contact.point;
		//		Instantiate(explosionPrefab, pos, rot);
		RocketDestory();

	}

	void OnTriggerEnter2D(Collider2D other) {
		if (stage != Stages.ready) {
			RocketDestory ();
		}
	}

	void RocketDestory(){
		if (stage != Stages.ready) {
		}

		if (stage == Stages.ready || stage == Stages.controlling) {
			gameObject.SetActive (false);
		}

		// Unparent the particles from the shell.
		explosion.transform.parent = null;
		// Play the particle system.
		explosion.Play();

		// Once the particles have finished, destroy the gameobject they are on.
		ParticleSystem.MainModule mainModule = explosion.main;
		Destroy (explosion.gameObject, mainModule.duration);

		// Destroy the rocket.
		Destroy (gameObject);
	}
}
