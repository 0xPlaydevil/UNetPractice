using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Config : MonoBehaviour {
	public string jsonPath = null;
	private string jsonStr = null;
	private JSONObject jsonRoot = null;
	public bool Initialized
	{
		get
		{return jsonStr != null;}
	}
	private static Config _instance = null;
	public static Config instance
	{
		get
		{
			if(_instance==null)
			{
				_instance = FindObjectOfType<Config>();
				if(_instance!=null)
				{
					_instance.Init();
				}
			}
			return _instance;
		}
	}

	public string ReadJsonFile(string path)
	{
		if(File.Exists(path))
		{
			return File.ReadAllText(path);
		}
		else
		{
			return null;
		}
	}

	public JSONObject GetJsonObj(string uri)
	{
		string[] fields = uri.Split("/".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
		JSONObject jo = jsonRoot;
		foreach(string field in fields)
		{
			jo = jo.GetField(field);
			if(jo == null)
			{
				return null;
			}
		}
		return jo;
	}

	void Init()
	{
		jsonStr = ReadJsonFile(jsonPath);
		jsonRoot = new JSONObject(jsonStr);
	}

	void OnEnable()
	{
		if(_instance==null)
		{
			_instance = this;
			if(!Initialized)
			{
				Init();
			}
		}
		else if(_instance!=this)
		{
			Debug.LogWarning("Try to initialize another instance for singleton class Config!");
		}
	}

	void OnDestroy()
	{
		if(_instance==this)
		{
			_instance = null;
		}
	}
}
