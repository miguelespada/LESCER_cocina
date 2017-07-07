using UnityEngine;
using System.Collections;

public class GoalFruitWithDistracts :StateGame {
	
	GameObject[] fruitsSpawned;
	GameObject[] distractFruits;
	Transform[] spawnfruitsPoints;
	GameObject gameController;
	float thresholdTimeToSpawn;
	bool haveToSpawnFruit = true;
	private int numberOfFruitsCatched = 0;
	int numberOfFruitSpawned = 0;
	private int FruitSelected = 0;
	
	public GoalFruitWithDistracts(GameObject[] fruits, GameObject[] distractFruits, Transform[] SpawnPoints, float minTimeSpawn,float maxTimeSpawn, GameObject gameControllerObject){
		fruitsSpawned = fruits;
		this.distractFruits = distractFruits;
		spawnfruitsPoints = SpawnPoints;
		minTimeForSpawn = minTimeSpawn;
		maxTimeFowSpawn = maxTimeSpawn;
		gameController = gameControllerObject;
		controller = gameController.GetComponent<GameModeController> ();
		thresholdTimeToSpawn = 4f;
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

		SpawnFruitInPlace(positionOfSelectFruit,fruitsSpawned[Random.Range (0, fruitsSpawned.Length)], true);
		SpawnFruitInPlace(positionDistractor,distractFruits[Random.Range (0, distractFruits.Length)], false);

		/*for (int i =0; i< spawnfruitsPoints.Length; i++) {
			int indexfruitsInstantiated  = (int) Random.Range (0f, fruitsSpawned.Length);
			if(i == positionOfSelectFruit){
				SpawnFruitInPlace(i,fruitsSpawned[Random.Range (0, fruitsSpawned.Length)], true);

			}else{

				SpawnFruitInPlace(i,distractFruits[Random.Range (0, distractFruits.Length)], false);

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
		oscSender.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		numberOfFruitsCatched++;

	}
	public override void FinishedTime(OSCsender oscSende, int positionIndex){
		//oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		numberOfFruitsCatched++;
	}
	public override void GoalFruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){
		oscSender.SendData(positionIndex, reactionTime);
		controller.DisplayGoalHasReached ();
		haveToSpawnFruit = true;
		controller.DeleteAllTheFruitofOtherDinamics ();
	}
	public override void GoalFinishedTime(OSCsender oscSende, int positionIndex){
		oscSende.SendData(positionIndex, -1f);
		controller.DiplayMadeMistake ();
		haveToSpawnFruit = true;
		controller.DeleteAllTheFruitofOtherDinamics ();
	}
	
	void SpawnFruitInPlace(int indexPointSpawn, GameObject indexFruit, bool isTheGoal){
		GameObject fruit = indexFruit;
		controller.InstantiateFruitAsBananeMode (fruit, spawnfruitsPoints[indexPointSpawn],indexPointSpawn, isTheGoal);
		
	}
}
