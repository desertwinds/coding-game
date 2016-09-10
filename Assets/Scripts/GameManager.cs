using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject m_PlayerPrefab;
	public Text m_Text;
	public ActionManagerScript m_ActionManager;

	private GameObject m_Player;
	private GameObject m_StartPoint;
	private PlayerScript m_PlayerScript;
	private bool m_LevelCompleted = false;
	private IEnumerator m_GameRoutine;


	// Use this for initialization
	void Start () {

		m_StartPoint = GameObject.FindGameObjectsWithTag ("Respawn")[0];
		SpawnPlayer ();
		m_ActionManager.m_Player = m_Player;
		m_ActionManager.m_PlayerScript = m_PlayerScript;
		m_ActionManager.initialize(m_StartPoint.transform);
		m_Text.text = "";
		m_Text.gameObject.SetActive (false);
		m_GameRoutine = GameLoop();

	}

	private void SpawnPlayer (){
		m_Player = Instantiate (m_PlayerPrefab, m_StartPoint.transform.position, m_StartPoint.transform.rotation) as GameObject;
		m_PlayerScript = m_Player.GetComponent<PlayerScript> ();
	}

	private IEnumerator GameLoop(){
		yield return StartCoroutine (LevelPlaying ());
		yield return StartCoroutine (LevelEnding ());
	}

	private IEnumerator LevelPlaying(){
		
		//while(!reachedFinish() && !playerFell()){
			yield return StartCoroutine(m_ActionManager.run());
		//}
	}

	private IEnumerator LevelEnding(){
		m_Text.text = "Yay finished!";
		m_LevelCompleted = true;

		if (playerFell ()) {
			m_Text.text = "Woops try not to fall from the level.";
			m_LevelCompleted = false;
		}

		m_Text.gameObject.SetActive (true);

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
		StartCoroutine(m_ActionManager.quickRun());
	}

	public void restartPlayerPosition(){
		m_Player.SetActive (false);
		m_Player.transform.position = m_StartPoint.transform.position;
		m_Player.transform.rotation = m_StartPoint.transform.rotation;
		m_Player.SetActive (true);
	}
}
