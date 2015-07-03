#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class UnityMarballsMenu : MonoBehaviour {

	#region Menu Options
	// CreateLevel - Automatically creates a new level ready to be set up properly.
	[MenuItem("Marballs/Create Level")]
	static void CreateLevel() {
		EditorApplication.SaveCurrentSceneIfUserWantsTo();	// Saves changes on current scene.
		EditorApplication.NewScene();						// Creates a new scene.
		
		// Instantiates everything a typical level will have.
		PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/UI Prefabs/LevelGUI"));
		PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/StartPlatform"));
		(PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/Marble Prefabs/Base Marble")) as GameObject).transform.position = new Vector3(0, 3, 0);
		(PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/KillZone")) as GameObject).transform.position = new Vector3(0, -20, 0);
		(PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/PanCam")) as GameObject).transform.position = new Vector3(0, 50, 0);
		(PrefabUtility.InstantiatePrefab(Resources.Load("Prefabs/FinishLine")) as GameObject).transform.position = new Vector3(0, 4, -8);
		Camera.main.gameObject.AddComponent<CameraController2>();
		
		// Attempts to save the new scene, and if saved, creates level data for it.
		if (EditorApplication.SaveScene()) {
			AddLevelData();
		}
	}
	
	// AddLevelData - Adds a container for storing the current level's data.
	[MenuItem("Marballs/Add Level Data")]
	static void AddLevelData() {
		// Generate file name.
		string fileName = EditorApplication.currentScene.Replace(".unity", string.Empty);
		fileName = fileName.Substring(fileName.LastIndexOf("/") + 1);
		
		// Create level data object.
		LevelDataObject newData = ScriptableObject.CreateInstance<LevelDataObject>();
		AssetDatabase.CreateAsset(newData, "Assets/Resources/Data/Level Data" + fileName + "Data.asset");
		AssetDatabase.SaveAssets();
		
		// Select and highlight it in the editor.
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = newData;
	}
	
	#endregion
}
#endif
