using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour {

	public DoorScript[] m_doors;

	public void openDoors(){
		foreach (DoorScript door in m_doors) {
			door.openDoor ();
		}
	}

	public void restartDoors(){
		foreach (var door in m_doors) {
			door.closeDoor ();
		}
	}
}
