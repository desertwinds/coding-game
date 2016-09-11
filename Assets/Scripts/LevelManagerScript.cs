using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour {

	public int m_levels;
	public GameObject m_levelButton;
	public Transform scrollViewContent;
	public GameObject loadingImage;

	void Awake () {
		GameObject[] levels = Resources.LoadAll<GameObject> ("Levels");
		m_levels = levels.Length;
	}

	void Start(){
		GameObject button;
		LevelButtonScript buttonScript;
		for (int i = 1; i <= m_levels; i++) {
			button = Instantiate (m_levelButton);
			buttonScript = button.GetComponent<LevelButtonScript> ();
			buttonScript.level = i;
			buttonScript.text.text = "Level " + i;
			buttonScript.loadingImage = loadingImage;
			button.transform.SetParent (scrollViewContent);

		}
	}

	public void exitGame(){
		Application.Quit ();
	}
	

}
