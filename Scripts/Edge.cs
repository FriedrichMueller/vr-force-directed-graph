using UnityEngine;
using System.Collections;


public class Edge : MonoBehaviour
{
	public LineRenderer rend;

	public Node node1;
	public Node node2;
	public float weight;

	public Node Other (Node ask)
	{
		if (ask == node1)
			return node2;
		else if (ask == node2)
			return node1;
	}

	public virtual void Update ()
	{
		Vector3 pos1 = node1.transform.position;
		Vector3 pos2 = node2.transform.position;

		GetComponent<LineRenderer> ().SetPosition (0, pos1);
		GetComponent<LineRenderer> ().SetPosition (1, pos2);
		transform.localPosition = Vector3.Lerp (pos1, pos2, 0.5f);
	}
}
