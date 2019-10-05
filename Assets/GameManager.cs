using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public enum Rule { number, handle, limit};
	public Rule rule;

	private int count;
	public int score;
	public int record;
	public Text scoreText;
	public Text limitText;
	private BangText countTextBang;
	public Text recordText;
	public Text angleText;
	public RocketMovement rocketObj;
	public RocketRotator rotator;
	public GameObject[] planet;
	public BangGUI recordImg;
	public float limitation;

	public Vector2 AngleRange;
	public Vector2 ScaleRange;
	public Vector2 FuelRange;
	public float fuel_rate;
	public float fuel_decay_rate;
	public float accer_spd;
	public Transform genPoint;

	public CameraControl cam;
	public float minSize_ready;
	public float minSize_flying;

	public bool showAngle;

	// Use this for initialization
	void Start () {
		count = 0;
		score = 0;
		GenerateRocket ();
		countTextBang = scoreText.GetComponent<BangText> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Launched(){
		CountPlus (1);
		if (rule == Rule.number) {
			ScorePlus ();
		}
		cam.SetMinSize (minSize_flying);
	}

	public void ZoneIn(){
		if (rule == Rule.handle || rule == Rule.limit) {
			ScorePlus ();
		}
	}

	private void CountPlus(int num){
		count+=num;
		if (count < 0)
			count = 0;
		UpdateText ();
	}

	private void ScorePlus(){
		score++;
		if (rule == Rule.limit) {
			score = Mathf.Clamp (score, 0, (int)Mathf.Pow(2,count));
		}
		if (score > record) {
			record = score;
			recordImg.Bang ();
		}
		countTextBang.Bang ();
		UpdateText ();

	}

	private void ScoreMinus(int num){
		score-=num;
		if (score < 0)
			score = 0;
		recordImg.turnOff ();
		UpdateText ();
	}

	private void UpdateText(){
		scoreText.text = score.ToString();
		limitText.text = (count*5).ToString ();
		recordText.text = record.ToString();
	}

	public void RocketDestroyed(){
		count--;
		if (rule == Rule.number) {
			ScoreMinus (1);
		} else if (rule == Rule.handle || rule == Rule.limit) {
			if (score > 0)
				ScoreMinus (2);
			//if in zone : minus
		}
	}

	public void RocketFinished(){
		GenerateRocket ();
	}

	private void GenerateRocket(){
		RocketMovement rocket = (RocketMovement)Instantiate (rocketObj, genPoint.position, genPoint.rotation);
		rocket.transform.parent = planet [0].transform;
		rocket.transform.Rotate(new Vector3(0,0,Random.Range (AngleRange.x, AngleRange.y)));

		rocket.gameManager = this;
		rocket.planet = this.planet;
		float scale = Random.Range (ScaleRange.x, ScaleRange.y);
		rocket.transform.localScale *= scale;
		rocket.transform.GetChild(0).localScale /= scale ;
		float fuelScale = Random.Range (FuelRange.x, FuelRange.y);
		rocket.fuel_decay_rate = fuel_decay_rate;
		rocket.fuel_rate = fuel_rate;
		rocket.fuel *= fuelScale;
		rocket.transform.GetChild(0).localScale *= scale ;
		rocket.accer_spd = accer_spd;
		rocket.limitation = limitation;

		rotator = rocket.GetComponentInParent<RocketRotator> ();
		rotator.planet = this.planet;
		if (showAngle) {
			rotator.showAngle = true; 
		}
		rotator.angleText = this.angleText;
		rotator.cam = this.cam.GetComponentInParent<Camera>();

		cam.m_Targets [0] = rocket.gameObject.transform;
		cam.SetMinSize ( minSize_ready);
	}
}
