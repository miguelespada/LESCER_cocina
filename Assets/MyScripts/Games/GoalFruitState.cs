using UnityEngine;
using System.Collections;

public class GoalFruitState :StateGame {
	
	GameObject[] fruitsSpawned;
	Transform[] spawnfruitsPoints;
	GameObject gameController;
	float thresholdTimeToSpawn;
	bool haveToSpawnFruit = true;
	private int numberOfFruitsCatched = 0;
	int numberOfFruitSpawned = 0;
	private int FruitSelected = 0;
	
	public GoalFruitState(GameObject[] fruits, Transform[] SpawnPoints, float minTimeSpawn,float maxTimeSpawn, GameObject gameControllerObject, int indexFruitGoal){
		fruitsSpawned = fruits;
		spawnfruitsPoints = SpawnPoints;
		minTimeForSpawn = minTimeSpawn;
		maxTimeFowSpawn = maxTimeSpawn;
		gameController = gameControllerObject;
		controller = gameController.GetComponent<GameModeController> ();
		thresholdTimeToSpawn = 4f;
		FruitSelected = indexFruitGoal;
	}   
	public override void OnStateBegin(){
		haveToSpawnFruit = true;
		numberOfFruitsCatched = 0;
		numberOfFruitSpawned = 0;
		TotalFruitsCatched = 0;
		
		
	}
	public override void Act(){
		if(haveToSpawnFruit ){
			TryToSpawnFruit();
			
		}
	}
	private void TryToSpawnFruit(){
	
		if (isTimeToSpawn ())
			SpawnNewFruit ();
		
	}
	
	public override void SpawnNewFruit ()
	{


        
		int positionOfSelectFruit = (int) Random.Range (0f, spawnfruitsPoints.Length);
		int positionDistractor = (int)Random.Range (0f, spawnfruitsPoints.Length);
		
		
		while(Mathf.Abs(positionDistractor - positionOfSelectFruit) < 4)
			positionDistractor = (int)Random.Range (0f, spawnfruitsPoints.Length);

		SpawnFruitInPlace(positionOfSelectFruit,FruitSelected, true);


		int indexfruitsInstantiated  = (int) Random.Range (0f, fruitsSpawned.Length);

		while(indexfruitsInstantiated == FruitSelected)
			indexfruitsInstantiated  = (int) Random.Range (0f, fruitsSpawned.Length);

		SpawnFruitInPlace(positionDistractor,indexfruitsInstantiated, false);


		/*
		for (int i =0; i< spawnfruitsPoints.Length; i++) {
			int indexfruitsInstantiated  = (int) Random.Range (0f, fruitsSpawned.Length);
			if(i == positionOfSelectFruit){
				SpawnFruitInPlace(i,FruitSelected, true);
			}else if(indexfruitsInstantiated == FruitSelected) {
				SpawnFruitInPlace(i,FruitSelected, true);

			}else{
				SpawnFruitInPlace(i,indexfruitsInstantiated, false);

			}
			numberOfFruitSpawned++;
		}*/

		haveToSpawnFruit = false;
	}
	private bool isTimeToSpawn(){
		timeWaitingforSpawn += Time.deltaTime;
		if (timeWaitingforSpawn > thresholdTimeToSpawn) {
			timeWaitingforSpawn = 0f;
			return true;
		} else {
			return false;
		}
	}
	public override void FruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		controller.DiplayMadeMistake ();
		oscSender.SendData(positionIndex, -1f);
		numberOfFruitsCatched++;

	}
	public override void FinishedTime(OSCsender oscSende, int positionIndex){
		//oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		numberOfFruitsCatched++;
	}
	public override void GoalFruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		controller.DisplayGoalHasReached ();
		oscSender.SendData(positionIndex, reactionTime);
		haveToSpawnFruit = true;
		controller.DeleteAllTheFruitofOtherDinamics ();
	}
	public override void GoalFinishedTime(OSCsender oscSende, int positionIndex){
		oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		haveToSpawnFruit = true;
		controller.DeleteAllTheFruitofOtherDinamics ();
	}
	
	void SpawnFruitInPlace(int indexPointSpawn, int indexFruit, bool isTheGoal){
		GameObject fruit = fruitsSpawned[indexFruit];
		controller.InstantiateFruitAsBananeMode (fruit, spawnfruitsPoints[indexPointSpawn],indexPointSpawn, isTheGoal);
		
	}
}
