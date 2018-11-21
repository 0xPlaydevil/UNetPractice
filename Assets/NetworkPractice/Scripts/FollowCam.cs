using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {
	public Transform target = null;
	public Vector3 offset = new Vector3(1.58f,2.7f,-2.58f);
	public Vector3 euler = new Vector3(27,-6,0);

	private Vector3 targetPos;
	private Quaternion targetRot;
	private Matrix4x4 pan;
	
	// Update is called once per frame
	void LateUpdate () {
		if(target)
		{
			pan = Matrix4x4.TRS(target.position,Quaternion.Euler(Vector3.up*target.eulerAngles.y),Vector3.one);
			targetPos = pan.MultiplyPoint(offset);
			transform.position = Vector3.Lerp(transform.position,targetPos, 0.4f);

			// targetRot = Quaternion.LookRotation(target.position-transform.position,Vector3.up);
			targetRot = pan.rotation*Quaternion.Euler(euler);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.4f);
		}
	}
}
