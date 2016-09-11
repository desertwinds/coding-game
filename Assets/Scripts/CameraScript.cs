using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour {

	Vector3 lastFramePosition;
	Vector3 origin = new Vector3(0,0,0);

	int m_furthestPositionX;
	int m_furthestPositionY;

	float m_minSize = 0f;
	float m_maxSize = 1f;

	public GameObject level;

	public Dictionary<TupleI, GameObject> m_Map { get; private set;}


	// Use this for initialization
	void Start () {

		Camera cam = GetComponent<Camera> ();
		cam.ScreenToWorldPoint (new Vector3 (0f, Screen.height * 2f, 0f));
		m_Map = new Dictionary<TupleI, GameObject> ();
		//updateCameraPosition ();
	}

	void Update(){
		Vector3 currFramePosition = (Input.mousePosition);

		if (Input.GetMouseButton(1) || Input.GetMouseButton(2)){
			
			Vector3 diff = lastFramePosition - currFramePosition;
			//Camera.main.transform.Translate (diff);
			Camera.main.transform.RotateAround (origin, Vector3.up, diff.x * 0.5f);
		}

		float mouseScroll = Input.GetAxis ("Mouse ScrollWheel");
		if(mouseScroll > 0f && Camera.main.orthographicSize <= m_maxSize){
			Camera.main.orthographicSize += .5f;
		}
		if(mouseScroll < 0f && Camera.main.orthographicSize >= m_minSize){
			Camera.main.orthographicSize -= .5f;
		}



		lastFramePosition =  (Input.mousePosition);
	}

	public void updateCameraPosition(){

		Vector3 max_x = origin;
		Vector3 min_x = origin;
		Vector3 max_z = origin;
		Vector3 min_z = origin;
		int position_x;
		int position_z;
		TupleI tilePosition;
		foreach(Transform trans in level.GetComponentInChildren<Transform>()){
			position_x = (int) Mathf.Floor (trans.position.x);
			position_z = (int) Mathf.Floor (trans.position.z);
			tilePosition = new TupleI (position_x, position_z);
			m_Map.Add (tilePosition, trans.gameObject);
			if(trans.position.x > max_x.x){
				max_x = trans.position;
			}
			if(trans.position.x < min_x.x){
				min_x = trans.position;
			}

			if(trans.position.z > max_z.z){
				max_z = trans.position;
			}
			if(trans.position.z < min_z.z){
				min_z = trans.position;
			}
		}

		float distance_x = Vector3.Distance(max_x, min_x);
		float distance_z = Vector3.Distance(max_z, min_z);

		m_minSize = Mathf.Max (m_minSize, Mathf.Abs (distance_x));

		m_minSize = Mathf.Max (m_minSize, Mathf.Abs (distance_z));

		Camera.main.orthographicSize = m_minSize;

		m_maxSize = 2 * m_minSize;
	}

}
