using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class Carrier : NetworkBehaviour {
	public GameObject driver = null;

	protected Vector3 _seat = Vector3.zero;
	public Vector3 seat
	{get {return _seat; } }

	protected Vector3 _getOffPos = Vector3.left;
	public Vector3 getOffPos
	{get{return _getOffPos;}}

	protected uint _weaponFireBits = 0;
	public uint weaponFireBits
	{
		get{return _weaponFireBits;}
	}
	protected float speed = 5;

	public abstract short msgType
	{
		get;
	}
	public abstract MessageBase msgContent
	{
		get;
	}

	public abstract void GetInput();

	public virtual void Update()
	{
		Run();
		if(driver==null)
		{
			Attenuate();
		}
	}

/*
	[Command]
	public void CmdMove()
	{
		if(isServer && !isClient)
		{
			Run();	
		}
		RpcRun();
	}
	[ClientRpc]
	public void RpcRun()
	{
		Run();
	}
*/
	public abstract void Run();
	// 衰减（暂定driver离开后用。如果driver在的时候也用，应该是衰减速度快过同步速度会影响，否则不影响）
	public abstract void Attenuate();

	// [Command]
	// public abstract void CmdFire(uint fireBits);
	public abstract void Fire(uint fireBits);
}
