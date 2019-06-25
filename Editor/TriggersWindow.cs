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
    public class TriggersWindow : AbstractWindow
    {
        Vector2 scrollLocation;

        public void drawWindow(TriggerLogger tl)
        {
            GUILayout.Space(5);

            var win = Screen.width * 0.8;
            var w1 = win * 0.35; var w2 = win * 0.10; var w3 = win * 0.10; var w4 = win * 0.10;

            var centeredStyle = new GUIStyle(GUI.skin.label);
            centeredStyle.alignment = TextAnchor.UpperCenter;

            GUILayout.BeginHorizontal("helpbox");

            GUILayout.Label("GameObject", GUILayout.Width((float)w1));

            GUILayout.Label("Triggers", centeredStyle, GUILayout.Width((float)w2));

            GUILayout.Label("Actions", centeredStyle, GUILayout.Width((float)w2));

            GUILayout.Label("Broadcasts", centeredStyle, GUILayout.Width((float)w2));

            GUILayout.Label("Empty", centeredStyle, GUILayout.Width((float)w2));


            GUILayout.EndHorizontal();

            scrollLocation = EditorGUILayout.BeginScrollView(scrollLocation);

            if (tl.triggerList.Count > 0)
            {

                for (var i = 0; i < tl.triggerList.Count; i++)
                {

                    var trigger = tl.triggerList[i];

                    List<string> errors = new List<string>();

                    if (trigger == null) continue;

                    if (!tl.ShowPrefabs && trigger.gameObject.scene.name == null) continue;

                    if (tl.query != "" && !trigger.gameObject.name.ToLower().Contains(tl.query.ToLower())) continue;

                    var empty = trigger.Triggers.Sum(t => t.Events.Count(e => e.ParameterObjects.Count() > 0));

                    if (tl.SendRPCquery != "" && !trigger.Triggers.Exists(t => { return t.Events.Exists(e => e.ParameterString.ToLower().Contains(tl.SendRPCquery.ToLower())); })) continue;

                    if (!tl.ShowEmpty && empty > 0) continue;

                    if (tl.triggerFlags == 0 || tl.broadcastFlags == 0 || tl.actionsFlags == 0) continue;

                    if (tl.triggerFlags != 0 || tl.broadcastFlags != 0 || tl.actionsFlags != 0)
                    {
                        if (tl.triggerFlags != 0 && tl.triggerFlags != -1 && !trigger.Triggers.Exists(t => { return tl.triggerFilter.Exists(o => t.TriggerType.ToString() == o); })) continue;
                        if (tl.broadcastFlags != 0 && tl.broadcastFlags != -1 && !trigger.Triggers.Exists(t => { return tl.broadcastFilter.Exists(o => t.BroadcastType.ToString() == o); })) continue;
                        if (tl.actionsFlags != 0 && tl.actionsFlags != -1 && !trigger.Triggers.Exists(t => { return t.Events.Exists(e => tl.actionFilter.Exists(o => e.EventType.ToString() == o)); })) continue;

                        if (tl.RpcMethodsFlags != 0 && tl.RpcMethodsFlags != -1 && !trigger.Triggers.Exists(t => { return t.Events.Exists(e => tl.RpcMethodsFilter.Exists(o => e.ParameterString.ToString() == o)); })) continue;
                    }

                    var triggers = trigger.Triggers.Count();
                    var events = trigger.Triggers.Sum(t => t.Events.Count);
                    var broadcasts = trigger.Triggers.Where(t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local).Count();

                    var guiColor = GUI.backgroundColor;

                    if (i % 2 == 1) GUI.backgroundColor = new Color(0.80f, 0.80f, 0.80f, 1f);

                    if (triggers == 0)
                    {
                        errors.Add("Object has the VRC_Trigger componet, but no triggers");
                        GUI.backgroundColor = Color.red;
                    }
                    if (!trigger.gameObject.activeInHierarchy || !trigger.isActiveAndEnabled)
                    {
                        if (trigger.gameObject.scene.name != null) errors.Add("Trigger is disabled");
                        GUI.backgroundColor = Color.yellow;
                    }

                    if (missingTeleportTo(trigger))
                    {
                        errors.Add("TeleportTo target is empty");
                        GUI.backgroundColor = Color.yellow;
                    }

                    if (empty == 0)
                    {
                        errors.Add("Trigger doesn't have any actions.");
                        GUI.backgroundColor = Color.red;
                    }
                    if (trigger.gameObject.scene.name == null) GUI.backgroundColor = Color.cyan;

                    if (Selection.objects.Contains(trigger.gameObject))
                    {
                        GUI.backgroundColor = new Color(0.047f, 0.564f, 0.929f, 1);
                    }

                    if (errors.Count > 0 && !tl.ShowErrors) continue;
                    if (errors.Count == 0 && tl.HideNonErrors) continue;

                    GUILayout.BeginVertical("helpbox");

                    GUILayout.BeginHorizontal();

                    EditorGUILayout.ObjectField(trigger.gameObject, typeof(GameObject), true, GUILayout.Width((float)w1));

                    GUILayout.Label(triggers.ToString(), centeredStyle, GUILayout.Width((float)w2));

                    GUILayout.Label(events.ToString(), centeredStyle, GUILayout.Width((float)w2));

                    GUILayout.Label(broadcasts.ToString(), centeredStyle, GUILayout.Width((float)w2));

                    GUILayout.Label(empty.ToString(), centeredStyle, GUILayout.Width((float)w2));

                    GUILayout.EndHorizontal();

                    if (errors.Count > 0)
                    {
                        GUILayout.BeginVertical();

                        errors.ForEach(e =>
                        {
                            GUILayout.Label(e);
                        });

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();

                    if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        GameObject[] selected = new GameObject[1];
                        selected[0] = trigger.gameObject;

                        Selection.objects = selected;

                        tl.Repaint();
                    }

                    GUI.backgroundColor = guiColor;
                }
            }

            EditorGUILayout.EndScrollView();
        }

        bool missingTeleportTo(VRC_Trigger trigger)
        {
            var emptyTeleport = false;

            trigger.Triggers
                .ForEach(t => {
                    t.Events.ForEach(e =>
                    {
                        if(e.EventType == VRC_EventHandler.VrcEventType.SendRPC && e.ParameterString == "TeleportTo")
                        {
                            object[] param = VRC_Serialization.ParameterDecoder(e.ParameterBytes);

                            if(param.Length < 1 || param[0] == null)
                            {
                                emptyTeleport = true;
                            }
                        }
                    });
                });

            return emptyTeleport;
        }

    }
}