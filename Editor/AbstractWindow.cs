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
    public abstract class AbstractWindow
    {
        public static void DrawUILine(Color color, int width = 6, int thickness = 1, int padding = 1)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 1;
            r.width += width;
            EditorGUI.DrawRect(r, color);
        }

        static GUIStyle centeredStyle;

        public static void UILabel(string text)
        {
            centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperLeft;
            centeredStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(text);
        }
    }
}