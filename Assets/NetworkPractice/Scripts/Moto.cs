using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Moto : NetworkBehaviour {

	public class MotoMember: NetworkMessage
	{
		public GameObject target = null;
		public float curSpeed = 0;
	}

	public MotoMember members = new MotoMember();
	//public short motoMsg = MsgType.Highest + 1;
	//protected short motoMsgToClient = MsgType.Highest +2;

	void Start()
	{
		members.curSpeed = transform.position.z;
	}

	void Update()
	{
		Vector3 pos = transform.position;
		pos.z = members.curSpeed;
		transform.position = pos;
	}

	public void OnServerReceiveMembers(MotoMember recMem)
	{
		members = recMem;
		NetworkServer.SendToAll<MotoMember>(recMem);
		Debug.Log("Server send to all");
	}

	public void OnClientReceiveMembers(MotoMember recMem)
	{
		members = recMem;
	}


	public override void OnStartServer()
	{
		base.OnStartServer();
		NetworkServer.RegisterHandler<MotoMember>(OnServerReceiveMembers);
	}

	public override void OnStartClient()
	{
		Debug.Log("isServer" + (isServer? "true": "false"));
		if(!isServer)
		{
			base.OnStartClient();
			NetworkClient.RegisterHandler<MotoMember>(OnClientReceiveMembers);
			Debug.Log("Register");
		}
	}


}
