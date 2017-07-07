using UnityEngine;
using System.Collections;

public class FruitsOnePerOneState : StateGame {
	GameObject[] fruitsSpawned;
	Transform[] spawnfruitsPoints;
	GameObject gameController;
	float thresholdTimeToSpawn;
	bool haveToSpawnFruit = true;




	
	public FruitsOnePerOneState(GameObject[] fruits, Transform[] SpawnPoints, float minTimeSpawn,float maxTimeSpawn, GameObject gameControllerObject){
		fruitsSpawned = fruits;
		spawnfruitsPoints = SpawnPoints;
		minTimeForSpawn = minTimeSpawn;
		maxTimeFowSpawn = maxTimeSpawn;
		gameController = gameControllerObject;
		controller = gameController.GetComponent<GameModeController> ();
		thresholdTimeToSpawn = Random.Range (minTimeForSpawn, maxTimeFowSpawn);
	}   
	public override void OnStateBegin(){
		haveToSpawnFruit = true;
		TotalFruitsCatched = 0;

	}
	public override void Act(){
		if(haveToSpawnFruit ){
			TryToSpawnFruit();

		}
	}
	private void TryToSpawnFruit(){
		timeWaitingforSpawn += Time.deltaTime;
		if (isTimeToSpawn ())
			SpawnNewFruit ();
		
	}

	public override void SpawnNewFruit ()
	{
		float indexFruit = Random.Range (0, fruitsSpawned.Length);
		float indexSpawnPoint = Random.Range (0, spawnfruitsPoints.Length);
		Transform spawnPoint = spawnfruitsPoints[(int)indexSpawnPoint];
		GameObject fruit = fruitsSpawned[(int)indexFruit];
		controller.InstantiateFruit (fruit, spawnPoint, (int)indexSpawnPoint);
		haveToSpawnFruit = false;
	}
	 private bool isTimeToSpawn(){
		if(timeWaitingforSpawn >= thresholdTimeToSpawn){
			timeWaitingforSpawn = 0f;
			thresholdTimeToSpawn = Random.Range (minTimeForSpawn, maxTimeFowSpawn);
			return true;
		}else{
			return false;

		}
	}
	public override void FruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		controller.DisplayGoalHasReached ();
		oscSender.SendData(positionIndex, reactionTime);
		haveToSpawnFruit = true;
	}
	public override void FinishedTime(OSCsender oscSende, int positionIndex){
		oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		haveToSpawnFruit = true;
	}

}
