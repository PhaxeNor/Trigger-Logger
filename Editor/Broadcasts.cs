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
    public class BroadcastItem
    {
        public VRC_EventHandler.VrcBroadcastType Type { get; set; }
        public int Total { get; set; }
        public int RPC { get; set; }

        public string GetTypeName() { return Type.ToString();  }
    }

    public class BroadcastList
    {
        public List<BroadcastItem> list { get; private set; }

        public void generateList()
        {
            list = new List<BroadcastItem>();

            foreach(VRC_EventHandler.VrcBroadcastType broadcast in Enum.GetValues(typeof(VRC_EventHandler.VrcBroadcastType)))
            {
                list.Add(new BroadcastItem() { Type = broadcast, Total = 0, RPC = 0 });
            }
        }
    }
}