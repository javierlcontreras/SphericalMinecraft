using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FirstPersonController))]
public class FirstPersonControllerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //FirstPersonController myTarget = (FirstPersonController)target;
        //EditorGUILayout.LabelField("Velocity", ""+myTarget.GetVerticalVelocity());
    }
}
