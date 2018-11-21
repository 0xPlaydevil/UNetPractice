using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestMsgPlayer : NetworkBehaviour {
	public Moto moto = null;

	void Start()
	{
		moto = GameObject.Find("MotoCube").GetComponent<Moto>();
	}

	
	// Update is called once per frame
	void Update () {
		if(isLocalPlayer && Input.GetKeyUp(KeyCode.A))
		{
			moto.members.curSpeed += 2;
			NetworkManager.singleton.client.Send(moto.motoMsg,moto.members);
		}
		if(Input.GetButtonUp("Jump"))
		{
			// Debug.LogFormat("{0}is server, {1}is client, {2}is localPlayer",isServer? "": "not",isClient? "": "not",isLocalPlayer? "": "not");
		}
		
	}
}
