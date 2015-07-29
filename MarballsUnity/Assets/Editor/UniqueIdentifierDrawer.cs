﻿// These scripts obtained from:
// - http://answers.unity3d.com/questions/487121/automatically-assigning-gameobjects-a-unique-and-c.html
// - http://joshuasmyth.maglevstudios.com/post/Generating-Persistent-Unique-Ids-in-Unity3D2

using UnityEditor;
using UnityEngine;
using System;

// Place this file inside Assets/Editor
[CustomPropertyDrawer (typeof(UniqueIdentifierAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer {
	public override void OnGUI (Rect position, SerializedProperty prop, GUIContent label) {
		// Generate a unique ID, defaults to an empty string if nothing has been serialized yet
		if (prop.stringValue == "") {
			Guid guid = Guid.NewGuid();
			prop.stringValue = guid.ToString();
		}
		
		// Place a label so it can't be edited by accident
		Rect textFieldPosition = position;
		textFieldPosition.height = 16;
		DrawLabelField (textFieldPosition, prop, label);
	}
	
	void DrawLabelField (Rect position, SerializedProperty prop, GUIContent label) {
		EditorGUI.LabelField(position, label, new GUIContent (prop.stringValue));
	} 
}
