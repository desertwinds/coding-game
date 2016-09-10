using UnityEngine;
using System.Collections;

public class PlayerActionsScript : MonoBehaviour {

	protected GameObject m_Player;
	protected PlayerScript m_PlayerScript;

	public void setPlayer(GameObject player){
		m_Player = player;
		m_PlayerScript = player.GetComponent<PlayerScript> ();
	}

	public virtual IEnumerator Run(){
		print ("haha");
		yield return null;
	}

	public virtual void Stop(){
		StopAllCoroutines ();
		//m_PlayerScript.stopAction ();
	}

	public virtual IEnumerator QuickRun(){
		yield return null;
	}
}
