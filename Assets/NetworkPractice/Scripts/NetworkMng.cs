using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkMng : MonoBehaviour {
	NetworkManagerHUD hud = null;

	// Use this for initialization
	void Start () {
		hud = GetComponent<NetworkManagerHUD>();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = Cursor.lockState!=CursorLockMode.Locked;
		Config config = Config.instance;
		if(config!=null)
		{
			JSONObject joClient = config.GetJsonObj("NetConfig/isClient");
			if(joClient!=null && joClient.IsBool)
			{
				NetworkManager net = NetworkManager.singleton;
				if(joClient.b)
				{
					JSONObject joIP = config.GetJsonObj("NetConfig/IP");
					if(joIP!=null && joIP.IsString)
					{
						net.networkAddress = joIP.str;
						net.StartClient();
						hud.showGUI = false;
					}
				}
				else
				{
					net.StartHost();
					hud.showGUI = false;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.U))
		{
			hud.showGUI = !hud.showGUI;
		}
		if(Input.GetKeyUp(KeyCode.L))
		{
			Cursor.lockState = 1-Cursor.lockState;
			Cursor.visible = Cursor.lockState!=CursorLockMode.Locked;
		}
	}
}
