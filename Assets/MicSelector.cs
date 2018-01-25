using UnityEngine;
using System.Collections;

public class MicSelector : MonoBehaviour {
	private int i = 0;
	private int c = 0;
	private bool debug = false;
	private Rect centre;
	// Use this for initialization
	void Start () {
		centre = new Rect(Screen.width/2, Screen.height/2, Screen.width/2, Screen.height/2);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("m")){
			i++;
			c = Microphone.devices.Length;
			i = i % c;
		}

		if (Input.GetKeyDown("d")){
			debug = !debug;
		}
	}

	void OnGUI(){
		if (debug){
			GUI.Label (centre, "Currently " + i + " of " + c);
		}
	}

	public string GetMicName(){
		var deviceList = Microphone.devices;
		var newI = i % deviceList.Length;
		return deviceList[i];
	}
}
