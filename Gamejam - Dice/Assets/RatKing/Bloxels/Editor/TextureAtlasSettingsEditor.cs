using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RatKing.Bloxels {
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TextureAtlasSettings))]
	public class TextureAtlasSettingsEditor : Editor {
		SerializedProperty id;
		SerializedProperty material;
		SerializedProperty properties;
		SerializedProperty atlasCompression;
		SerializedProperty atlasFilterMode;
		SerializedProperty asTexArray;
		SerializedProperty texArraySize;

		//

		void OnEnable() {
			id = serializedObject.FindProperty("id");
			material = serializedObject.FindProperty("material");
			properties = serializedObject.FindProperty("properties");
			atlasCompression = serializedObject.FindProperty("atlasCompression");
			atlasFilterMode = serializedObject.FindProperty("atlasFilterMode");
			asTexArray = serializedObject.FindProperty("asTexArray");
			texArraySize = serializedObject.FindProperty("texArraySize");
		}

		public override void OnInspectorGUI() {
			EditorGUILayout.PropertyField(id);
			EditorGUILayout.PropertyField(material);
			EditorGUILayout.PropertyField(properties);
			EditorGUILayout.PropertyField(atlasCompression);
			EditorGUILayout.PropertyField(atlasFilterMode);
			EditorGUILayout.PropertyField(asTexArray);
			if (asTexArray.boolValue) { EditorGUILayout.PropertyField(texArraySize); }

			if (serializedObject.hasModifiedProperties) {
				serializedObject.ApplyModifiedProperties();
			}
		}
	}

}