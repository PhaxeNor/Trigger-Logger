using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRCSDK2;

/**
 * Author: Christian "PhaxeNor" Wiig
 * Website: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCPrefabs.TriggerLogger
{
    public class ExtraWindow : AbstractWindow
    {
        public void drawWindow(TriggerLogger triggerLogger)
        {
            var guicolor = GUI.backgroundColor;

            UILabel("Help");

            GUI.backgroundColor = Color.gray;
            GUILayout.BeginHorizontal("helpbox");
            GUILayout.Label("Disabled");
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.yellow;
            GUILayout.BeginHorizontal("helpbox");
            GUILayout.Label("Warning");
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.red;
            GUILayout.BeginHorizontal("helpbox");
            GUILayout.Label("Error");
            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal("helpbox");
            GUILayout.Label("Prefab");
            GUILayout.EndHorizontal();


            GUI.backgroundColor = guicolor;

            GUILayout.Space(10);

            UILabel("Author: PhaxeNor");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Twitter"))
            {
                Application.OpenURL("https://twitter.com/phaxenor");
            }

            if (GUILayout.Button("Github"))
            {
                Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger");
            }

            GUILayout.EndHorizontal();



            GUILayout.Space(10);

            UILabel("VRC Prefabs");

            if (GUILayout.Button("Twitter"))
            {
                Application.OpenURL("https://twitter.com/VRCPrefabs");
            }

            if (GUILayout.Button("Github"))
            {
                Application.OpenURL("https://github.com/vrc-prefabs");
            }

            if (GUILayout.Button("Website"))
            {
                Application.OpenURL("https://vrcprefabs.com");
            }

            if (GUILayout.Button("Unofficial Wiki"))
            {
                Application.OpenURL("https://vrchat.wikidot.com");
            }

        }
    }
}