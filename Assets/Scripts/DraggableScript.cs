using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public Transform m_FinalDestination = null;			//Transform where the dragged object will be placed.
	public GameObject m_SelectedActionPrefab;			//The prefab of the element that is draggable.
	public bool m_DeleteAfterDrag = true;				//Check's if the object was placed in the right position.

	protected GameObject m_DraggingAction;				//The instance of the element dragged.
	protected RectTransform m_DraggingPlane;			//No idea.
	protected GameObject m_Placeholder;					//Placeholder of the object when dragged around once it is a selected action.
	protected Transform m_PlaceholderParent;				//The position in which the place holder is located.

	public virtual void OnBeginDrag(PointerEventData eventData){

		//We locate the canvas where the element can be dragged around.
		var canvas = FindInParents<Canvas>(gameObject);	
		if (canvas == null)
			return;

		//We instantiate the element to be dragged.
		m_DraggingAction = Instantiate (m_SelectedActionPrefab, canvas.transform, false) as GameObject;

		//Set the text of the element according to the type of action that it is.
		Text text = m_DraggingAction.transform.GetChild(0).GetComponent<Text> ();
		text.text = transform.GetChild(0).GetComponent<Text> ().text;

		//Add the respective action.
		CopyComponent(this.GetComponent<PlayerActionsScript>(), m_DraggingAction);
		//m_DraggingAction.AddComponent  (gameObject.GetComponent<PlayerActionsScript> ().GetType ());



		//Set the element as the last sibling in the canvas and block the raycast to let the drop listener work.
		m_DraggingAction.transform.SetAsLastSibling ();
		m_DraggingAction.GetComponent<CanvasGroup>().blocksRaycasts = false;

		m_DraggingPlane = canvas.transform as RectTransform;

	}

	//This function is the responsible of dragging the object according to the position of the mouse.
	protected void SetDraggedPosition(PointerEventData eventData){
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null)
			m_DraggingPlane = eventData.pointerEnter.transform as RectTransform;

		var rt = m_DraggingAction.GetComponent<RectTransform> ();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos))
		{
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

	public virtual void OnDrag(PointerEventData eventData){
		if (m_DraggingAction != null)
			SetDraggedPosition(eventData);

		if(m_Placeholder != null){
			int newSiblingIndex = m_PlaceholderParent.childCount;

			for(int i=0; i < m_PlaceholderParent.childCount; i++) {
				if(m_DraggingAction.transform.position.y > m_PlaceholderParent.GetChild(i).position.y) {

					newSiblingIndex = i;

					if(m_Placeholder.transform.GetSiblingIndex() < newSiblingIndex)
						newSiblingIndex--;

					break;
				}
			}

			m_Placeholder.transform.SetSiblingIndex(newSiblingIndex);
		}
	}

	//Function called at the end of the drag. If the element is placed in the drop zone it will be saved and added to that UI list.
	//Else it will be destroyed.
	public virtual void OnEndDrag(PointerEventData eventData){

		if(m_DeleteAfterDrag){
			Destroy (m_DraggingAction);
		}

		else{
			setContentHeight ();
			m_DraggingAction.transform.SetParent (m_PlaceholderParent);
			m_DraggingAction.transform.SetSiblingIndex (m_Placeholder.transform.GetSiblingIndex ());
			m_DraggingAction.GetComponent<CanvasGroup>().blocksRaycasts = true;
		}

		m_DeleteAfterDrag = true;

		if (m_Placeholder != null)
			Destroy (m_Placeholder);

	}

	private void setContentHeight(){
		GridLayoutGroup gridLayout = m_PlaceholderParent.GetComponent<GridLayoutGroup>();
		RectTransform scrollContent = m_PlaceholderParent.GetComponent<RectTransform> ();
		if(scrollContent.sizeDelta.y/gridLayout.cellSize.y > (gridLayout.transform.childCount + 5))
			return;
		float scrollContentHeight = ((gridLayout.transform.childCount + 4) * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
		scrollContent.sizeDelta = new Vector2(0, scrollContentHeight);
	}

	//Function that searches the parents tree until reaches the element T or none is found.
	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;

		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}

	public void setPlaceholder(Transform parent, int index){

		m_Placeholder = new GameObject ();
		m_Placeholder.transform.SetParent (parent);
		LayoutElement le = m_Placeholder.AddComponent<LayoutElement> ();
		le.preferredWidth = this.GetComponent<LayoutElement> ().preferredWidth;
		le.preferredHeight = this.GetComponent<LayoutElement> ().preferredHeight;
		le.flexibleWidth = 0;
		le.flexibleHeight = 0;

		m_Placeholder.transform.SetSiblingIndex (index);

		m_PlaceholderParent = parent;
		
	}
		

	public GameObject getPlaceholder(){
		return this.m_Placeholder;
	}

	Component CopyComponent(Component original, GameObject destination)
	{
		System.Type type = original.GetType();
		Component copy = destination.AddComponent(type);
		// Copied fields can be restricted with BindingFlags
		System.Reflection.FieldInfo[] fields = type.GetFields(); 
		foreach (System.Reflection.FieldInfo field in fields)
		{
			field.SetValue(copy, field.GetValue(original));
		}
		return copy;
	}


}
