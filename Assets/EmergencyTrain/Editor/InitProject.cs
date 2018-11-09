// version: unity3d 5.2.3
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class IInitConfig
{
	static IInitConfig()
	{
		if(!PlayerPrefs.HasKey("InitProjectFlag"))
		{
			PlayerPrefs.SetInt("InitProjectFlag",1);
			InitConfig.ShowConfig();
		}
	}

	[MenuItem("Edit/Clear/ErasePlayerPrefs")]
	static public void ClearPrefs()
	{
		PlayerPrefs.DeleteAll();
	}

	[MenuItem("Edit/Clear/EraseInitFlag")]
	static public void ClearInit()
	{
		PlayerPrefs.DeleteKey("InitProjectFlag");
	}
}

public class InitConfig : EditorWindow {
	string workPath;
	string[] folders = new string[]{"Editor","Scenes","Scripts","Prefabs","Models","Textures","Materials","UI"};
	bool[] isSelected;

	[MenuItem("Window/InitProject... %&#i")]
	static public void ShowConfig()
	{
		InitConfig win = (InitConfig)EditorWindow.GetWindow(typeof(InitConfig));
		win.Show();
	}

	void OnEnable()
	{
		// 初始化类对象
		workPath = "./Assets/" + PlayerSettings.productName;
		Init(6);
	}

	void Init(int enableCount = 3)
	{
		if(enableCount>folders.Length)	enableCount = folders.Length;
		isSelected = new bool[folders.Length];
		for(int i=0;i<enableCount;++i)
		{
			isSelected[i] = true;
		}
	}

	void OnGUI()
	{
		EditorGUILayout.BeginVertical(GUILayout.MaxWidth(200));
		GUILayout.Label("Select and Create folders:");
		for(int i=0;i<folders.Length;++i)
		{
			isSelected[i] = EditorGUILayout.ToggleLeft(folders[i],isSelected[i]);
		}
		if(GUILayout.Button("CreateFolders"))
		{
			List<string> selectedFolders = new List<string>();
			for(int i=0;i<folders.Length;++i)
			{
				if(isSelected[i])
				{
					selectedFolders.Add(folders[i]);
				}
			}
			CreateFolders(selectedFolders.ToArray());
			CheckAndMoveSelf();
			Close();
		}
		EditorGUILayout.EndVertical();
	}

	void CreateFolders(string[] folderNames)
	{
		Directory.CreateDirectory(workPath);

		for(int i=0;i<folderNames.Length;++i)
		{
			string path = workPath+"\\"+folderNames[i];
			Directory.CreateDirectory(path);
			path += "\\readme.txt";
			if(!File.Exists(path))
				File.CreateText(path);
		}
		AssetDatabase.Refresh();
	}

	void CheckAndMoveSelf()
	{
		// if(Directory.Exists(workPath+"/Editor"))
		// {
		// 	FileUtil.MoveFileOrDirectory("./Assets/Editor/*",workPath+"/Editor/*");
		// 	// FileUtil.DeleteFileOrDirectory("./Assets/Editor");
		// 	AssetDatabase.Refresh();
		// }
	}
}
