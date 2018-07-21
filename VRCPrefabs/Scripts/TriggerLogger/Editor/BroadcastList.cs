using System;
using System.Collections.Generic;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
	public class BroadcastItem
	{
		public VRC_EventHandler.VrcBroadcastType type { get; set; }
		public int total { get; set; }
		public int rpc { get; set; }

		public string getTypeName()
		{
			return type.ToString ();
		}
	}

	public class BroadcastList
	{
		public List<BroadcastItem> list { get; set; }

		public void initList()
		{
			list = new List<BroadcastItem> ();

			foreach (VRC_EventHandler.VrcBroadcastType broadcast in Enum.GetValues(typeof(VRC_EventHandler.VrcBroadcastType))) {
				list.Add (new BroadcastItem () { type = broadcast, total = 0, rpc = 0 });
			}
		}
	}
}