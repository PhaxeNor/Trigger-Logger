using System;
using System.Collections.Generic;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
	public class ActionItem
	{
		public VRC_EventHandler.VrcEventType type { get; set; }
		public int total { get; set; }

		public string getTypeName()
		{
			return type.ToString ();
		}
	}

	public class ActionList
	{
		public List<ActionItem> list { get; set; }

		public void initList()
		{
			list = new List<ActionItem> ();

			foreach (VRC_EventHandler.VrcEventType e in Enum.GetValues(typeof(VRC_EventHandler.VrcEventType))) {
				list.Add (new ActionItem() { type = e, total = 0 });
			}
		}
	}
}