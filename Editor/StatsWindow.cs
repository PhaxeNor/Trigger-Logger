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
    public class StatsWindow : AbstractWindow
    {
        private enum StatsType
        {
            Broadcasts,
            Triggers,
            Actions
        }

        StatsType dropdown = StatsType.Broadcasts;

        Vector2 scrollLocation;

        public void drawWindow(TriggerLogger triggerLogger)
        {
            GUILayout.Space(5);

            dropdown = (StatsType)EditorGUILayout.EnumPopup(dropdown);

            triggerLogger.StatsShowZero = GUILayout.Toggle(triggerLogger.StatsShowZero, "Show with zero");

            scrollLocation = EditorGUILayout.BeginScrollView(scrollLocation);

            switch (dropdown)
            {
                case StatsType.Broadcasts: ShowBroadcasts(triggerLogger); break;
                case StatsType.Triggers: ShowTriggers(triggerLogger); break;
                case StatsType.Actions: ShowActions(triggerLogger); break;
            }

            EditorGUILayout.EndScrollView();
        }

        private void ShowBroadcasts(TriggerLogger triggerLogger)
        {
            var win = Screen.width * 0.8;
            var w1 = win * 0.17; var w2 = win * 0.06;

            var centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Label("Broadcasts: " + triggerLogger.broadcasts.list.Where(t => t.Type != VRC_EventHandler.VrcBroadcastType.Local).Sum(t => t.Total).ToString() + " - Non-local");
            GUILayout.Label("Network: " + triggerLogger.broadcasts.list.Sum(t => t.RPC).ToString());

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width((float)w1));
            GUILayout.Label("Total", centeredStyle, GUILayout.Width((float)w2));
            GUILayout.Label("RPC", centeredStyle, GUILayout.Width((float)w2));
            EditorGUILayout.EndHorizontal();

            triggerLogger.broadcasts.list.ForEach(bl =>
            {
                if (!triggerLogger.StatsShowZero && bl.Total == 0) return;

                DrawUILine(Color.gray);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(bl.Type.ToString(), GUILayout.Width((float)w1));
                EditorGUILayout.LabelField(bl.Total.ToString(), centeredStyle, GUILayout.Width((float)w2));
                EditorGUILayout.LabelField(bl.RPC.ToString(), centeredStyle, GUILayout.Width((float)w2));
                EditorGUILayout.EndHorizontal();
            });
            

        }

        private void ShowTriggers(TriggerLogger triggerLogger)
        {
            var win = Screen.width * 0.8;
            var w1 = win * 0.20; var w2 = win * 0.06;

            var centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Label("Triggers: " + triggerLogger.triggers.list.Sum(t => t.Total).ToString());
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width((float)w1));
            GUILayout.Label("Total", centeredStyle, GUILayout.Width((float)w2));
            EditorGUILayout.EndHorizontal();

            triggerLogger.triggers.list.ForEach(bl =>
            {
                if (!triggerLogger.StatsShowZero && bl.Total == 0) return;

                DrawUILine(Color.gray);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(bl.Type.ToString(), GUILayout.Width((float)w1));
                EditorGUILayout.LabelField(bl.Total.ToString(), centeredStyle, GUILayout.Width((float)w2));
                EditorGUILayout.EndHorizontal();
            });
        }

        private void ShowActions(TriggerLogger triggerLogger)
        {
            var win = Screen.width * 0.8;
            var w1 = win * 0.20; var w2 = win * 0.06;

            var centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.Label("Actions: " + triggerLogger.actions.list.Sum(t => t.Total).ToString());
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Type", GUILayout.Width((float)w1));
            GUILayout.Label("Total", centeredStyle, GUILayout.Width((float)w2));
            EditorGUILayout.EndHorizontal();

            triggerLogger.actions.list.ForEach(bl =>
            {
                if (!triggerLogger.StatsShowZero && bl.Total == 0) return;

                DrawUILine(Color.gray);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(bl.Type.ToString(), GUILayout.Width((float)w1));
                EditorGUILayout.LabelField(bl.Total.ToString(), centeredStyle, GUILayout.Width((float)w2));
                EditorGUILayout.EndHorizontal();
            });
        }
    }
}