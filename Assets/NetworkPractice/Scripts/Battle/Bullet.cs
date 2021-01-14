using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour {
	[SyncVar]public float speed = 10;
	public GameObject owner = null;
	private float startTime = 0;

	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*speed*Time.deltaTime);
		if(isServer)
		{
			if(Time.time-startTime > 3)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	void OnEnable()
	{
		startTime = Time.time;
	}

	void OnTriggerEnter(Collider other)
	{
		// 注意：按理说要排除载具，但是这里出现了问题，client的pill与载具发生碰撞并销毁了。但是Server看似并没有与自身载具发生碰撞。待出现再解决。
		// 是因为owner没同步，与owner碰撞销毁的
		if(isServer)
		{
			Carrier carrier = other.GetComponent<Carrier>();
			if(other.gameObject !=owner && (carrier==null || carrier!=null && carrier.driver!=owner))
			{
				Object.Destroy(gameObject);
			}
		}
	}
}
