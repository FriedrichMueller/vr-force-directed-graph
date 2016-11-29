using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Renderer rend;

	public Text namefield;

	public bool hideconnections;
	public bool dontRepel;
	public bool toBeDestroyed;

	public List<Node> repulsionlist = new List<Node> ();
	public List<Edge> attractionlist = new List<Edge> ();


	#region showandhide

	protected override void Hide ()
	{
		hidden = true;
		rend.enabled = false;
		GetComponent<SphereCollider> ().enabled = false;

		CanvasGroup cv = GetComponent<CanvasGroup> ();
		cv.alpha = 0;
		cv.blocksRaycasts = false;
		cv.interactable = false;
	}

	protected override void Show ()
	{
		hidden = false;
		rend.enabled = true;

		GetComponent<SphereCollider> ().enabled = true;

		CanvasGroup cv = GetComponent<CanvasGroup> ();
		cv.alpha = 1;
		cv.blocksRaycasts = true;
		cv.interactable = true;

		if (ui.navigation.threeD.isOn)
			namefield.transform.localPosition = Vector3.zero;
	}

	protected void HideConnections ()
	{
		hideconnections = true;
	}

	public void ShowConnections ()
	{
		hideconnections = false;
	}

	public void Remove ()
	{
		dontRepel = true;
		toBeDestroyed = true;
		HideConnections ();
		Invoke ("Destroy", 1.5f);
	}

	protected virtual void Destroy () // IS INVOKED
	{
		Destroy (gameObject);
	}

	#endregion

	#region events

	public virtual bool MouseOver ()
	{
		return false;
	}

	public virtual bool Selected ()
	{
		return false;
	}

	public virtual void OnPointerEnter (PointerEventData eventData)
	{

	}

	public virtual void OnPointerExit (PointerEventData eventData)
	{
		
	}

	public virtual void OnPointerClick (PointerEventData eventData)
	{
		
	}

	public virtual void OnBeginDrag (PointerEventData eventData)
	{
	}

	public virtual void OnDrag (PointerEventData eventData)
	{
		//transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
	}

	public virtual void OnEndDrag (PointerEventData eventData)
	{
		
	}

	#endregion events


	#region physics

	protected Vector3 CalcAttractionToCenter (float force)
	{
		Vector3 dir = -transform.position.normalized * force;
		Vector3 returnvector = dir * force;

		if (!main.ui.navigation.threeD.isOn && Mathf.Abs (transform.localPosition.z) > 1)
			returnvector += new Vector3 (0, 0, Mathf.Max (1, Mathf.Abs (transform.localPosition.z)) - 0) * Mathf.Sign (dir.z) * dia.zforce;

		return returnvector;
	}

	protected Vector3 CalcAttraction (Node otherNode, float weight)
	{
		if (otherNode) {
			Vector3 a = transform.position;
			Vector3 b = otherNode.transform.position;
			return (b - a).normalized * (dia.attractionNodes + weight) * Mathf.Log (Vector3.Distance (a, b) / dia.springLength);
		} else
			return Vector3.zero;
	}

	protected Vector3 CalcRepulsion (Node otherNode)
	{
		if (otherNode) {
			
			// Coulomb's Law: F = k(Qq/r^2)
			float distance = Vector3.Distance (transform.position, otherNode.transform.position);
			Vector3 returnvector = ((transform.position - otherNode.transform.position).normalized * dia.repulsionNodes) / (distance * distance);

			return returnvector;
		} else
			return Vector3.zero;
	}

	#endregion physics


	public void Reset ()
	{
		transform.localPosition = new Vector3 (Random.range, Random.range, Random.range) * 5;
	}


	public void Update ()
	{		
		velocity = Vector3.zero;

		if (transform.localPosition.magnitude > 5000 || transform.localPosition.x == 0f & transform.localPosition.y == 0f)
			Reset ();

		// REPULSION
		foreach (Node rn in repulsionlist)
			if (rn && !dontRepel && !rn.dontRepel)
				velocity += CalcRepulsion (rn);
			else if (rn && dontRepel)
				velocity += CalcRepulsion (rn);

		//ATTRACTION
		foreach (Edge e in attractionlist)
			if (!dontRepel && e.Other (this) && !e.Other (this).dontRepel)
				velocity += CalcAttraction (e.Other (this), e.weight);

		velocity += CalcAttractionToCenter (dia.attractionNodes);
	}

}
