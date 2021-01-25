using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerControl : NetworkBehaviour {
	public GameObject bullet = null;
	private Carrier curCarrier = null;
	private float moveSpeed = 3;
	private float rotSpeed = 90;

	void Start()
	{
		if(isLocalPlayer)
		{
			FollowCam cam = FindObjectOfType<FollowCam>();
			cam.target = transform;
		}
	}

	// Update is called once per frame
	void Update () {
		if(isLocalPlayer)
		{
			if(curCarrier==null)
			{
				float h = Input.GetAxis("Horizontal");
				float v = Input.GetAxis("Vertical");
				float r = Input.GetAxis("Mouse X");
				bool run = Input.GetButton("Fire3");
				CmdMove(h,v,run,r);
				if(Input.GetButtonDown("Fire1"))
				{
					CmdFire();				
				}
			}
			if(Input.GetKeyDown(KeyCode.F))
			{
				if(curCarrier)
				{
					CmdGetOffCarrier();
				}
				else
				{
					CmdGetOnCarrier();
				}
			}
		}
	}

	[Command]
	public void CmdFire()
	{
		NetworkServer.Spawn(Fire());
	}
	public GameObject Fire()
	{
		GameObject blt = GameObject.Instantiate(bullet,transform.position,transform.rotation);
		Bullet bltInfo = blt.GetComponent<Bullet>();
		bltInfo.owner = gameObject;
		blt.SetActive(true);
		return blt;
	}

	[Command]
	public void CmdMove(float h, float v, bool isRun, float r)
	{
		if(isServer && !isClient)
		{
			Move(h,v,isRun,r);
		}
		RpcMove(h,v,isRun,r);
	}
	[ClientRpc]
	public void RpcMove(float h, float v, bool isRun, float r)
	{
		Move(h,v,isRun,r);
	}
	public void Move(float h,float v,bool isRun, float r)
	{
		Vector3 deltaPos = new Vector3(h,0,v)*Time.deltaTime*moveSpeed*(isRun? 1.8f: 1);
		transform.Translate(deltaPos,Space.Self);
		Vector3 deltaRot = new Vector3(0,r,0)*Time.deltaTime*rotSpeed;
		transform.eulerAngles += deltaRot;
	}

	[Command]
	public void CmdGetOnCarrier()
	{
		int seat = -1;
		if(isServer)
		{
			Collider[] cols = Physics.OverlapSphere(transform.position, 1);
			List<Carrier> carriers = new List<Carrier>();
			foreach (Collider col in cols)
			{
				Carrier carrier = col.GetComponent<Carrier>();
				if (carrier != null)
				{
					carriers.Add(carrier);
				}
			}
			IEnumerator<Carrier> iter = carriers.GetEnumerator();
			while(iter.MoveNext() && (seat=GetOnCarrier(iter.Current))<0) { }
			//foreach (Carrier carrier in carriers)
			//{
			//	// Todo:添加距离判断，上距离近的
			//	// Todo:添加乘位判定
			//	seat = GetOnCarrier(carrier);
			//	if (seat>=0)
			//	{
			//		break;
			//	}
			//}
		}
		if(seat>=0)
		{
			RpcGetOnCarrier(curCarrier.GetComponent<NetworkIdentity>().netId);
		}
	}
	// RPC方法无法接受Carrier类型的参数，可能是无法接受复杂类型
	[ClientRpc]
	public void RpcGetOnCarrier(uint carrierId)
	{
		if(isClient && !isServer)
		{
			GetOnCarrier(NetworkIdentity.spawned[carrierId].gameObject.GetComponent<Carrier>());
		}
	}
	public int GetOnCarrier(Carrier carrier)
	{
		int seat = carrier.AddPassenger(this);
		if(seat>=0)
		{
			curCarrier = carrier;
		}
		return seat;
	}

	[Command]
	public void CmdGetOffCarrier()
	{
		int seat = -1;
		if(isServer)
		{
			seat=GetOffCarrier();
		}
		if(seat>=0)
		{
			RpcGetOffCarrier();
		}
	}
	[ClientRpc]
	public void RpcGetOffCarrier()
	{
		if(isClient && !isServer)
		{
			GetOffCarrier();
		}
	}
	public int GetOffCarrier()
	{
		int seat = -1;
		if(curCarrier)
		{
			seat= curCarrier.RemovePassenger(this);
			if(seat>=0)
			{
				curCarrier = null;
				return seat;
			}
		}
		return -1;
	}
/*
	[Command]
	public void CmdCarrierRun()
	{
		RpcCarrierRun();
		if(isServer && !isClient)
		{
			curCarrier.Run();
		}
	}
	[ClientRpc]
	public void RpcCarrierRun()
	{
		curCarrier.Run();
	}
*/
}
