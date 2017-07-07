using UnityEngine;
using System.Collections;

public class StateGame {
	 protected float minTimeForSpawn;
	 protected float maxTimeFowSpawn;
	 protected float timeWaitingforSpawn;
	protected int maxTime;
	protected float currentTime;
	protected int maxNumFruits;
	delegate bool IsTheDInamicFinish();
	IsTheDInamicFinish isTheDinamicFinish;
	protected GameModeController controller;
	protected int TotalFruitsCatched = 0;


	public virtual void Act (){}
	public virtual void React(){
		if (isTheDinamicFinish != null) {

			//Cambios de requerimientos: ya no se comprueba si termina la dinámica, la función GameHasFinished()
			// se llama de forma externa. Descomentar si se quiere volver  a incluir dinámica

			/*if (isTheDinamicFinish ()) {
				controller.GameHasFinished ();

			}*/
		}

	}
	public virtual void OnStateBegin(){}
	public virtual void SpawnNewFruit(){}
	public virtual void FruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){}
	public virtual void FinishedTime (OSCsender oscSende,int positionIndex){}
	public virtual void GoalFinishedTime (OSCsender oscSende, int positionIndex){}
	public virtual void  GoalFruitHasBeenCatched(int positionIndex,float  reactionTime,OSCsender oscSender){}



	protected bool IsTimeToFinish(){

		if(currentTime > maxTime){
			currentTime = 0f;
			return true;
			
		}else{
			currentTime += Time.deltaTime;
			controller.SendTimeOut(maxTime - currentTime);
			return false;
		}
		
	}

	protected bool HasBeenCatchedAllTheFruits(){

		if(TotalFruitsCatched > maxNumFruits){
			TotalFruitsCatched = 0;
			return true;
			
		}else{
			return false;
		}


	}
	
	public void SetGameGoal(GameGoalMode GoalGame, int value){
		currentTime = 0f;
		TotalFruitsCatched = 0;	

		if (GoalGame == GameGoalMode.TimerMode) {
			isTheDinamicFinish = null;
			maxTime = value;
			currentTime = 0f;
			isTheDinamicFinish = IsTimeToFinish;

		} else if (GoalGame == GameGoalMode.TakingFruitMode) {
			isTheDinamicFinish = null;
			isTheDinamicFinish = HasBeenCatchedAllTheFruits;
			TotalFruitsCatched = 0;		
			maxNumFruits = value;

		} else if (GoalGame == GameGoalMode.FreeMode) {
			isTheDinamicFinish = null;
			TotalFruitsCatched = 0;		
			maxNumFruits = value;
		}
		
	}
	public void AddOnFruitToCatched(){

		TotalFruitsCatched++;
		controller.SendFruitCatched (maxNumFruits - TotalFruitsCatched);


	}
}
