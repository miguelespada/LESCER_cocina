using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 public enum GameGoalMode{FreeMode, TimerMode, TakingFruitMode}
public enum StatesID{onePerOne, twoPerTwo, aLotOfFruits, idle, findBanane, findFruitWithDistracts, findApple}

public class GameModeController : MonoBehaviour {
	List<GameObject>  FruitSpawned;
	public Transform[] SpawnPoints;
	public GameObject[] Fruits;
	public GameObject[] DistractObjects;
	public float minTimeToReSpawn;
	public float maxTimeToReSpawn;
	List<StateGame>  States;
	StateGame currentState;
	private bool sound = true;
	public FeedBackController feedBackController;
	GameObject OSCsenderContainer;
	OSCsender oscSender;
	public int fruitLifeTime = 10;
	public GameObject pauseGUI;

	bool pause = false;
	bool playHasBeenPressed = false;

	// Use this for initialization
	void Start () {
		States = new List<StateGame> ();
		FruitSpawned = new List<GameObject>();
		OSCsenderContainer = GameObject.Find("OSCsender");
		oscSender = OSCsenderContainer.GetComponent<OSCsender>();
		ConstructFSM ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!pause && playHasBeenPressed) {
			currentState.Act ();
			currentState.React ();
		}
		CheckForInputs ();

		if(currentState.ToString() != "LookAroundKitchenState")
		 CheckFourPause ();

