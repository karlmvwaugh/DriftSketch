using UnityEngine;
using System;
using System.Collections;

public class Drawer : BaseControl {
	public GameObject point; 
	public Scatterer scatterer; 
	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	// Update is called once per frame
	void Update () {
		if (IsTouch()){
			SpawnPoint(lastX, lastY);
		}
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

	public 	bool IsTouch(){
		lastX = -1f;
		lastY = -1f; 

		if (Input.GetMouseButtonDown (0)){
			lastX = Input.mousePosition.x;
			lastY = Input.mousePosition.y; 
			return true;
		}else if (Input.touchCount > 0) {
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