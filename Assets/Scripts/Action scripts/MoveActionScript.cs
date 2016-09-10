using UnityEngine;
using System.Collections;

public class MoveActionScript : PlayerActionsScript {

	public override IEnumerator Run(){
		yield return StartCoroutine( m_PlayerScript.move ());
	}

	public override IEnumerator QuickRun(){
		m_PlayerScript.logicalMove ();
		yield return null;
	}

}
