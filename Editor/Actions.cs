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
    public class ActionItem
    {
        public VRC_EventHandler.VrcEventType Type { get; set; }
        public int Total { get; set; }

        public string GetTypeName() { return Type.ToString();  }
    }

    public class ActionList
    {
        public List<ActionItem> list { get; private set; }

        public void generateList()
        {
            list = new List<ActionItem>();

            foreach(VRC_EventHandler.VrcEventType action in Enum.GetValues(typeof(VRC_EventHandler.VrcEventType)))
            {
                list.Add(new ActionItem() { Type = action, Total = 0 });
            }
        }
    }
}