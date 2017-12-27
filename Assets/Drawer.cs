using UnityEngine;
using System;
using System.Collections;

public class Drawer : BaseControl {
	public GameObject point; 
	public Scatterer scatterer; 
	public AudioRecorder recorder;

	private bool previousClick;
	private float previousX;
	private float previousY;
	private int shares = 5;

	private bool currentlyBeingTouched = false;

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

	}
	
	// Update is called once per frame
	void Update () {
		if (IsTouch()){
			RecordingOn();
			DealWithPrevious();
			SpawnPoint(lastX, lastY);
			PrepareForNext();
		} else {
			RecordingOff();
			previousClick = false; 
		}
	}

	void RecordingOn(){
		if (! currentlyBeingTouched){
			currentlyBeingTouched = true;
			recorder.StartRecording();
		}
	}

	void RecordingOff(){
		if (currentlyBeingTouched){
			currentlyBeingTouched = false;
			recorder.StopRecording(scatterer.getDeathTime());
		}
	}


	void DealWithPrevious(){
		if (previousClick){
			for (var i =1; i<shares; i++){
				var shareX = getShare (previousX, lastX, i);
				var shareY = getShare (previousY, lastY, i);
				SpawnPoint(shareX, shareY);
			}

		}
	}

	float getShare(float first, float second, int step){
		return first + step*(second - first)/shares;
	}


	void PrepareForNext(){
		previousClick = true; 
		previousX = lastX;
		previousY = lastY;
	}

	void SpawnPoint(float x, float y){
		var go = (GameObject)Instantiate(point);
		var newCoord = Camera.main.ScreenToWorldPoint(new Vector3(x, y));
		go.transform.position = new Vector3(newCoord.x, newCoord.y, -1f); 
		var floating = go.GetComponent<Floating>();
		floating.Init(scatterer.getX(), scatterer.getY(), scatterer.getDeathTime()); 
	}
}


public class BaseControl : MonoBehaviour{
	protected float lastX;
	protected float lastY;

	private bool mouseOn = false;
	private bool touchScreen;
	

	
	public 	bool IsTouch(){
		lastX = -1f;
		lastY = -1f; 

		var isMouse = isMouseTouch();
		if (isMouse) return isMouse; 

		return isPhoneTouch(); 
	}
	
	private bool isMouseTouch(){		
		if (Input.GetMouseButton(0)){
			lastX = Input.mousePosition.x;
			lastY = Input.mousePosition.y; 
			return true;
		}
		
		return false; 
	}
	
	private bool isPhoneTouch(){
		if (Input.touchCount > 0) {
			Touch tou = Input.GetTouch(0);
			lastX = tou.position.x;
			lastY = tou.position.y; 
			return true;
		}

		return false;
	}
	
	
	void Start(){}
	
	void Update(){}
	
	
}