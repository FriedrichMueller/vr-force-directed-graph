using UnityEngine;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
	public static Graph instance;
    [SerializeField]
	public SteamVR_Controller.Device leftController;
    [SerializeField]
	public SteamVR_Controller.Device rightController;
	public GameObject center;
	public float repulsion;
	public float attraction;
	public float springLength;
	public float damping;

	public List<Node> nodes = new List<Node> ();
	public List<Edge> edges = new List<Edge> ();


	public void Awake ()
	{
		instance = this;
	}

	public void Update ()
	{
		//if (leftController.GetTouchDown (Valve.VR.EVRButtonId.k_EButton_DPad_Down) && repulsion > 1)
		//	repulsion -= 0.5f;
		//else if (leftController.GetTouchUp (Valve.VR.EVRButtonId.k_EButton_DPad_Up) && repulsion < 100)
		//	repulsion += 0.5f;
		//if (rightController.GetTouchDown (Valve.VR.EVRButtonId.k_EButton_DPad_Down) && repulsion > 1)
		//	repulsion -= 0.5f;
		//else if (rightController.GetTouchUp (Valve.VR.EVRButtonId.k_EButton_DPad_Up) && repulsion < 100)
		//	repulsion += 0.5f;		
	}

}