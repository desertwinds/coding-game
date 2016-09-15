using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour {

	public float m_Speed = 12f;
	public bool m_Finished { get; private set;}
	public bool m_canMove { get; private set;}

	private Rigidbody m_RigidBody;

	public TupleI m_currentRotation { get; private set;}
	public TupleI m_currentPosition { get; private set;}
	public string m_playerMessage { get; private set;}

	//Logical angles tuples
	private TupleI zeroDegrees = new TupleI(0,1);
	private TupleI ninetyDegrees = new TupleI(1,0);
	private TupleI houndredAndEightyDegrees = new TupleI(0, -1);
	private TupleI twoSeventyDegrees = new TupleI(-1, 0);

	//Current map 
	private Dictionary<TupleI, GameObject> m_map;

	//Defined strings
	private string blockedLandingPath = "It seems that you would have landed on a blocked field.";
	private string outOfMap = "Woops try not to fall from the map.";
	private string blockedPath = "There seems to be an object blocking your way.";
	private string noSwitchFound = "There is no switch to activate.";

	// Use this for initialization
	void Awake () {
		m_RigidBody = GetComponent<Rigidbody> ();
		m_canMove = true;
	}

	private void OnEnable ()
	{
		//m_RigidBody.isKinematic = false;
		m_canMove = true;
		m_Finished = false;
		calculateLogicalRotation ((int) this.transform.rotation.eulerAngles.y);
		int x = (int) Mathf.Floor(transform.position.x);
		int z = (int) Mathf.Floor(transform.position.z);
		m_currentPosition = new TupleI (x, z);
		m_map = Camera.main.GetComponent<CameraScript> ().m_Map;
	}


	private void OnDisable ()
	{
		//m_RigidBody.isKinematic = true;
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag("Finish")){
			m_Finished = true;
		}
	}


	public IEnumerator move(){
		m_canMove = checkMove ();
		if (!m_canMove) {
			yield break;
		}
		
		Vector3 end = transform.position + transform.forward;

		Vector3 endWithoutY = end;
		endWithoutY.Set (end.x, 0f, end.z);
		Vector3 startWithoutY = transform.position;
		startWithoutY.Set (transform.position.x, 0f, transform.position.z);
		float sqrRemainingDistance = (startWithoutY - endWithoutY).sqrMagnitude;
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(m_RigidBody.position, end, m_Speed * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			m_RigidBody.MovePosition (newPostion);

			//Recalculate the remaining distance after moving.
			startWithoutY.Set (transform.position.x, 0f, transform.position.z);
			sqrRemainingDistance = (startWithoutY - endWithoutY).sqrMagnitude;

			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

	}

	public IEnumerator jump(){
		m_canMove = canJump ();
		if (!m_canMove) {
			yield break;
		}

		Vector3 end = transform.position + transform.forward;
		end = end + transform.up;

		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(m_RigidBody.position, end, m_Speed * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			m_RigidBody.MovePosition (newPostion);

			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

		yield return new WaitForFixedUpdate();

		end = transform.position - transform.up;
		end = end + transform.forward;

		sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(m_RigidBody.position, end, m_Speed * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			m_RigidBody.MovePosition (newPostion);

			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

		yield return new WaitForFixedUpdate();


	}

	public void logicalJump(){
		m_canMove = canJump ();
	}

	private bool checkTileAvailability(GameObject tile){
		if (tile.CompareTag ("Floor") || tile.CompareTag ("Switch"))
			return true;
		else if (tile.CompareTag("Door")){
			DoorScript door = tile.GetComponent<DoorScript> ();
			return door.m_isOpen;
		}
		else{
			return false;
		}
	}

	private bool canJump(){

		TupleI jumpAbove = m_currentPosition + m_currentRotation;
		TupleI landOn = jumpAbove + m_currentRotation;

		GameObject tile = null;
		m_map.TryGetValue (jumpAbove, out tile);
		//First see if the tile in front can be jumped over.
		if (tile == null || checkTileAvailability(tile)){
			tile = null;
			//Next see if the tile where it is supposed to land is available.
			if (m_map.TryGetValue (landOn, out tile)) {
				//Finally see if such tile is walkable.
				if (checkTileAvailability(tile)) {
					m_currentPosition = landOn;
					return true;
				}
				//This case means that we would land on a blocked tile.
				else {
					m_playerMessage = blockedLandingPath;
					return false;
				}
			} 
			//This means we are jumping into the void.
			else {
				m_playerMessage = outOfMap;
				return false;
			}
		}
		//There is something blocking our way and we can't jump it.
		else{
			m_playerMessage = blockedPath;
			return false;
		}
	}

	private bool checkMove(){
		TupleI newPosition = m_currentPosition + m_currentRotation;
		GameObject tile = null;
		if (m_map.TryGetValue (newPosition, out tile)){
			if (checkTileAvailability(tile)) {
				m_currentPosition = newPosition;
				return true;
			} else {
				m_playerMessage = blockedPath;
				return false;
			}
		}
		else{
			m_playerMessage = outOfMap;
			return false;
		}
	}



	private GameObject checkActivateSwitch(){
		GameObject tile = null;
		if(m_map.TryGetValue(m_currentPosition, out tile)){
			if(tile.CompareTag("Switch"))
				return tile;
		}
		m_playerMessage = noSwitchFound;
		m_canMove = false;
		return tile;
		
	}

	public void activateSwitch(GameObject switchTile){
		SwitchScript mySwitch = switchTile.GetComponent<SwitchScript> ();
		mySwitch.openDoors ();
	}

	public void logicalPressSwitch(){
		GameObject switchTile = checkActivateSwitch ();
		if(switchTile != null){
			activateSwitch (switchTile);
		}
	}

	public IEnumerator pressSwitchRoutine(){
		GameObject switchTile = checkActivateSwitch ();
		if (!m_canMove)
			yield break;

		Vector3 end = transform.position + transform.up;

		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(m_RigidBody.position, end, m_Speed * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			m_RigidBody.MovePosition (newPostion);

			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

		yield return new WaitForFixedUpdate();

		end = transform.position - transform.up;

		sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(m_RigidBody.position, end, m_Speed * Time.deltaTime);

			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			m_RigidBody.MovePosition (newPostion);

			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;

			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

		activateSwitch (switchTile);

		yield return new WaitForFixedUpdate();

	}

	public void logicalMove(){
		m_canMove = checkMove ();
	}

	public void logicalRotation(int direction){
		int currentAngle = rotationToAngle ();
		currentAngle += 360 + (direction * 90);
		currentAngle = currentAngle % 360;
		calculateLogicalRotation (currentAngle);
	}

	public IEnumerator RotateRoutine(int direction){

		//we also do a logical rotation here.
		logicalRotation(direction);

		//This rotation is too slow, needs more work maybe.
		for (int i = 0; i < 90; i+=5){
			yield return null;

			transform.Rotate (0, direction * 5, 0);
		}
	}

	public void endLogicalRun(){
		GameObject finalPosition;
		if (m_map.TryGetValue(m_currentPosition, out finalPosition)){
			this.transform.position = new Vector3 (finalPosition.transform.position.x, this.transform.position.y, finalPosition.transform.position.z);
			this.transform.rotation = Quaternion.Euler(0,rotationToAngle(), 0);
		}
	}

	public void stopAction(){
		StopAllCoroutines ();
	}

	private int rotationToAngle(){
		if (m_currentRotation.Equals (zeroDegrees))
			return 0;
		else if (m_currentRotation.Equals (ninetyDegrees))
			return 90;
		else if (m_currentRotation.Equals (houndredAndEightyDegrees))
			return 180;
		else if (m_currentRotation.Equals (twoSeventyDegrees))
			return 270;
		else{
			Debug.LogError ("Invalid rotation");
			return -1;
		}
			
	}
		
	private void calculateLogicalRotation(int angle){
		switch(angle){

		case 0:
			m_currentRotation = new TupleI (0, 1);
			break;
		case 90: 
			m_currentRotation = new TupleI (1, 0);
			break;
		case 180:
			m_currentRotation = new TupleI (0, -1);
			break;
		case 270:
			m_currentRotation = new TupleI (-1, 0);
			break;
		default:
			m_currentRotation = new TupleI (0, 0);
			Debug.LogError ("Error calculating");
			break;
		}
	}
		
}
