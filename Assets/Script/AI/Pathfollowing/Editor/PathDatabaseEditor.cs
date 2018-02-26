﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


    [CustomEditor (typeof (PathDatabase))]
    public class PathDatabaseEditor : Editor
    {

        private int pathIndex = -1;

        private PathDatabase database;

        public override void OnInspectorGUI() {
            database = (PathDatabase)target;

            if (database.paths == null)
                database.paths = new List<AIPath> ();

            for (int i = 0; i < database.paths.Count; i++) {

                GUI.backgroundColor = new Color (0.6f, 0.6f, 0.5f, 1f);


                if (pathIndex == i) {
                    GUI.backgroundColor = new Color (0.75f, 0.75f, 0.75f, 1f);

                    EditorGUILayout.BeginHorizontal ();
                    if (GUILayout.Button (database.paths[i].name, EditorStyles.toolbarButton)) {
                        pathIndex = -1;
                    } else if (GUILayout.Button ("x", EditorStyles.toolbarButton, GUILayout.Width (24))) {
                        database.paths.RemoveAt (i);
                        pathIndex = -1;
                    }
                    EditorGUILayout.EndHorizontal ();

                    if (pathIndex != -1) {
                        database.paths[i].name = EditorGUILayout.TextField (database.paths[i].name, EditorStyles.toolbarTextField);

                        database.paths[i].color = EditorGUILayout.ColorField ("Colour", database.paths[i].color);

                        int newPointIndex = -1;

                        if (database.paths[i].points == null)
                            database.paths[i].points = new List<Vector3> ();
                        for (int j = 0; j < database.paths[i].points.Count; j++) {
                            EditorGUILayout.BeginHorizontal ();
                            EditorGUILayout.LabelField (j.ToString (), GUILayout.Width (16));
                            database.paths[i].points[j] = EditorGUILayout.Vector3Field ("", database.paths[i].points[j]);
                            if (GUILayout.Button ("+", EditorStyles.toolbarButton, GUILayout.Width (20))) {
                                newPointIndex = j;
                            }
                            if (GUILayout.Button ("x", EditorStyles.toolbarButton, GUILayout.Width (20))) {
                                database.paths[i].points.RemoveAt (j);
                            }
                            EditorGUILayout.EndHorizontal ();
                        }
                        if (newPointIndex >= 0) {
                            database.paths[i].points.Insert (newPointIndex, database.paths[i].points[newPointIndex]);
                        } else if (GUILayout.Button ("+", EditorStyles.toolbarButton)) {
                            if (database.paths[i].points.Count == 0)
                                database.paths[i].points.Add (new Vector3 ());
                            else
                                database.paths[i].points.Add (database.paths[i].points[database.paths[i].points.Count - 1]);
                        }
                    }

                    GUILayout.Space (24);
                    GUI.backgroundColor = new Color (0.6f, 0.6f, 0.5f, 1f);
                } else if (GUILayout.Button (database.paths[i].name, EditorStyles.toolbarButton)) {
                    pathIndex = i;
                }
            }
            if (GUILayout.Button ("+", EditorStyles.toolbarButton)) {
                database.paths.Add (new AIPath());
            }
        }

        void OnEnable() {
            SceneView.onSceneGUIDelegate += OnSceneGUI;
        }

        void OnDisable() {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }


        void OnSceneGUI(SceneView sceneView) {

            database = (PathDatabase)target;
            if ((pathIndex < database.paths.Count) && (pathIndex >= 0)) {
                if (database.paths[pathIndex] != null) {
                AIPath path = database.paths[pathIndex];
                    Handles.color = path.color;
                    

                    for (int i = 0; i < path.points.Count; i++) {
                        Handles.DrawWireCube (path.points[i], new Vector3 (1f, 1f, 1f));
                        path.points[i] = Handles.PositionHandle (path.points[i], new Quaternion (0f, 0f, 0f, 1f));
                        if (i < path.points.Count - 1)
                            Handles.DrawLine (path.points[i], path.points[i + 1]);
                        else if (i == path.points.Count - 1)
                            Handles.DrawLine (path.points[i], path.points[0]);
                        Handles.Label (path.points[i], i.ToString ());
                    }

                    database.paths[pathIndex] = path;
                }
            }

        }

    }


