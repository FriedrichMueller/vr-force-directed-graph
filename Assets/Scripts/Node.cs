using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Node : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	public Graph graph;

	public int id;
	public Text namefield;

	public bool calculate = false;
	public bool hidden;
	public bool hideconnections;
	public bool dontRepel;
	public bool mouseOver;

	public Vector3 savedPosition;
	public Vector3 velocity;
	public List<Node> repulsionlist = new List<Node> ();
	public List<Edge> attractionlist = new List<Edge> ();

	public static Node CreateNode (Graph graph, int id, string name)
	{
		GameObject newNodeGO = GameObject.Instantiate (GraphImporter.instance.nodePrefab) as GameObject;
		newNodeGO.transform.SetParent (graph.transform);
		newNodeGO.name = name;
		Node newNode = newNodeGO.GetComponent<Node> ();
		newNode.id = id;

		newNode.graph = graph;
		graph.nodes.Add (newNode);

		return newNode;
	}

	#region showandhide

	public void Hide ()
	{
		hidden = true;
		GetComponent<Renderer> ().enabled = false;
		GetComponent<SphereCollider> ().enabled = false;

		CanvasGroup cv = GetComponent<CanvasGroup> ();
		cv.alpha = 0;
		cv.blocksRaycasts = false;
		cv.interactable = false;
	}

	public void Show ()
	{
		hidden = false;
		GetComponent<Renderer> ().enabled = true;

		GetComponent<SphereCollider> ().enabled = true;

		CanvasGroup cv = GetComponent<CanvasGroup> ();
		cv.alpha = 1;
		cv.blocksRaycasts = true;
		cv.interactable = true;
		namefield.transform.localPosition = Vector3.zero;			
	}

	public void HideConnections ()
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
		HideConnections ();
		Invoke ("Destroy", 1.5f);
	}

	protected virtual void Destroy () // IS INVOKED
	{
		Destroy (gameObject);
	}

	#endregion

	#region events


	public virtual void OnPointerEnter (PointerEventData eventData)
	{
		Debug.Log ("Enter" + name);
	}

	public virtual void OnPointerExit (PointerEventData eventData)
	{
		Debug.Log ("Exit" + name);
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

	protected Vector3 CalcAttractionToCenter ()
	{
		Vector3 a = transform.position;
		Vector3 b = graph.center.transform.position;
		return (b - a).normalized * graph.attraction * (Vector3.Distance (a, b) / graph.springLength);
	}

	protected Vector3 CalcAttraction (Node otherNode, float weight)
	{
		if (otherNode) {
			Vector3 a = transform.localPosition;
			Vector3 b = otherNode.transform.localPosition;
			return (b - a).normalized * (graph.attraction + weight) * (Vector3.Distance (a, b) / graph.springLength);
		} else
			return Vector3.zero;
	}

	protected Vector3 CalcRepulsion (Node otherNode)
	{
		if (otherNode) {
			
			// Coulomb's Law: F = k(Qq/r^2)
			float distance = Vector3.Distance (transform.localPosition, otherNode.transform.localPosition);
			Vector3 returnvector = ((transform.localPosition - otherNode.transform.localPosition).normalized * graph.repulsion) / (distance * distance);

			if (!float.IsNaN (returnvector.x) && !float.IsNaN (returnvector.y) && !float.IsNaN (returnvector.z))
				return returnvector;
			else
				return Vector3.zero;
		} else
			return Vector3.zero;
	}

	#endregion physics


	public void RefreshRepulsionList ()
	{
		repulsionlist.Clear ();
		GameObject[] allnodes = GameObject.FindGameObjectsWithTag ("Node");
		foreach (GameObject go in allnodes) {
			if (go != gameObject)
				repulsionlist.Add (go.GetComponent<Node> ());
		}
		calculate = true;
	}

	public void Hit ()
	{
		mouseOver = true;
		Debug.Log (name + " is hit");
	}

	public void Reset ()
	{
		transform.localPosition = graph.center.transform.position + new Vector3 (Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
	}

	public void Start ()
	{
		namefield.text = name;
		Reset ();
	}

	public void Update ()
	{		
		if (calculate) {

			if (!mouseOver) {
				velocity = Vector3.zero;

				// REPULSION
				foreach (Node rn in repulsionlist)
					velocity += CalcRepulsion (rn);

				//ATTRACTION
				foreach (Edge e in attractionlist)
					velocity += CalcAttraction (e.Other (this), e.weight);

				//ATTRACTION TO CENTER
				velocity += CalcAttractionToCenter ();

				//APPLY FORCES
				if (!float.IsNaN (velocity.x) && !float.IsNaN (velocity.y) && !float.IsNaN (velocity.z)) {
					transform.localPosition += velocity * graph.damping * Time.deltaTime;
					savedPosition = transform.localPosition;
				} else
					Debug.LogError (name + " " + velocity.ToString ());
			} else {



			}

		}
			
	}

	public void LateUpdate ()
	{
		mouseOver = false;

	}



}
