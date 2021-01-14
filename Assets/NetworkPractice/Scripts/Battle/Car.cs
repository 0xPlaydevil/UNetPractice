using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Car : Carrier {
	float turnRate = 30;

	float turnSpeed = 0;

	public class InputInfo : NetworkMessage
	{
		public float inRun = 0;
		public float inTurn = 0;
	}

	// Todo: 需要一种方法把变量发到服务器。可以考虑用消息
	private const short msgInput = 1001;
	[SyncVar]private InputInfo inInfo = new InputInfo();
	public override short msgType
	{
		get{return msgInput;}
	}
	public override NetworkMessage msgContent
	{
		get{return inInfo;}
	}

	// Use this for initialization
	void Start () {
		speed = 6;
	}

	void FixedUpdate()
	{

	}
	
	public override void GetInput()
	{
		// 驾驶部分
		inInfo.inRun = Input.GetAxis("Vertical");
		inInfo.inTurn = Input.GetAxis("Horizontal");
		// 火控部分
		_weaponFireBits = 0;
		// Todo:武器系统类型化，重用
		_weaponFireBits |= Input.GetButtonDown("Fire1")? 1u: 0u;
		_weaponFireBits <<= 1;
		_weaponFireBits |= Time.time%0.2f<0.02f&&Input.GetButton("Fire2")? 1u: 0u;
		_weaponFireBits <<= 1;
		_weaponFireBits |= Input.GetButtonUp("Fire3")? 1u: 0u;
	}

	public override void Run()
    {
    	float curSpeed = inInfo.inRun*speed;
    	turnSpeed = curSpeed*turnRate;
        transform.Translate(Vector3.forward*Time.deltaTime*curSpeed, Space.Self);
        transform.Rotate(Vector3.up,inInfo.inTurn*Time.deltaTime*turnSpeed);
    }

    public override void Attenuate()
    {
    	inInfo.inRun = Mathf.Lerp(inInfo.inRun,0,0.1f);
    	inInfo.inTurn = Mathf.Lerp(inInfo.inTurn,0,0.3f);
    }

    public override void Fire(uint fireBits)
    {
    	if((fireBits &1u) ==1u)
    	{
    		// fire3
    		NetworkServer.Spawn(PillManager.instance.Create(2,driver));
    	}
    	fireBits >>= 1;
    	if((fireBits &1u) ==1u)
    	{
    		// fire2
    		NetworkServer.Spawn(PillManager.instance.Create(1,driver));
    	}
    	fireBits >>=1;
    	if((fireBits &1u) == 1u)
    	{
    		// fire1
    		NetworkServer.Spawn(PillManager.instance.Create(0,driver));
    	}
    }

    void OnMsgInputInfo(InputInfo msg)
    {
    	inInfo = msg;
    }

/*
    void OnStartClient()
    {
    	if(!isServer)
    	{
	    	NetworkClient.RegisterHandler(msgInput,OnMsgInputInfoClient);
    	}
    }
*/
    public override void OnStartServer()
    {
    	base.OnStartServer();
    	NetworkServer.RegisterHandler<InputInfo>(OnMsgInputInfo);
    }
}
