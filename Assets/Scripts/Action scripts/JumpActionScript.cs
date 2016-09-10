using UnityEngine;
using System.Collections;

public class JumpActionScript : PlayerActionsScript {

	public override IEnumerator Run(){
		yield return StartCoroutine( m_PlayerScript.jump ());
	}

	public override IEnumerator QuickRun(){
		m_PlayerScript.logicalJump ();
		yield return null;
	}

}
