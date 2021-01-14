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
			else
			{
				curCarrier.GetInput();
				NetworkClient.Send(curCarrier.msgContent);
				if(curCarrier.weaponFireBits !=0)
				{
					CmdCarrierFire(curCarrier.weaponFireBits);
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
					Collider[] cols = Physics.OverlapSphere(transform.position,1);
					List<Carrier> carriers = new List<Carrier>();
					foreach(Collider col in cols)
					{
						Carrier carrier = col.GetComponent<Carrier>();
						if(carrier!=null)
						{
							carriers.Add(carrier);
						}
					}
					foreach(Carrier carrier in carriers)
					{
						// Todo:添加距离判断，上距离近的
						// Todo:添加乘位判定
						if(carrier.driver==null)
						{
							CmdGetOnCarrier(carrier.gameObject);
							break;
						}
					}
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
	public void CmdGetOnCarrier(GameObject carrierObj)
	{
		RpcGetOnCarrier(carrierObj);
		if(isServer && !isClient)
		{
			GetOnCarrier(carrierObj.GetComponent<Carrier>());
		}
	}
	[ClientRpc]
	public void RpcGetOnCarrier(GameObject carrierObj)
	{
		GetOnCarrier(carrierObj.GetComponent<Carrier>());
	}
	public void GetOnCarrier(Carrier carrier)
	{
		curCarrier = carrier;
		carrier.driver = gameObject;
		transform.rotation = carrier.transform.rotation;	// 先转，再改层级关系。否则会发生变形。
		transform.parent = carrier.transform;
		transform.localPosition = carrier.seat;
	}

	[Command]
	public void CmdGetOffCarrier()
	{
		RpcGetOffCarrier();
		if(isServer && !isClient)
		{
			GetOffCarrier();
		}
	}
	[ClientRpc]
	public void RpcGetOffCarrier()
	{
		GetOffCarrier();
	}
	public void GetOffCarrier()
	{
		curCarrier.driver = null;
		transform.parent =null;
		transform.position += curCarrier.getOffPos;
		curCarrier = null;
		/*
		if(isServer)
		{
			transform.localScale = Vector3.one;
		}
		*/
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
	[Command]
	public void CmdCarrierFire(uint fireBits)
	{
		curCarrier.Fire(fireBits);
	}

}
