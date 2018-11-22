using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillManager : MonoBehaviour {
	public GameObject[] pills = null;

	private static PillManager _instance = null;

	public static PillManager instance
	{
		get
		{
			if(_instance==null)
			{
				_instance = FindObjectOfType<PillManager>();
			}
			return _instance;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	public GameObject Create(int pillType, GameObject owner,Vector3 pos=new Vector3(),Quaternion rot=new Quaternion())
	{
		GameObject pill = null;
		if(pillType>=0 && pillType<pills.Length)
		{
			if(rot.w==0)
			{
				rot = Quaternion.identity;
			}
			pill = Instantiate(pills[pillType],owner.transform.TransformPoint(pos),owner.transform.rotation*rot);
			pill.GetComponent<Bullet>().owner = owner;
			pill.SetActive(true);
		}
		return pill;
	}

	void OnEnable()
	{
		if(_instance == null)
		{
			_instance = this;
		}
		else if(_instance!=this)
		{
			Debug.LogError("Try create another instance to a singleton class!");
		}
	}

	void OnDestroy()
	{
		if(_instance == this)
		{
			_instance=null;
		}
	}
}
