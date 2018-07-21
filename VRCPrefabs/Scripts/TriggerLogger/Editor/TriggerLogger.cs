using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
	public class TriggerLogger : EditorWindow {

        bool showSettings = false;

        GUIStyle centeredStyle;
		GUIStyle rightStyle;

		List<VRC_Trigger> triggerList = new List<VRC_Trigger> ();

        List<GameObject> dynamicPrefabs = new List<GameObject> ();

		BroadcastList broadcastTypes = new BroadcastList();

		TriggerList triggerEvents = new TriggerList();

		int triggerFilter = 0;
		List<string> triggerTypes = new List<string>();
		List<string> broadcastEvents = new List<string> ();

		int broadFilter = 0;

		string filterGameObjective = "";

		bool canBeOptimized = false;

		bool showPrefabs;

        [MenuItem("VRC Prefabs/Trigger Logger")]
		public static void ShowMenu()
		{
			EditorWindow window = EditorWindow.GetWindow<TriggerLogger> ("Trigger Logger"); 
			window.minSize = new Vector2(1300, 500);
			window.Show ();
		}

		void OnEnable()
		{
			broadcastTypes.initList ();
			triggerEvents.initList ();

			triggerTypes = Enum.GetNames (typeof(VRC_Trigger.TriggerType)).Select (x => x.ToString ()).ToList ();
			triggerTypes.Insert (0, "Everything");

			broadcastEvents = Enum.GetNames (typeof(VRC_EventHandler.VrcBroadcastType)).Select (x => x.ToString ()).ToList ();
			broadcastEvents.Insert (0, "Everything");

			if(!EditorPrefs.HasKey("TriggerLogger.ShowPrefabs")) {
				EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", true);
			} else {
				showPrefabs = EditorPrefs.GetBool ("TriggerLogger.ShowPrefabs");
			}

            if(EditorPrefs.GetBool("TriggerLogger.Liability"))
            {
                FillList();
            }
        }

		void OnGUI()
		{
			centeredStyle = GUI.skin.GetStyle("Label");
			centeredStyle.alignment = TextAnchor.UpperCenter;
			centeredStyle.fontStyle = FontStyle.Bold;

            if(EditorPrefs.GetBool("TriggerLogger.Liability"))
            {
                FilterPanel();

                if (showSettings)
                {
                    ExtraPanel();
                }
                else
                {
                    StatsPanel();
                }

                TriggerViewPanel(); 
            }
            else
            {
                ShowLiabilityPanel();
            }
        }

        private void ExtraPanel()
        {
            GUILayout.BeginArea(new Rect(0, 201, 300, 100), new GUIStyle(GUI.skin.box));

            var guicolor_backup = GUI.backgroundColor;

            EditorGUILayout.LabelField("Help", centeredStyle);

            GUI.backgroundColor = Color.yellow;
            GUILayout.BeginHorizontal("helpbox");

                GUILayout.Label("Information");

            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.grey;
            GUILayout.BeginHorizontal("helpbox");
                
                GUILayout.Label("Disabled");

            GUILayout.EndHorizontal();

            GUI.backgroundColor = Color.cyan;
            GUILayout.BeginHorizontal("helpbox");

                GUILayout.Label("Prefabs");

            GUILayout.EndHorizontal();

            GUI.backgroundColor = guicolor_backup;

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, 305, 300, 100), new GUIStyle(GUI.skin.box));

            EditorGUILayout.LabelField("Extras", centeredStyle);

            GUILayout.Space(10);

            if(GUILayout.Button("VRCat Forums - World Building"))
            {
                Application.OpenURL("https://vrcat.club/forums/world-building.7/");
            }

            GUILayout.Space(10);

            if(GUILayout.Button("Show Terms of Services"))
            {
                EditorPrefs.SetBool("TriggerLogger.Liability", false);
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(0, 408, 300, 90), new GUIStyle(GUI.skin.box));

            GUILayout.Label("Created by PhaxeNor");

            if (GUILayout.Button("Discord"))
            {
                Application.OpenURL("https://discord.gg/3RBe8kB");
            }

            if (GUILayout.Button("Twitter"))
            {
                Application.OpenURL("https://twitter.com/phaxenor");
            }

            if (GUILayout.Button("Github"))
            {
                Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger");
            }

            GUILayout.EndArea();
        }

        private void ShowLiabilityPanel()
        {
            GUILayout.BeginArea(new Rect((Screen.width / 2) - 250, (Screen.height / 2) - 150, 500, 500));
            GUILayout.Label(
@"Trigger Logger

This tool was created with the intention that you could get
a better overview over your triggers. It is by no means perfect
and could contain flaws (Please report these)

It also includes some guidelines from the community
on how to improve your triggers to reduce network requests.
These are just guidelines and not rules on how you should do your triggers.

All support requests must be done in the discord server. You find the link
under 'Show Help/Extra'

Terms & Conditions

By using this tool you also accept that I, the creator, is not to be held
liable for any damages to your project.

All actions except filling the list with
triggers is done on the users own accord.
Filling the list is done using STANDARD Unity code.");

            GUILayout.Space(10);

            if(GUILayout.Button("I Accept"))
            {
                EditorPrefs.SetBool("TriggerLogger.Liability", true);
                FillList();
            }

            GUILayout.EndArea();
        }

        bool showTrigger = true;
		bool showBroadcast = true;

		Vector2 scrollPosLeft;

        string extraButton = "Show Help/Extra";

        void FilterPanel()
        {
            GUILayout.BeginArea(new Rect(0, 0, 300, 197), new GUIStyle(GUI.skin.box));

            EditorGUILayout.LabelField("Filters", centeredStyle);

            if (triggerList.Count == 0)
                GUI.enabled = false;

            filterGameObjective = EditorGUILayout.TextField("GameObject", filterGameObjective);

            triggerFilter = EditorGUILayout.Popup("Trigger", triggerFilter, triggerTypes.ToArray());

            broadFilter = EditorGUILayout.Popup("Buffer", broadFilter, broadcastEvents.ToArray());

            canBeOptimized = EditorGUILayout.Toggle("Can be optimzied", canBeOptimized);

            showPrefabs = EditorGUILayout.Toggle("Show Prefabs", showPrefabs);

            if (showPrefabs)
            {
                EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", showPrefabs);
            }
            else
            {
                EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", showPrefabs);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Reset", GUILayout.Height(20)))
            {
                triggerFilter = 0;
                broadFilter = 0;
                filterGameObjective = "";
                canBeOptimized = false;
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Export"))
            {
                Export export = new Export();
                export.ExportToTxt(triggerList);
            }

            

            if(GUILayout.Button(extraButton))
            {
                if (showSettings)
                {
                    showSettings = false;
                    extraButton = "Show Help/Extra";
                }
                else
                {
                    showSettings = true;
                    extraButton = "Hide Help/Extra";
                }

                Repaint();
            }

            GUI.enabled = true;

            GUILayout.EndArea();
        }

		void StatsPanel()
		{
			/** STATS AREA **/
			GUILayout.BeginArea(new Rect(0, 202, 300, position.height - 200), new GUIStyle( GUI.skin.box ));

			scrollPosLeft = EditorGUILayout.BeginScrollView(scrollPosLeft);

			GUIStyle style = EditorStyles.foldout;
			FontStyle previousStyle = style.fontStyle;
			style.fontStyle = FontStyle.Bold;

			showBroadcast = EditorGUILayout.Foldout(showBroadcast, "Broadcast (" + broadcastTypes.list.Sum(t => t.total) + "/"+ broadcastTypes.list.Sum(t => t.rpc) +")");
			if (showBroadcast) {

				EditorGUILayout.BeginHorizontal ();
				EditorGUIUtility.labelWidth = 83f;
				EditorGUILayout.LabelField ("Type", EditorStyles.boldLabel);
				EditorGUIUtility.labelWidth = 4f;
				EditorGUILayout.LabelField ("Total / RPC", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal ();

				addBroadcastRows (broadcastTypes.list);
			}

			GUILayout.Space (5);

			showTrigger = EditorGUILayout.Foldout(showTrigger, "Trigger (" + triggerEvents.list.Sum(t => t.total) + ")");
			if (showTrigger) {

				EditorGUILayout.BeginHorizontal ();
				EditorGUIUtility.labelWidth = 125f;
				EditorGUILayout.LabelField ("Type", EditorStyles.boldLabel);
				EditorGUIUtility.labelWidth = 0.1f;
				EditorGUILayout.LabelField ("Total", EditorStyles.boldLabel);
				EditorGUILayout.EndHorizontal ();

				GUILayout.Space (5);

				AddTriggerRows (triggerEvents.list);
			}

			style.fontStyle = previousStyle;

			EditorGUILayout.EndScrollView();

			GUILayout.EndArea();
		}

		Vector2 scrollPosRight;

		void TriggerViewPanel()
		{
			if (triggerList.Count == 0) {
				GUILayout.BeginArea (new Rect(305, (position.height / 3), position.height, position.width));
				GUILayout.Label("Click 'Check Triggers' to display the list");
				GUILayout.EndArea ();

				return;
			}

			GUILayout.BeginArea(new Rect(305, 0, position.width - 305, position.height), new GUIStyle( GUI.skin.box ));

			GUIStyle alignTextLeft = new GUIStyle( GUI.skin.label );
			alignTextLeft.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField ("GameObject", EditorStyles.boldLabel, GUILayout.Width(200));
			GUILayout.Label("Triggers", alignTextLeft, GUILayout.Width(350));
			GUILayout.Label("Broadcast", alignTextLeft, GUILayout.MinWidth(160));
			GUILayout.Label("Receivers", GUILayout.Width(70));
			GUILayout.Label("RPC", GUILayout.Width(50));
			GUILayout.Label("Is Enabled", GUILayout.Width(75));
			GUILayout.EndHorizontal(); 

			DrawUILine (Color.gray);

			scrollPosRight = EditorGUILayout.BeginScrollView(scrollPosRight);

			if (triggerList.Count > 0) {
				triggerList.ForEach (delegate (VRC_Trigger trigger) {
					if(trigger == null)
						return;

                    // Text search filter
					if(filterGameObjective != "" && !trigger.gameObject.name.ToLower().Contains(filterGameObjective.ToLower()))
						return;
 
                    // Trigger and broadcast filter
                    if (triggerFilter != 0 || broadFilter != 0) {

                        var triggerType = triggerTypes.ToArray();
                        var broadcastEvent = broadcastEvents.ToArray();

                        // Both trigger and broadcast filter has been set
                        if (triggerFilter != 0 && broadFilter != 0 && !trigger.Triggers.Exists(t => (t.TriggerType.ToString() == triggerType[triggerFilter] && t.BroadcastType.ToString() == broadcastEvent[broadFilter])))
                            return;

                        if (triggerFilter != 0 && !trigger.Triggers.Exists(t => t.TriggerType.ToString() == triggerType[triggerFilter]))
                            return;

                        if (broadFilter != 0 && !trigger.Triggers.Exists(t => t.BroadcastType.ToString() == broadcastEvent[broadFilter]))
                            return;
                    }
                   
                    TriggerRow(trigger);
				});
			}

			EditorGUILayout.EndScrollView ();

			GUILayout.EndArea();
		}

		void addBroadcastRows(List<BroadcastItem> broadcastlist)
		{
			broadcastlist.ForEach (delegate (BroadcastItem item) {
				if(item.total > 0)
					AddBroadcastRow (item.getTypeName(), item.total, item.rpc);
			});
		}

		void AddBroadcastRow(string name, int total, int rpc)
		{
			DrawUILine (Color.gray);
			EditorGUILayout.BeginHorizontal ();
			EditorGUIUtility.labelWidth = 130f;
			EditorGUILayout.LabelField (name);
			EditorGUIUtility.labelWidth = 1f;
			EditorGUILayout.LabelField (total + " / " + rpc);
			EditorGUILayout.EndHorizontal ();
		}

		void AddTriggerRows(List<TriggerItem> trigger)
		{
			trigger.ForEach (delegate (TriggerItem item) {
				if(item.total > 0)
					AddTriggerRow(item.getTypeName(), item.total);	
			});

		}

		void AddTriggerRow(string name, int count)
		{
			DrawUILine (Color.gray);
			EditorGUILayout.BeginHorizontal ();
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.LabelField (name);
			EditorGUIUtility.labelWidth = 0.1f;
			EditorGUILayout.LabelField (count.ToString());
			EditorGUILayout.EndHorizontal ();
		}

		void TriggerRow(VRC_Trigger trigger)
		{
			if (!showPrefabs && trigger.gameObject.scene.name == null)
				return;

			int triggers = trigger.Triggers.Count;
			int receivers = trigger.Triggers.Sum (t => t.Events.Count);

			int rpc = trigger.Triggers
                .Where (t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                .Where (c => c.Events.Count >= 1)
                .Sum (s => s.Events.Count);

			int broad = trigger.Triggers
                .Where (t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                .Count ();

			bool isCustomLocal = trigger.Triggers
				.Where (t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
				.Where (t => t.TriggerType == VRC_Trigger.TriggerType.Custom).Count () == 0;

			bool isEnabled = true;
			if ((!trigger.gameObject.activeInHierarchy || !trigger.isActiveAndEnabled) && trigger.gameObject.scene.name != null)
				isEnabled = false;

			var guicolor_backup = GUI.backgroundColor;

			if (rpc > broad || !isCustomLocal) {
				GUI.backgroundColor = Color.yellow;
			} else if (canBeOptimized) {
				return;
			}

			if (!isEnabled) {
				GUI.backgroundColor = Color.grey;
			} else if (trigger.gameObject.scene.name == null) {
				GUI.backgroundColor = Color.cyan; // CyanLaser
			}

			var triggerTypes = trigger.Triggers.Select (t => { return t.TriggerType.ToString(); }).ToList ();

			GUILayout.BeginHorizontal("helpbox");

			EditorGUILayout.ObjectField (trigger.gameObject, typeof(GameObject), true, GUILayout.Width(190));

			GUIStyle alignTextLeft = new GUIStyle( GUI.skin.label );
			alignTextLeft.alignment = TextAnchor.MiddleLeft;
			alignTextLeft.stretchWidth = true;

			string triggerString = string.Join (", ", triggerTypes.ToArray ());

			GUILayout.Label(triggerString, alignTextLeft, GUILayout.Width(350));

			var broadcastTypes = trigger.Triggers.Select (t => { return t.BroadcastType.ToString(); }).ToList ();
			string broadcastString = string.Join (", ", broadcastTypes.ToArray ());

			GUILayout.Label(broadcastString, alignTextLeft);

			GUILayout.Label(receivers.ToString(), GUILayout.Width(70));
			GUILayout.Label(rpc.ToString(), GUILayout.Width(50));
			GUILayout.Label(isEnabled.ToString(), GUILayout.Width(75));

			GUILayout.EndHorizontal(); 

			if (rpc > broad || !isCustomLocal) {

				GUILayout.BeginVertical ("box");

				if (rpc > broad) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Buffer triggers that have 3 or more receivers should utilize custom triggers.", alignTextLeft);
					if(GUILayout.Button("Click here for more info", GUI.skin.label)) {
						Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger/wiki/Buffer-Triggers");
					}
					GUILayout.EndHorizontal ();

				}


				if (!isCustomLocal) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Normaly custom triggers should be local, but that is not always the case.", alignTextLeft);
                    if (GUILayout.Button("Click here for more info", GUI.skin.label))
                    {
                        Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger/wiki/Custom-Triggers");
                    }
                    GUILayout.EndHorizontal();
                    
				}

				GUILayout.EndVertical ();




			}

			GUI.backgroundColor = guicolor_backup;
		}

		public static void DrawUILine(Color color, int thickness = 1, int padding = 1)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
			r.height = thickness;
			r.y+=padding/2;
			r.x-=2;
			r.width +=6;
			EditorGUI.DrawRect(r, color);
		}

		void FillList()
		{
			broadcastTypes.initList ();
			triggerEvents.initList ();

			triggerList.Clear();
            dynamicPrefabs.Clear();

			triggerList.AddRange (Resources.FindObjectsOfTypeAll<VRC_Trigger>());

            var dynamicPrefab = FindObjectOfType<VRC_SceneDescriptor> ();

            if(dynamicPrefab)
            {
                dynamicPrefabs.AddRange(dynamicPrefab.DynamicPrefabs);
            }

			triggerList.ForEach (delegate (VRC_Trigger trigger) {

				if(trigger.gameObject.scene.name == null)
					return;

				trigger.Triggers.ForEach(delegate (VRC_Trigger.TriggerEvent action) {

					int rpcs = action.Events.Count;

					if(rpcs == 0) {
						triggerEvents.emptyTrigger.Add(trigger);
						return;
					}

					broadcastTypes.list.Where(b => b.type == action.BroadcastType).Select(t => {
						t.total += 1;
						if(action.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local) 
							t.rpc += rpcs;
						return t;
					}).ToList();

					triggerEvents.list.Where(e => e.type == action.TriggerType).Select(t => {
						t.total += 1;
						return t;
					}).ToList();
				});
			});

			triggerList.Reverse ();
		}

		void OnFocus()
		{
			FillList ();
		}
	}
}