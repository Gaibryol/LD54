using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PieceTemplate))]
public class PieceTemplateInspector : Editor
{
    PieceTemplate targetScript;

    void OnEnable()
    {
        targetScript = target as PieceTemplate;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("x -> Rows, y -> Cols", MessageType.Warning);
        EditorGUILayout.BeginVertical();
        for (int y = 0; y < 5; y++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(y.ToString(), GUILayout.MaxWidth(20));
            for (int x = 0; x < 5; x++)
            {
                if (y == 0)
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField(x.ToString(), GUILayout.MaxWidth(20));
                }
                targetScript.pieceDefinition[y].cols[x] = EditorGUILayout.Toggle(targetScript.pieceDefinition[y].cols[x]);
                if (y == 0)
                {
                    EditorGUILayout.EndVertical();
                }
            }
            EditorGUILayout.EndHorizontal();

        }
        EditorGUILayout.EndVertical();

        base.OnInspectorGUI();
    }
}
