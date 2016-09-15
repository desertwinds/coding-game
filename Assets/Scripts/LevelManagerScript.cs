using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManagerScript : MonoBehaviour {

	public int m_levels;
	public GameObject m_levelButton;
	public RectTransform scrollViewContent;
	public GameObject loadingImage;

	private GridLayoutGroup m_grid;

	void Awake () {
		GameObject[] levels = Resources.LoadAll<GameObject> ("Levels");
		m_levels = levels.Length;
		m_grid = scrollViewContent.GetComponent<GridLayoutGroup> ();
	}

	void Start(){
		GameObject button;
		LevelButtonScript buttonScript;
		float scrollContentHeight = scrollViewContent.sizeDelta.y;
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
