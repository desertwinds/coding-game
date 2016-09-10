using UnityEngine;
using System.Collections;

public class TurnActionScript : PlayerActionsScript {

	public int direction;

	public override IEnumerator Run ()
	{
		yield return StartCoroutine (m_PlayerScript.RotateRoutine (direction));
	}

	public override IEnumerator QuickRun(){
		m_PlayerScript.logicalRotation (direction);
		yield return null;
	}
}
