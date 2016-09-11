using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject m_PlayerPrefab;
	public ActionManagerScript m_ActionManager;
	public GameObject m_level;
	public Transform m_possibleActionsHolder;
	public GameObject m_finalMessageWindow;

	private GameObject m_Player;
	private GameObject m_StartPoint;
	private PlayerScript m_PlayerScript;
	private bool m_LevelCompleted = false;
	private IEnumerator m_GameRoutine;
	private CameraScript m_cameraScript;
	private int m_currentLevel;
	private Text m_text;
	private GameObject m_continueButton;

	void Awake(){

		m_GameRoutine = GameLoop();
		m_currentLevel = PlayerPrefs.GetInt ("currentLevel");
		GameObject levelPrefab = Resources.Load<GameObject> ("Levels/Level" + m_currentLevel);
		m_level = Instantiate (levelPrefab) as GameObject;
		m_StartPoint = GameObject.FindGameObjectsWithTag ("Respawn")[0];
		m_cameraScript = gameObject.GetComponent<CameraScript> ();
		m_text = m_finalMessageWindow.transform.FindChild ("MessageText").GetComponent<Text>();
		m_continueButton = m_finalMessageWindow.transform.FindChild ("ContinueButton").gameObject;
	}

	// Use this for initialization
	void Start () {

		m_cameraScript.level = m_level;
		m_cameraScript.updateCameraPosition ();
		SpawnActions ();
		SpawnPlayer ();
		m_ActionManager.m_Player = m_Player;
		m_ActionManager.m_PlayerScript = m_PlayerScript;
		m_ActionManager.initialize(m_StartPoint.transform);


	}


	private void SpawnPlayer (){
		m_Player = Instantiate (m_PlayerPrefab, m_StartPoint.transform.position, m_StartPoint.transform.rotation) as GameObject;
		m_PlayerScript = m_Player.GetComponent<PlayerScript> ();
	}

	private void SpawnActions(){
		GameObject[] actions = m_level.GetComponent<LevelActionsScript> ().m_possibleActions;
		GameObject newAction;
		foreach (GameObject action in actions) {
			newAction = Instantiate (action, m_possibleActionsHolder, false) as GameObject;
		}
	}

	private IEnumerator GameLoop(){
		yield return StartCoroutine (LevelPlaying ());
		yield return StartCoroutine (LevelEnding ());
	}

	private IEnumerator quickRunLoop(){
		yield return StartCoroutine (m_ActionManager.quickRun ());
		yield return StartCoroutine (LevelEnding ());
	}

	private IEnumerator LevelPlaying(){
		
		//while(!reachedFinish() && !playerFell()){
			yield return StartCoroutine(m_ActionManager.run());
		//}
	}

	private IEnumerator LevelEnding(){
		m_text.text = "You are almost there. Try to get to the finish line.";
		m_LevelCompleted = false;

		if (playerFell ()) {
			m_text.text = "Woops try not to fall from the map.";
		}

		if(reachedFinish()){
			m_text.text = "Level " + m_currentLevel + " completed!";
			m_LevelCompleted = true;
			m_continueButton.SetActive (true);
		}

		m_finalMessageWindow.SetActive (true);

		yield return null;
	}

	private bool reachedFinish(){
		return m_PlayerScript.m_Finished;
	}

	private bool playerIsAlive(){
		return true;
	}

	private bool playerFell(){
		return m_PlayerScript.m_PlayerFell;
	}

	public void runScript(){
		StopAllCoroutines ();
		m_ActionManager.stopActions ();
		m_GameRoutine = GameLoop();
		restartPlayerPosition ();
		StartCoroutine (m_GameRoutine);
	}

	public void quickRunScript(){
		StopAllCoroutines ();
		m_ActionManager.stopActions ();
		m_GameRoutine = GameLoop();
		restartPlayerPosition ();
		StartCoroutine(this.quickRunLoop());
	}

	public void restartPlayerPosition(){
		m_Player.SetActive (false);
		m_Player.transform.position = m_StartPoint.transform.position;
		m_Player.transform.rotation = m_StartPoint.transform.rotation;
		m_Player.SetActive (true);
	}

	public void exitLevel(){
		SceneManager.LoadScene (0);
	}

	public void dismissMessage(){
		m_finalMessageWindow.SetActive (false);
	}
		
}
