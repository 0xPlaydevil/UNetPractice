using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Moto : NetworkBehaviour {

	public class MotoMember: MessageBase
	{
		public GameObject target = null;
		public float curSpeed = 0;
	}

	public MotoMember members = new MotoMember();
	public short motoMsg = MsgType.Highest + 1;
	protected short motoMsgToClient = MsgType.Highest +2;

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

	public void OnServerReceiveMembers(NetworkMessage msg)
	{
		MotoMember recMem = msg.ReadMessage<MotoMember>();
		members = recMem;
		NetworkServer.SendToAll(motoMsgToClient,recMem);
		Debug.Log("Server send to all");
	}

	public void OnClientReceiveMembers(NetworkMessage msg)
	{
		MotoMember recMem = msg.ReadMessage<MotoMember>();
		members = recMem;
	}


	public override void OnStartServer()
	{
		base.OnStartServer();
		NetworkServer.RegisterHandler(motoMsg,OnServerReceiveMembers);
	}

	public override void OnStartClient()
	{
		Debug.Log("isServer" + (isServer? "true": "false"));
		if(!isServer)
		{
			base.OnStartClient();
			NetworkManager.singleton.client.RegisterHandler(motoMsgToClient,OnClientReceiveMembers);
			Debug.Log("Register");
		}
	}


}
