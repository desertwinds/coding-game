using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonScript : MonoBehaviour {

	public int level;
	public GameObject loadingImage;
	public Text text;

	public void OnClick(){
		loadingImage.SetActive (true);
		PlayerPrefs.SetInt ("currentLevel", level);
		SceneManager.LoadScene (1);
	}
}
