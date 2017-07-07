using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Fruit : MonoBehaviour {
	private float timeStayingCollision;
	public float thresholdInsedCollider = 0.7f;
	public GameObject  GameController;
	public AudioClip SoundSpawn;
	GameModeController controller;
	private AudioSource audio;
	private float reactionTime;
	public int positionIndex;
	bool onlyOnePass = true;
	public bool isBananeMode = false;
	public bool IsTheGoalFruit = false;
	public float lifeTimeFruit;
    GameObject OSCsenderContainer;
    OSCsender oscSender;
	bool fruitHasFinishedTime = false;
	public bool takeFruitEmulator = false;


    void Awake()
    {
        OSCsenderContainer = GameObject.Find("OSCsender");
        oscSender = OSCsenderContainer.GetComponent<OSCsender>();
    }

    void Start(){
		controller = GameController.GetComponent<GameModeController>();

    }

	void Update(){

		if (reactionTime > lifeTimeFruit && !fruitHasFinishedTime) {
			FruitHasFinishedLifeTime();
			fruitHasFinishedTime = true;
		}
       
		if (Input.GetKeyDown(KeyCode.M)){
			FruitIsConsideredTouched();
           // Destroy(gameObject, 1f);
		}
		if (takeFruitEmulator) {
			takeFruitEmulator = false;
			FruitIsConsideredTouched();

		}
		reactionTime += Time.deltaTime;
	}
	void OnTriggerEnter(Collider collider){
		timeStayingCollision = 0f;

	}
	void OnTriggerStay(Collider collider){
		timeStayingCollision += Time.deltaTime;
		if(timeStayingCollision >= thresholdInsedCollider){
			FruitIsConsideredTouched();
		}
	}
	void FruitIsConsideredTouched(){
		if(onlyOnePass){
			if( !IsTheGoalFruit){
				FruitHasBeenCatched();
			}else{

				//oscSender.SendData(positionIndex, reactionTime);
				Destroy(gameObject, 0.1f);
				controller.GoalFruitHasBeenCatched(positionIndex, reactionTime,oscSender);

			}
			onlyOnePass = false;
		}
		
	}
	void FruitHasBeenCatched(){
		controller.FruitHasBeenCatched(positionIndex, reactionTime,oscSender);
		//oscSender.SendData(positionIndex, reactionTime);
		Destroy(gameObject, 0.1f);
	}
	void FruitHasFinishedLifeTime(){
		if( !IsTheGoalFruit){
			controller.FruistHasFinishedTime(oscSender, positionIndex);
		}else{
			controller.GoalFruitHasFinishedTime(oscSender, positionIndex);
		}
		//oscSender.SendData(positionIndex, -1f);
		Destroy(gameObject, 0.1f);

	}


	void OnTriggerExit(Collider collider){
		timeStayingCollision = 0f;
	}
	public void PlaySpawnSound(){
		audio = gameObject.GetComponent<AudioSource>();
		audio.PlayOneShot (SoundSpawn);

	}

	public void SetFruitAsGoal(bool isGoal){
		isBananeMode = true;
		IsTheGoalFruit = isGoal;


	}
    

}
