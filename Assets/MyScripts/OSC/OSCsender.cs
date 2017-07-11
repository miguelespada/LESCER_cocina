using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class OSCsender : MonoBehaviour {
    public string UDPHost = "192.168.1.59";
    public int listenerPort = 8080;
    public int broadcastPort = 8001;
    private Osc oscHandler;
	public GameObject GameModeContainer;
	private GameModeController GameController;
	bool newMessage = false;
	int lvl = 0;
	int valueOfLvl = 10; 
	int lifeTimeFruit = 20;
	bool end = false;
	GameGoalMode goalmode;
    // Use this for initialization
    void Start () {
        UDPPacketIO udp = GetComponent<UDPPacketIO>();
        udp.init(UDPHost, broadcastPort, listenerPort);
        oscHandler = GetComponent<Osc>();
        oscHandler.init(udp);
		GameController = GameModeContainer.GetComponent<GameModeController>();
		oscHandler.SetAddressHandler("/start", SetStart);
		oscHandler.SetAddressHandler("/game",StartGame);
		oscHandler.SetAddressHandler("/sound",Sound);
		oscHandler.SetAddressHandler("/end", FinishGame);


    }
	void Update(){
	

		if (newMessage) {
			newMessage = false;
			SetLevel(lvl, goalmode, valueOfLvl, lifeTimeFruit);

		}
		if (end)
			EndGame ();


	}

	void SetLevel(int level, GameGoalMode goal, int value, int time){
		SetLevelByIndex(level);
		GameController.SetPlayGoal (goal, value);
		GameController.fruitLifeTime = time;
	}

	public void SendFruitsCatched(int FruitsCatched){
		/*Debug.Log (FruitsCatched);
		OscMessage oscM = Osc.StringToOscMessage("/currentFruits" + " " + FruitsCatched );
		oscHandler.Send(oscM);*/
		
	}

	public void SendTimeOff(float time){
		Debug.Log (time);
		OscMessage oscM = Osc.StringToOscMessage("/currentTime" + " " + time );
		oscHandler.Send(oscM);

	}
    public void SendData(int index, float time)
    {
        OscMessage oscM = Osc.StringToOscMessage("/reaction" + " " + index + " " + time);
        oscHandler.Send(oscM);
        Debug.Log("Reaction time : " + time + ". With index position of " + index);
    }
    public void SendOculusData(Transform trans, float horizontal)
    {
        float x = trans.parent.transform.position.x;
        float z = trans.parent.transform.position.z;
        OscMessage oscM = Osc.StringToOscMessage("/position" + " " + x + " " + z +
                                                        " " + trans.eulerAngles.x
                                                 + " " + trans.eulerAngles.y
                                                 + " " + trans.eulerAngles.z
                                                 + " " + horizontal
		                                         + " " + "Setas");
        oscHandler.Send(oscM);
    }

	public void SendPause(){
		Debug.Log ("Enviando /pause por OSC");
		OscMessage oscM = Osc.StringToOscMessage ("/pause");
		oscHandler.Send(oscM);
	}

	public void SendFeedBackEjercicio(string ejercicio){
		/*Debug.Log ("Enviando /variation  por OSC  "+ ejercicio);
		OscMessage oscM = Osc.StringToOscMessage ("/variation" + " " + ejercicio );
		oscHandler.Send(oscM);*/
	}
	public void SendNewGame(){
		Debug.Log ("Enviando /new por OSC");
		OscMessage oscM = Osc.StringToOscMessage ("/new");
		oscHandler.Send(oscM);
	}

    void OnApplicationQuit()
    {
        Debug.Log("Closing Osc");
        oscHandler.Cancel();
    }

	public void SetStart(OscMessage oscMessage)
	 {
		Debug.Log("Se recibe mensaje" +oscMessage.Values[0]);
	 } 

	//Método deprecated de antigua estructura de mensaje de OSC. Goal setado por etiqueta de mensaje. 
	public void StartTimeGame(OscMessage oscMessage)
	{   
		Debug.Log ("Entra en startTime");
		newMessage = true;
		lvl = (int)oscMessage.Values [0];
		valueOfLvl = (int)oscMessage.Values [1];
		lifeTimeFruit = (int)oscMessage.Values [2]; 
		goalmode = GameGoalMode.TimerMode;
	} 

	//Método implementado para iteración del 18.4.16. El mensaje de OSC tiene la estructura /game datoNoRelevante TipodeJuego VidaFruta
	//Ya no existe goalMode por tiempo o máximo de fruta, el final de la mecánica viene dado por el mensaje de OSC /end
	public void StartGame(OscMessage oscMessage){
		Debug.Log ("Entra en startGame");
		string message = "/variation" + " " + oscMessage.Values [0].ToString ();
		OscMessage oscM = Osc.StringToOscMessage (message);
		oscHandler.Send(oscM);
		newMessage = true;
		valueOfLvl = (int)oscMessage.Values [0];
		lvl = (int)oscMessage.Values [1];
		lifeTimeFruit = (int)oscMessage.Values [2]; 
		goalmode = GameGoalMode.TimerMode;


	}

	public void StartCountGame(OscMessage oscMessage)
	{   
		Debug.Log ("Entra en startCount");

		newMessage = true;
		lvl = (int)oscMessage.Values [0];
		valueOfLvl = (int)oscMessage.Values [1];
		lifeTimeFruit = (int)oscMessage.Values [2]; 
		goalmode = GameGoalMode.TakingFruitMode;
			
	} 



	public void Sound(OscMessage oscMessage){
		if ((int)oscMessage.Values [0] == 0) {
			GameController.SwitchSound (false);
		} else {
			GameController.SwitchSound (true);

		}

	}

	//Método asociado al encabezado de mensaje /end. Se encarga de poner la dinámica actual en Idle para finalizar la anterior.
	public void FinishGame(OscMessage oscMessage){
	
		Debug.Log ("Se recibe /end, termina Dinámica");
		end = true;
			
		
		
	}

	void EndGame(){
		end = false;
		GameController.GameHasFinished ();


	}
     private void SetLevelByIndex(int i){
		Debug.Log ("Entra a StateByIndex");

		if (i == 0)
			GameController.SetStateByID (StatesID.idle);
		if (i == 1)
			GameController.SetStateByID (StatesID.onePerOne);
		if (i == 2) {
			GameController.SetStateByID (StatesID.twoPerTwo);
		}

		if (i == 3)
			GameController.SetStateByID (StatesID.aLotOfFruits);
		if (i == 4)
			GameController.SetStateByID (StatesID.findBanane);
		if (i == 5)
			GameController.SetStateByID (StatesID.findFruitWithDistracts);
		if (i == 6)
			GameController.SetStateByID (StatesID.findApple);

	}
}
