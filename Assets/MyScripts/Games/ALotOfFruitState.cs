using UnityEngine;
using System.Collections;

public class ALotOfFruitState : StateGame {

	GameObject[] fruitsSpawned;
	Transform[] spawnfruitsPoints;
	GameObject gameController;
	float thresholdTimeToSpawn;
	bool haveToSpawnFruit = true;
	private int numberOfFruitsCatched = 0;
	int numberOfFruitSpawned = 0;
	
	public ALotOfFruitState(GameObject[] fruits, Transform[] SpawnPoints, float minTimeSpawn,float maxTimeSpawn, GameObject gameControllerObject){
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
		timeWaitingforSpawn += Time.deltaTime;
		if (isTimeToSpawn ())
			SpawnNewFruit ();
		
	}
	
	public override void SpawnNewFruit ()
	{
		for (int i =0; i< spawnfruitsPoints.Length; i++) {
			if(Random.Range(0f,2f) > 1f){
				SpawnFruitInPlace(i);
				numberOfFruitSpawned++;

			}
		}
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
		numberOfFruitsCatched++;
		if (numberOfFruitsCatched >= numberOfFruitSpawned) {
			haveToSpawnFruit = true;
			numberOfFruitsCatched = 0;
			numberOfFruitSpawned = 0;
		}
	}
	public override void FinishedTime(OSCsender oscSende, int positionIndex){
		oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		numberOfFruitsCatched++;
		if (numberOfFruitsCatched >= numberOfFruitSpawned) {
			haveToSpawnFruit = true;
			numberOfFruitsCatched = 0;
			numberOfFruitSpawned = 0;
		}

	}
	private void SpawnFruitInRange(int minFruitValue, int maxFruitValue, int minIndexPoint, int maxIndexPoint){
		float indexFruit = Random.Range (minFruitValue, maxFruitValue);
		float indexSpawnPoint = Random.Range (minIndexPoint, maxIndexPoint);
		Transform spawnPoint = spawnfruitsPoints[(int)indexSpawnPoint];
		GameObject fruit = fruitsSpawned[(int)indexFruit];
		controller.InstantiateFruit (fruit, spawnPoint, (int)indexSpawnPoint);
		
	}

	void SpawnFruitInPlace(int indexPointSpawn){
		float indexFruit = Random.Range (0, fruitsSpawned.Length);
		GameObject fruit = fruitsSpawned[(int)indexFruit];
		controller.InstantiateFruit (fruit, spawnfruitsPoints[indexPointSpawn],indexPointSpawn);

	}
}
