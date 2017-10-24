using UnityEngine;
using System;
using System.Collections;

public class Drawer : BaseControl {
	public GameObject point; 
	public Scatterer scatterer; 

	private bool previousClick;
	private float previousX;
	private float previousY;
	
	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		InitBase(); 
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (IsTouch()){
			DealWithPrevious();
			SpawnPoint(lastX, lastY);
			PrepareForNext();
		} else {
			previousClick = false; 
		}
	}

	void DealWithPrevious(){
		if (previousPoint != null){
			var shareX = (previousPoint.previousX + lastX)/2.0f;
			var shareY = (previousPoint.previousY + lastY)/2.0f;
			SpawnPoint(shareX, shareY);
		}
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
		floating.Init(scatterer.getX(), scatterer.getY()); 
	}
}


public class BaseControl : MonoBehaviour{
	protected float lastX;
	protected float lastY;

	private bool mouseOn = false;
	private bool touchScreen;
	
	protected void InitBase() {
		touchScreen = Input.touchSupported;
	}
	
	public 	bool IsTouch(){
		lastX = -1f;
		lastY = -1f; 

		if (! touchScreen)
			return isMouseTouch();

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