using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SelectedActionsDropScript : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData eventData){
		if (eventData.pointerDrag == null)
			return;

		DraggableScript d = eventData.pointerDrag.GetComponent<DraggableScript> ();
		if (d.getPlaceholder() == null){
			Transform contentTransf = this.transform.GetChild (0);
			d.setPlaceholder (contentTransf, contentTransf.childCount);
		}
		
	}

	public void OnPointerExit(PointerEventData eventData){
		if (eventData.pointerDrag == null)
			return;
	}

	//Listener function called when the object is dropped on the UI. This will let the drag listener know that
	//the element should not be destroyed. And informs where the object should be placed at the end of the drag.
	public void OnDrop(PointerEventData eventData){
		DraggableScript d = eventData.pointerDrag.GetComponent<DraggableScript> ();
		if (d != null){
			d.m_DeleteAfterDrag = false;
			d.m_FinalDestination = this.transform;
		}
	}

}
