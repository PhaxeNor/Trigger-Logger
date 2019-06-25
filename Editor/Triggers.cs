using System;
using System.Collections.Generic;
using VRCSDK2;

/**
 * Author: Christian "PhaxeNor" Wiig
 * Website: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCPrefabs.TriggerLogger
{
    public class TriggerItem
    {
        public VRC_Trigger Trigger { get; set; }
    }

    public class Trigger
    {
        public VRC_Trigger.TriggerType Type { get; set; }
        public int Total { get; set; }
    }

    public class TriggerList
    {
        public List<Trigger> list { get; private set; }
        
        public void generateList()
        {
            list = new List<Trigger>();

            foreach(VRC_Trigger.TriggerType type in Enum.GetValues(typeof(VRC_Trigger.TriggerType)))
            {
                list.Add(new Trigger()
                {
                    Type = type,
                    Total = 0
                });
            }
        }
    }
}