using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tetris))]
public class TetrisInspector : Editor
{
    Tetris targetScript;

    void OnEnable()
    {
        targetScript = target as Tetris;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("x -> Rows, y -> Cols", MessageType.Warning);
        EditorGUILayout.BeginVertical();
        for (int y = 0; y < TetrisConstants.ROWS; y++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(y.ToString(), GUILayout.MaxWidth(20));
            for (int x = 0; x < TetrisConstants.COLS; x++)
            {
                if (y == 0)
                {
                    EditorGUILayout.BeginVertical();

                    EditorGUILayout.LabelField(x.ToString(), GUILayout.MaxWidth(20));
                }
                EditorGUILayout.Toggle(targetScript.playspace.board[y].cols[x] != null);
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
