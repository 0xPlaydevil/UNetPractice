using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Carrier : NetworkBehaviour {
	//public PlayerControl driver = null;

	//protected Vector3 _seat = Vector3.zero;
	protected PlayerControl[] seats = null;
	protected Vector3[] seatPoses = null;
	//public Vector3 seat
	//{get {return _seat; } }
	public int seatCount
	{
		get { return seats.Length; }
	}
	public PlayerControl driver
	{
		get { return seats[0]; }
	}

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
	public abstract NetworkMessage msgContent
	{
		get;
	}

	public abstract void GetInput();
	public abstract void PushInput();

	protected virtual void InitSeats(int cnt)
	{
		seats = new PlayerControl[cnt];
		seatPoses = new Vector3[cnt];
	}

	public virtual PlayerControl GetPlayer(int idx)
	{
		return seats[idx];
	}

	public virtual int SeatOfPlayer(PlayerControl player)
	{
		for(int i=0;i<seats.Length;++i)
		{
			if(player== seats[i])
			{
				return i;
			}
		}
		return -1;
	}

	public virtual int GetValidSeat()
	{
		int i;
		for(i=0;i<seats.Length;++i)
		{
			if(seats[i]==null)
			{
				return i;
			}
		}
		return -1;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="idx"></param>
	/// <param name="player"></param>
	/// <param name="adapt"></param>
	/// <returns>成功返回座位号，失败返回-1</returns>
	protected virtual int SetPlayer(int idx,PlayerControl player,bool adapt=true)
	{
		if(seats[idx]==null || adapt && (idx = GetValidSeat()) >= 0)
		{
			seats[idx] = player;
			return idx;
		}
		return -1;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="player"></param>
	/// <param name="seat"></param>
	/// <returns>seatId. -1:full, 0:driver, n:passenger</returns>
	public virtual int AddPassenger(PlayerControl player, int seat=0,bool adapt=false)
	{
		seat= SetPlayer(seat, player,adapt);
		if(seat>=0)
		{
			player.transform.rotation = transform.rotation;    // 先转，再改层级关系。否则会发生变形。
			player.transform.parent = transform;
			player.transform.localPosition = seatPoses[seat];
		}
		return seat;
	}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="player"></param>
	/// <returns>seatId</returns>
	public virtual int RemovePassenger(PlayerControl player)
	{
		int seat = SeatOfPlayer(player);
		if(seat>=0)
		{
			seats[seat] = null;
			transform.parent = null;
			transform.position += getOffPos;
		}
		return seat;
		/*
		if(isServer)
		{
			transform.localScale = Vector3.one;
		}
		*/
	}

	protected virtual void Awake()
	{
		InitSeats(1);
	}

	public virtual void Update()
	{
		if(driver!=null && driver.isLocalPlayer)
		{
			GetInput();
			PushInput();
			CmdFire(weaponFireBits);
		}
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

	[Command]
	protected void CmdFire(uint fireBits)
	{
		ServerFire(fireBits);
	}
	public abstract void ServerFire(uint fireBits);
}
