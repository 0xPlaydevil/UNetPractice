using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MotorCycle : Carrier {
	float turnRate = 30;
	float maxRoll = 45;

	float turnSpeed = 0;
	float curRoll = 0;

	public class InputInfo : MessageBase
	{
		public float inRun = 0;
		public float inTurn = 0;
	}

	// Todo: 需要一种方法把变量发到服务器。可以考虑用消息
	private const short msgInput = 1002;
	[SyncVar]private InputInfo inInfo = new InputInfo();
	public override short msgType
	{
		get{return msgInput;}
	}
	public override MessageBase msgContent
	{
		get{return inInfo;}
	}

	// Use this for initialization
	void Start () {
		speed = 10;
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
		_weaponFireBits |= Time.time%0.1f<0.02f&&Input.GetButton("Fire2")? 1u: 0u;
	}

	public override void Run()
    {
    	float curSpeed = inInfo.inRun*speed;
    	turnSpeed = curSpeed*turnRate;
        transform.Translate(Vector3.forward*Time.deltaTime*curSpeed, Space.Self);
        transform.Rotate(Vector3.up,inInfo.inTurn*Time.deltaTime*turnSpeed,Space.World);
        curRoll = Mathf.MoveTowardsAngle(curRoll,-inInfo.inTurn*maxRoll,4);
        Vector3 euler = transform.eulerAngles;
        euler.z = curRoll;
        transform.eulerAngles = euler;
    }

    public override void Attenuate()
    {
    	inInfo.inRun = Mathf.Lerp(inInfo.inRun,0,0.05f);
    	inInfo.inTurn = Mathf.Lerp(inInfo.inTurn,0,0.3f);
    }

    public override void Fire(uint fireBits)
    {
    	if((fireBits &1u) ==1u)
    	{
    		// fire2
    		NetworkServer.Spawn(PillManager.instance.Create(3,driver,new Vector3(-0.3f,0.3f,0)));
    		NetworkServer.Spawn(PillManager.instance.Create(3,driver,new Vector3(0.3f,0.3f,0)));
    	}
    }

    void OnMsgInputInfo(NetworkMessage netMsg)
    {
    	InputInfo msg = netMsg.ReadMessage<InputInfo>();
    	inInfo = msg;
    }

/*
    void OnStartClient()
    {
    	if(!isServer)
    	{
	    	NetworkManager.singleton.client.RegisterHandler(msgInput,OnMsgInputInfoClient);
    	}
    }
*/
    public override void OnStartServer()
    {
    	base.OnStartServer();
    	NetworkServer.RegisterHandler(msgInput,OnMsgInputInfo);
    }
}
