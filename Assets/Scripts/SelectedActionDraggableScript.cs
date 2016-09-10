using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectedActionDraggableScript : DraggableScript {

	public override void OnBeginDrag(PointerEventData eventData){

		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		m_DraggingAction = this.gameObject;

		m_DraggingAction.GetComponent<CanvasGroup>().blocksRaycasts = false;

		m_DraggingPlane = canvas.transform as RectTransform;

		setPlaceholder (this.transform.parent, this.transform.GetSiblingIndex ());

		m_DraggingAction.transform.SetParent( this.transform.parent.parent.parent.parent );

	}




}
