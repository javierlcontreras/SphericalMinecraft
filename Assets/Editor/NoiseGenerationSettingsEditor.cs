using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseGeneratorSettings))]
public class NoiseGenerationSettingsEditor : Editor 
{
    Texture2D texture = null;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoiseGeneratorSettings myTarget = (NoiseGeneratorSettings)target;
        //myTarget.offset = EditorGUILayout.Vector3Field("Offset", myTarget.offset);
        //EditorGUILayout.LabelField("Level", myTarget.GetName());
        if(GUILayout.Button("Test"))
        {   
            texture = myTarget.DebugTexture();
        }
        GUILayout.Label(texture);
    }
}
