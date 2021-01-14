using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetMove : NetworkBehaviour {
	private float moveSpeed = 3;
	private float rotSpeed = 90;
	private Transform targetTransform = null;

	void Start()
	{
		targetTransform = GameObject.Find("TestCube").transform;
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		if(isLocalPlayer)
		{
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			float r = Input.GetAxis("Mouse X");
			bool run = Input.GetKey(KeyCode.LeftShift);
			CmdMove(h,v,run);
			CmdRotate(r);
		}
	}

	[Command]
	public void CmdMove(float h, float v, bool isRun)
	{
		if(isServer && !isClient)
		{
			Move(h,v,isRun);
		}
		RpcMove(h,v,isRun);
	}

	[ClientRpc]
	public void RpcMove(float h, float v, bool isRun)
	{
		Move(h,v,isRun);
	}

	public void Move(float h,float v,bool isRun)
	{
		Vector3 delta = new Vector3(h,0,v)*Time.deltaTime*moveSpeed*(isRun? 2: 1);
		if(delta.sqrMagnitude>1)
		{
			delta = delta.normalized;
		}
		targetTransform.Translate(delta,Space.World);
	}

	[Command]
	public void CmdRotate(float r)
	{
		if(isServer && !isClient)
		{
			Rotate(r);
		}
		RpcRotate(r);
	}

	[ClientRpc]
	public void RpcRotate(float r)
	{
		Rotate(r);
	}

	public void Rotate(float r)
	{
		Vector3 delta = new Vector3(0,r,0)*Time.deltaTime*rotSpeed;
		targetTransform.eulerAngles += delta;
	}
}
