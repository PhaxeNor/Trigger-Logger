using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
	class Export 
	{
		public void ExportToTxt(List<VRC_Trigger> triggerList)
		{
			if (triggerList.Count == 0) {
				return;
			}

			List<string> lines = new List<string>();

			triggerList.ForEach (trigger => {

				bool isPrefab = trigger.gameObject.scene.name == null ? true : false;

				lines.Add("[" + trigger.gameObject.name + " (" + trigger.gameObject.GetInstanceID() + ")] - Prefab: " + isPrefab);

				trigger.Triggers.ForEach(t => {
					if(t.TriggerType == VRC_Trigger.TriggerType.Custom) {
						lines.Add("\tTrigger: Custom (" + t.Name + ")");
					} else {
						lines.Add("\tTrigger: " + t.TriggerType.ToString());

						if(Enum.IsDefined(typeof(InteractType), t.TriggerType.ToString())) {
							lines.Add("\tInteract Text: \"" + trigger.interactText + "\"");
							lines.Add("\tProximity: " + trigger.proximity);
						} else if (Enum.IsDefined(typeof(ColliderType), t.TriggerType.ToString())) {
							lines.Add("\tTrigger Individuals: " + t.TriggerIndividuals);
						}
					}

					lines.Add("\tBroadcast: " + t.BroadcastType.ToString());
					lines.Add("\tDelay: " + t.AfterSeconds.ToString());

					t.Events.ForEach(e => {

                        lines.Add("\n");

                        lines.Add("\t[Action: " + e.EventType.ToString() + "]");


						switch(e.EventType) {
						case VRC_EventHandler.VrcEventType.SpawnObject:
							lines.Add("\t\tPrefab: " + e.ParameterString);

							foreach(GameObject obj in e.ParameterObjects)
							{
								lines.Add("\t\tLocation: [" + obj.name + " (" + obj.GetInstanceID() + ")]");
							}
							break;

                        default:
                            if(e.ParameterString.Length != 0)
                                lines.Add("\t\tMethod: " + e.ParameterString);

                            foreach (GameObject obj in e.ParameterObjects)
                            {
                                lines.Add("\t\t[" + obj.name + " (" + obj.GetInstanceID() + ")]");
                            }
                            break;
						}
					});

					lines.Add("\n");
				});
			});

			DateTime currentDateTime = DateTime.Now;

			var path = Application.dataPath + "/VRCPrefabs/Data/TriggerLogger/";

			File.WriteAllLines (path + "triggers.export.txt", lines.ToArray());

            bool option = EditorUtility.DisplayDialog("Export Complete",
                           "Your triggers have now been exported to a text file.",
                           "Open Folder",
                           "Close");

            switch (option)
            {
                // Open Folder.
                case true:
                    System.Diagnostics.Process.Start(@path);
                    break;

                // Close.
                case false:
                    break;

                default:
                    Debug.LogError("Unrecognized option.");
                    break;
            }
        }
    }
       

	enum ColliderType
	{
		OnEnterTrigger,
		OnExitTrigger,
		OnEnterCollider,
		OnExitCollider
	}

    enum InteractType
    {
        OnInteract,
        OnPickup,
        OnDrop
    }


}