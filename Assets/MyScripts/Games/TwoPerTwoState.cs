using UnityEngine;
using System.Collections;

public class TwoPerTwoState: StateGame {

	GameObject[] fruitsSpawned;
	Transform[] spawnfruitsPoints;
	GameObject gameController;
	float thresholdTimeToSpawn;
	bool haveToSpawnFruit = true;
	private int numberOfFruitsCatched = 0;
	
	public TwoPerTwoState(GameObject[] fruits, Transform[] SpawnPoints, float minTimeSpawn,float maxTimeSpawn, GameObject gameControllerObject){
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
		Debug.Log ("Se instancian las nuevas frutas");
		SpawnFruitInRange (0, fruitsSpawned.Length, 0, spawnfruitsPoints.Length / 2);
		SpawnFruitInRange (0, fruitsSpawned.Length, spawnfruitsPoints.Length / 2, spawnfruitsPoints.Length);
		haveToSpawnFruit = false;
	}
	private bool isTimeToSpawn(){
		if(timeWaitingforSpawn >= thresholdTimeToSpawn){
			Debug.Log ("Supera el threshold de tiempo");
			timeWaitingforSpawn = 0f;
			thresholdTimeToSpawn = Random.Range (minTimeForSpawn, maxTimeFowSpawn);
			return true;
		}else{
			return false;
			
		}
	}
	public override void FruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		oscSender.SendData(positionIndex, reactionTime);
		controller.DisplayGoalHasReached ();
		numberOfFruitsCatched++;
		Debug.Log ("Se ha sumado uno más a numberofCatched,siendo ahora : " + numberOfFruitsCatched);

		if (numberOfFruitsCatched > 1 ) {
			Debug.Log ("Supera a 1, siendo ahora" + numberOfFruitsCatched);
			haveToSpawnFruit = true;
			numberOfFruitsCatched = 0;
		}
	}
	public override void FinishedTime(OSCsender oscSende, int positionIndex){
		oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		numberOfFruitsCatched++;
		Debug.Log ("Se ha sumado uno más a numberofCatched,siendo ahora : " + numberOfFruitsCatched);
		
		if (numberOfFruitsCatched > 1 ) {
			Debug.Log ("Supera a 1, siendo ahora" + numberOfFruitsCatched);
			haveToSpawnFruit = true;
			numberOfFruitsCatched = 0;
		}

	}
	private void SpawnFruitInRange(int minFruitValue, int maxFruitValue, int minIndexPoint, int maxIndexPoint){
		float indexFruit = Random.Range (minFruitValue, maxFruitValue);
		float indexSpawnPoint = Random.Range (minIndexPoint, maxIndexPoint);
		Transform spawnPoint = spawnfruitsPoints[(int)indexSpawnPoint];
		GameObject fruit = fruitsSpawned[(int)indexFruit];
		controller.InstantiateFruit (fruit, spawnPoint, (int)indexSpawnPoint);

	}
}
