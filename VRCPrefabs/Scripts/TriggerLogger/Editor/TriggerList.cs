using System;
using System.Collections.Generic;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
	public class TriggerItem
	{
		public VRC_Trigger.TriggerType type { get; set; }
		public int total { get; set; }

		public string getTypeName()
		{
			return type.ToString ();
		}
	}

	public class TriggerList
	{
		public List<TriggerItem> list { get; set; }

		public List<VRC_Trigger> emptyTrigger { get; set; }

		public void initList()
		{
			list = new List<TriggerItem> ();
			emptyTrigger = new List<VRC_Trigger> ();

			foreach (VRC_Trigger.TriggerType e in Enum.GetValues(typeof(VRC_Trigger.TriggerType))) {
				list.Add (new TriggerItem () { type = e, total = 0 });
			}
		}
	}
}