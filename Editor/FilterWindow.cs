using System;
using System.Reflection;
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
    public class FilterWindow : AbstractWindow
    {
        public void drawWindow(TriggerLogger tl)
        {
            GUILayout.Space(5);

            tl.query = EditorGUILayout.TextField("GameObject Search", tl.query);

            GUILayout.Space(5);

            UILabel("VRC Trigger");

            var oldTFalgs = tl.triggerFlags;

            tl.triggerFlags = EditorGUILayout.MaskField("Triggers", tl.triggerFlags, tl.triggerTypes.ToArray());

            if(tl.triggerFlags != oldTFalgs)
            {
                tl.triggerFilter.Clear();
                for (int i = 0; i < tl.triggerTypes.ToArray().Length; i++)
                {
                    if ((tl.triggerFlags & (1 << i)) == (1 << i)) tl.triggerFilter.Add(tl.triggerTypes.ToArray()[i]);
                }
            }

            var oldBFalgs = tl.broadcastFlags;

            tl.broadcastFlags = EditorGUILayout.MaskField("Broadcasts", tl.broadcastFlags, tl.broadcasTypes.ToArray());

            if (tl.broadcastFlags != oldBFalgs)
            {
                tl.broadcastFilter.Clear();
                for (int i = 0; i < tl.broadcasTypes.ToArray().Length; i++)
                {
                    if ((tl.broadcastFlags & (1 << i)) == (1 << i)) tl.broadcastFilter.Add(tl.broadcasTypes.ToArray()[i]);
                }
            }

            var oldAFalgs = tl.actionsFlags;

            tl.actionsFlags = EditorGUILayout.MaskField("Actions", tl.actionsFlags, tl.actionsTypes.ToArray());

            if (tl.actionsFlags != oldAFalgs)
            {
                tl.actionFilter.Clear();
                for (int i = 0; i < tl.actionsTypes.ToArray().Length; i++)
                {
                    if ((tl.actionsFlags & (1 << i)) == (1 << i)) tl.actionFilter.Add(tl.actionsTypes.ToArray()[i]);
                }
            }

            GUILayout.Space(5);

            var oldRPCFalgs = tl.RpcMethodsFlags;

            tl.RpcMethodsFlags = EditorGUILayout.MaskField("SendRPC Methods", tl.RpcMethodsFlags, tl.sendRpcMethods.ToArray());

            if (tl.RpcMethodsFlags != oldRPCFalgs)
            {
                tl.RpcMethodsFilter.Clear();
                for (int i = 0; i < tl.sendRpcMethods.ToArray().Length; i++)
                {
                    if ((tl.RpcMethodsFlags & (1 << i)) == (1 << i)) tl.RpcMethodsFilter.Add(tl.sendRpcMethods.ToArray()[i]);
                }
            }

            GUILayout.Space(5);

            UILabel("Advanced");

            tl.ShowPrefabs = EditorGUILayout.Toggle("Show Prefabs", tl.ShowPrefabs);

            tl.ShowEmpty = EditorGUILayout.Toggle("Show Empty", tl.ShowEmpty);

            tl.ShowErrors = EditorGUILayout.Toggle("Show Errors/Warnings", tl.ShowErrors);

            GUILayout.Space(5);

            tl.HideNonErrors = EditorGUILayout.Toggle("Hide Non Error/Warning", tl.HideNonErrors);

            GUILayout.Space(10);
        }
    }
}