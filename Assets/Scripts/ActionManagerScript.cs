using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionManagerScript : MonoBehaviour {

	public bool m_KeepRunning = true;
	public GameObject m_SelectedActions;

	public GameObject m_Player;
	public PlayerScript m_PlayerScript;
	private ArrayList m_PlayerActions;

	private int currentScriptPosition = 0;
	private TupleI m_StartPosition;
	private TupleI m_StartRotation;

	public void initialize(Transform position){
		int x = (int) Mathf.Floor(position.position.x);
		int z = (int) Mathf.Floor(position.position.z);
		m_StartPosition = new TupleI (x, z);

		x = (int)Mathf.Floor (position.rotation.x);
		z = (int)Mathf.Floor (position.rotation.z);
		m_StartRotation = new TupleI (x, z);

	}

	public IEnumerator run(){
		PlayerActionsScript script;
		getScripts ();
		int steps = m_PlayerActions.Count;
		int currentStep = 0;
		m_KeepRunning = true;
		while(m_KeepRunning && currentStep < steps){
			currentScriptPosition = currentStep;
			script = (m_PlayerActions [currentScriptPosition] as PlayerActionsScript);
			yield return StartCoroutine(script.Run ());
			currentStep++;
		}
		yield return null;
	}

	private void getScripts(){
		m_PlayerActions = new ArrayList ();
		foreach(PlayerActionsScript action in m_SelectedActions.GetComponentsInChildren<PlayerActionsScript>()){
			action.setPlayer (m_Player);
			m_PlayerActions.Add (action);
		}
	}

	public IEnumerator quickRun(){
		PlayerActionsScript script;
		getScripts ();
		calculateMap ();
		int steps = m_PlayerActions.Count;
		int currentStep = 0;
		m_KeepRunning = true;
		while(m_KeepRunning && currentStep < steps){
			currentScriptPosition = currentStep;
			script = (m_PlayerActions [currentScriptPosition] as PlayerActionsScript);
			yield return StartCoroutine(script.QuickRun ());
			currentStep++;
		}
		m_PlayerScript.endLogicalRun ();
		yield return null;
	}

	private void calculateMap(){
		
	}

	public void stopActions(){
		if (m_PlayerActions == null || m_PlayerActions.Count == 0)
			return;
		m_KeepRunning = false;
		StopAllCoroutines ();
		PlayerActionsScript script = m_PlayerActions [currentScriptPosition] as PlayerActionsScript;
		if(script)
			script.Stop ();
		if(currentScriptPosition > 0){
			script = m_PlayerActions [currentScriptPosition - 1] as PlayerActionsScript;
			if (script)
				script.Stop ();
		}
		m_PlayerActions.Clear ();
	}
}
