using UnityEngine;
using System;
using System.Collections;

public class Drawer : BaseControl {
	public GameObject point; 
	public Scatterer scatterer; 

	//private bool previousClick;
	//private float previousX;
	//private float previousY;
	private PreviousPoint previousPoint;

	private class PreviousPoint {
		public float previousX;
		public float previousY;
	}

	// Use this for initialization
	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (IsTouch()){
			DealWithPrevious();
			SpawnPoint(lastX, lastY);
			previousPoint = new PreviousPoint(){ previousX = lastX, previousY = lastY}; 
		} else {
			previousPoint = null;
		}
	}

	void DealWithPrevious(){
		if (previousPoint != null){
			var shareX = (previousPoint.previousX + lastX)/2.0f;
			var shareY = (previousPoint.previousY + lastY)/2.0f;
			SpawnPoint(shareX, shareY);
		}
	}

	//void PrepareForNext(){
	//	previousClick = true; 
	//	previousX = lastX;
	//	previousY = lastY;
	//}

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

	public 	bool IsTouch(){
		lastX = -1f;
		lastY = -1f; 

		if (Input.GetMouseButtonDown (0)){
			mouseOn = !mouseOn; 
		}

		if (mouseOn){
			lastX = Input.mousePosition.x;
			lastY = Input.mousePosition.y; 
			return true;
		}

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