		CheckForPlay ();
	}

	void CheckFourPause(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			if(pause){
				pause = false;
				pauseGUI.SetActive (false);
				Time.timeScale = 1;

			}else{
				Debug.Log (" Se pulsa Pausa");
				pause = true;
				pauseGUI.SetActive (true);
				Time.timeScale = 0;

			}
			oscSender.SendPause();

		}
	}

	void CheckForPlay(){
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (!playHasBeenPressed) {
				playHasBeenPressed = true;	
				currentState.OnStateBegin();
				oscSender.SendNewGame();
				Debug.Log ("Se pulsa Return");
			}
		}
	}
	
	void ConstructFSM(){
		StateGame OnePerOneState = new FruitsOnePerOneState (Fruits, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject);
	
		States.Add (OnePerOneState);

		StateGame twoPerTwoState = new TwoPerTwoState (Fruits, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject);
		States.Add (twoPerTwoState);
        
		StateGame ALotOfFruitState = new ALotOfFruitState (Fruits, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject);
		States.Add (ALotOfFruitState);

		StateGame IdleState = new LookAroundKitchenState ();
		States.Add (IdleState);

		StateGame FindGoalFruit = new GoalFruitState (Fruits, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject, 0);
		States.Add (FindGoalFruit);

		StateGame FindGoalFruitWithDistracts = new GoalFruitWithDistracts(Fruits, DistractObjects, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject);
		States.Add (FindGoalFruitWithDistracts);  

		StateGame FindGoalAppleFruit = new GoalFruitState (Fruits, SpawnPoints, minTimeToReSpawn, maxTimeToReSpawn, gameObject, 6);
		States.Add (FindGoalAppleFruit);



		currentState = States[3];
	}

	public void InstantiateFruit(GameObject Fruit, Transform trans,int indexPos){
		GameObject fruit = Instantiate (Fruit, trans.position,trans.rotation) as GameObject;
		Fruit fruitScript = fruit.GetComponent<Fruit> ();
		fruitScript.GameController = gameObject;
		fruitScript.positionIndex = indexPos;
		fruitScript.lifeTimeFruit = fruitLifeTime;
		fruitScript.SetFruitAsGoal (false);
		FruitSpawned.Add (fruit);
		if(sound)
		fruitScript.PlaySpawnSound ();

	}
	public void InstantiateFruitAsBananeMode(GameObject Fruit, Transform trans,int indexPos, bool isTheGoalFruit){

		GameObject fruit = Instantiate (Fruit, trans.position,trans.rotation) as GameObject;
		Fruit fruitScript = fruit.GetComponent<Fruit> ();
		fruitScript.GameController = gameObject;
		fruitScript.positionIndex = indexPos;
		fruitScript.lifeTimeFruit = fruitLifeTime;
		fruitScript.SetFruitAsGoal (isTheGoalFruit);
		FruitSpawned.Add (fruit);
		if(sound)
			fruitScript.PlaySpawnSound ();
		
	}

	public void FruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		currentState.FruitHasBeenCatched(positionIndex, reactionTime,oscSender);
		currentState.AddOnFruitToCatched ();

	}
	public void FruistHasFinishedTime(OSCsender oscSender, int positionIndex){
		currentState.FinishedTime(oscSender, positionIndex);
		currentState.AddOnFruitToCatched ();
	}

	public void GoalFruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		currentState.GoalFruitHasBeenCatched (positionIndex, reactionTime,oscSender);

	}

	public void GoalFruitHasFinishedTime(OSCsender oscSender, int positionIndex){
		currentState.GoalFinishedTime(oscSender, positionIndex);

	}
	public void CheckForInputs(){
        /*
		if (Input.GetKeyDown (KeyCode.Alpha9)) {
			sound = !sound;
		}
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			SetNewState(0);
		}
		if(Input.GetKeyDown (KeyCode.Alpha2)){
			SetNewState(1);
		}
		if(Input.GetKeyDown (KeyCode.Alpha3)){
			SetNewState(2);
		}
		if(Input.GetKeyDown (KeyCode.Alpha4)){
			SetNewState(3);
		}
		if(Input.GetKeyDown (KeyCode.Alpha5)){
			SetNewState(4);
		}
		if(Input.GetKeyDown (KeyCode.Alpha6)){
			SetNewState(5);
		}
		if(Input.GetKeyDown (KeyCode.Alpha7)){
			SetNewState(6);
		}
		if(Input.GetKeyDown (KeyCode.F)){
			GameHasFinished();
		}
        */


	}

	public void SetStateByID(StatesID stateID){
		Debug.Log ("LLega a SetStateByID");
		if (stateID == StatesID.idle)
			SetNewState (3);
		if (stateID == StatesID.onePerOne)
			SetNewState (0);
		if (stateID == StatesID.twoPerTwo)
			SetNewState (1);
		if (stateID == StatesID.aLotOfFruits)
			SetNewState (2);
		if (stateID == StatesID.findBanane)
			SetNewState (4);
		if (stateID == StatesID.findFruitWithDistracts)
			SetNewState (5);
		if (stateID == StatesID.findApple)
			SetNewState (6);
	}

	private void SetNewState(int newState){		
		Debug.Log ("!!!!");

		DeleteAllTheFruitofOtherDinamics();
		currentState = States[newState];


		//currentState.OnStateBegin();
		oscSender.SendFeedBackEjercicio(currentState.ToString());
		pause = false;
		playHasBeenPressed = false;
		pauseGUI.SetActive (false);
		Time.timeScale = 1;
		Debug.Log ("Nuevo stado instaurado: "+ currentState);
		
	}

	public void SetPlayGoal(GameGoalMode GoalMode,int value){
		currentState.SetGameGoal (GoalMode, value);

	}
	
	public void DeleteAllTheFruitofOtherDinamics(){
		StartCoroutine ("DeletFruits");

	}

	IEnumerator DeletFruits(){
		yield return new WaitForSeconds(0.1f);
		foreach(GameObject oldFruit in FruitSpawned)
		{
			Destroy(oldFruit);
		}
		FruitSpawned.Clear();



	}
	public bool thereAreFruitSpawned(int numbLastSpawen){
		if (FruitSpawned.Count >  numbLastSpawen) {
			return true;
		} else {
			return false;
		}

	}

	public void GameHasFinished(){
		Debug.Log ("Entra en funcion Finish");
		SetNewState (3);
		playHasBeenPressed = false;

	} 
	public void SwitchSound(bool On){
		if (On) {
			sound = true;
		} else {
			sound = false;
		}

	}

	public void DisplayGoalHasReached(){
		feedBackController.DisplayGoodJob ();
	}
	public void DiplayMadeMistake(){

		feedBackController.DisplayBadJob();
	}
	public void SendTimeOut(float TimeOut){
		oscSender.SendTimeOff (TimeOut);

	}
	public void SendFruitCatched(int fruit){
		oscSender.SendFruitsCatched (fruit);
		
	}
}
