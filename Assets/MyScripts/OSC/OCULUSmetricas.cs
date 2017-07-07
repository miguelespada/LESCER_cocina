using UnityEngine;
using System.Collections;

public class OCULUSmetricas : MonoBehaviour {
    GameObject OSCsenderContainer;
    OSCsender oscSender;
    // Use this for initialization
    void Awake()
    {
        OSCsenderContainer = GameObject.Find("OSCsender");
        oscSender = OSCsenderContainer.GetComponent<OSCsender>();
    }
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
       oscSender.SendOculusData(transform, Input.GetAxis("Horizontal"));


    }
}
