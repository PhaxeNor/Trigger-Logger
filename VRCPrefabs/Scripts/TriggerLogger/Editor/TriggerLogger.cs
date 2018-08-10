using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRCSDK2;

/**
 * Author: PhaxeNor
 * Github: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCP.TriggerLogger
{
    public class TriggerLogger : EditorWindow {

        readonly string VERSION = "0.3.1";

        GUIStyle centeredStyle;

        List<VRC_Trigger> triggerList = new List<VRC_Trigger>();

        List<GameObject> dynamicPrefabs = new List<GameObject>();

        BroadcastList broadcastTypes = new BroadcastList();

        TriggerList triggerEvents = new TriggerList();

        ActionList actionEvents = new ActionList();

        List<string> triggerFilter = new List<string>();
        List<string> broadcastFilter = new List<string>();
        List<string> actionFilter = new List<string>();

        List<string> triggerTypes = new List<string>();
        List<string> broadcastEvents = new List<string>();
        List<string> actionTypes = new List<string>();

        string filterGameObjective = "";

        bool canBeOptimized = false;

        bool emptyTriggers = false;

        bool showPrefabs;
        bool alwaysExpand;
        string alwaysExpandButton = "Expand All";

        Dictionary<int, bool> expanded = new Dictionary<int, bool>();

        [MenuItem("VRC Prefabs/Trigger Logger")]
        public static void ShowMenu()
        {
            EditorWindow window = EditorWindow.GetWindow<TriggerLogger>("Trigger Logger");
            window.maxSize = new Vector2(1150, 9999);
            window.minSize = new Vector2(1150, 500);
            window.Show();
        }

        void OnEnable()
        {
            broadcastTypes.initList();
            triggerEvents.initList();
            actionEvents.initList();

            triggerTypes = Enum.GetNames(typeof(VRC_Trigger.TriggerType)).Select(x => x.ToString()).ToList();

            broadcastEvents = Enum.GetNames(typeof(VRC_EventHandler.VrcBroadcastType)).Select(x => x.ToString()).ToList();

            actionTypes = Enum.GetNames(typeof(VRC_EventHandler.VrcEventType)).Select(x => x.ToString()).ToList();

            if (!EditorPrefs.HasKey("TriggerLogger.ShowPrefabs")) {
                EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", true);
            } else {
                showPrefabs = EditorPrefs.GetBool("TriggerLogger.ShowPrefabs");
            }

            if (EditorPrefs.GetBool("TriggerLogger.Liability"))
            {
                FillList();
            }
        }

        private void SetFilters()
        {
            broadcastEvents.ForEach(b =>
            {
                broadcastFilter.Add(b.ToString());
            });

            triggerTypes.ForEach(b =>
            {
                triggerFilter.Add(b.ToString());
            });

            actionTypes.ForEach(b =>
            {
                actionFilter.Add(b.ToString());
            });


        }

        int tab = 1;

        void OnGUI()
        {
            centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;
            centeredStyle.fontStyle = FontStyle.Bold;

            if (EditorPrefs.GetBool("TriggerLogger.Liability"))
            {
                GUILayout.BeginArea(new Rect(0, 0, 300, position.height), new GUIStyle(GUI.skin.box));

                tab = GUILayout.Toolbar(tab, new string[] { "Stats", "Actions", "Extra" });
                switch (tab)
                {
                    case 0:
                        StatsPanel();
                        break;

                    case 1:
                        FilterPanel();
                        break;

                    case 2:
                        ExtraPanel();
                        break;
                }

                GUILayout.EndArea();

                TriggerViewPanel();

            }
            else
            {
                ShowLiabilityPanel();
            }
        }

        private void ExtraPanel()
        {
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

            GUI.backgroundColor = Color.red;
            GUILayout.BeginHorizontal("helpbox");

            GUILayout.Label("Empty Action/Receiver");

            GUILayout.EndHorizontal();

            GUI.backgroundColor = guicolor_backup;

            DrawUILine(Color.gray);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("Extras", centeredStyle);

            GUILayout.Space(10);

            if (GUILayout.Button("VRCat Forums - World Building"))
            {
                Application.OpenURL("https://vrcat.club/forums/world-building.7/");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Show Terms of Services"))
            {
                EditorPrefs.SetBool("TriggerLogger.Liability", false);
            }

            GUILayout.Label("Created by PhaxeNor. V" + VERSION);

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
        }

        private void ShowLiabilityPanel()
        {
            GUILayout.BeginArea(new Rect((position.width / 2) - 250, (position.height / 2) - 150, 500, 500));
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

            if (GUILayout.Button("I Accept"))
            {
                EditorPrefs.SetBool("TriggerLogger.Liability", true);
                FillList();
            }

            GUILayout.EndArea();
        }

        bool showTrigger = true;
        bool showBroadcast = true;
        bool showActions = true;

        Vector2 scrollPosLeft;



        List<string> triggerSelected = new List<string>();

        public void Callback(object value)
        {
             if(triggerSelected.Contains(value.ToString()))
            {
                triggerSelected.Remove(value.ToString());
            }
            else
            {
                triggerSelected.Add(value.ToString());
            }
        }

        int triggerFlags = -1;
        int broadcastFlags = -1;
        int actionFlags = -1;

        void FilterPanel()
        {
            EditorGUILayout.LabelField("Filters", centeredStyle);

            if (triggerList.Count == 0)
                GUI.enabled = false;

            filterGameObjective = EditorGUILayout.TextField("GameObject", filterGameObjective);

            var tArray = triggerTypes.ToArray();

            int oldTriggerflags = triggerFlags;

            triggerFlags = EditorGUILayout.MaskField("Triggers", triggerFlags, tArray);

            if(triggerFlags != oldTriggerflags)
            {
                triggerFilter.Clear();
                for (int i = 0; i < tArray.Length; i++)
                {
                    if ((triggerFlags & (1 << i)) == (1 << i)) triggerFilter.Add(tArray[i]);
                }
            }

            var bArray = broadcastEvents.ToArray();

            int oldBroadcastFlags = broadcastFlags;

            broadcastFlags = EditorGUILayout.MaskField("Broadcast", broadcastFlags, bArray);

            if (broadcastFlags != oldBroadcastFlags)
            {
                broadcastFilter.Clear();
                for (int i = 0; i < bArray.Length; i++)
                {
                    if ((broadcastFlags & (1 << i)) == (1 << i)) broadcastFilter.Add(bArray[i]);
                }
            }

            var aArray = actionTypes.ToArray();

            int oldActionFlags = actionFlags;

            actionFlags = EditorGUILayout.MaskField("Actions", actionFlags, aArray);

            if (actionFlags != oldActionFlags)
            {
                actionFilter.Clear();
                for (int i = 0; i < aArray.Length; i++)
                {
                    if ((actionFlags & (1 << i)) == (1 << i)) actionFilter.Add(aArray[i]);
                }
            }

            canBeOptimized = EditorGUILayout.Toggle("Can be optimzied", canBeOptimized);

            emptyTriggers = EditorGUILayout.Toggle("Empty Triggers", emptyTriggers);

            GUILayout.Space(5);

            showPrefabs = EditorGUILayout.Toggle("Show Prefabs", showPrefabs);

            GUILayout.Space(5);

            if (GUILayout.Button(alwaysExpandButton, GUILayout.Height(20)))
            {
                alwaysExpand = alwaysExpand ? false : true;
                alwaysExpandButton = alwaysExpandButton == "Expand All" ? "Minimize All" : "Expand All";
                expanded.Clear();
            }

            if (GUILayout.Button("Reset", GUILayout.Height(20)))
            {
                triggerFlags = -1;
                broadcastFlags = -1;
                actionFlags = -1;
                SetFilters();
                filterGameObjective = "";
                canBeOptimized = false;
                emptyTriggers = false;
                alwaysExpand = false;
                alwaysExpandButton = "Expand All";
                expanded.Clear();

                Repaint();
            }

            if (showPrefabs)
            {
                EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", showPrefabs);
            }
            else
            {
                EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", showPrefabs);
            }

            if (GUI.Button(new Rect(5, position.height - 25, 290, 20), "Export to Text"))
            {
                Export export = new Export();
                export.ExportToTxt(triggerList);
            }

            GUI.enabled = true;
        }

		void StatsPanel()
		{
			/** STATS AREA **/
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

				AddStatsBroadcastRow (broadcastTypes.list);
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

                AddStatsTriggerRow(triggerEvents.list);
            }

            GUILayout.Space(5);

            showActions = EditorGUILayout.Foldout(showActions, "Actions (" + actionEvents.list.Sum(t => t.total) + ")");
            if (showActions)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 125f;
                EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
                EditorGUIUtility.labelWidth = 0.1f;
                EditorGUILayout.LabelField("Total", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                AddStatsActionRow(actionEvents.list);
            }

            style.fontStyle = previousStyle;

			EditorGUILayout.EndScrollView();
		}

		Vector2 scrollPosRight;

		void TriggerViewPanel()
		{
			if (triggerList.Count == 0) {
				GUILayout.BeginArea (new Rect(305, (position.height / 3), position.height, position.width));
				GUILayout.Label("No Triggers Found!");
				GUILayout.EndArea ();

				return;
			}

			GUILayout.BeginArea(new Rect(305, 0, 850, position.height), new GUIStyle( GUI.skin.box ));

			GUIStyle alignTextLeft = new GUIStyle( GUI.skin.label );
			alignTextLeft.alignment = TextAnchor.MiddleLeft;

			GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField ("GameObject", EditorStyles.boldLabel, GUILayout.Width(200));
			GUILayout.Label("Triggers", alignTextLeft, GUILayout.Width(225));
			GUILayout.Label("Broadcast", alignTextLeft, GUILayout.Width(210));
			GUILayout.Label("Actions", GUILayout.Width(100));
			GUILayout.Label("RPC", GUILayout.Width(50));
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

                    // if(triggerFlags != 0 || triggerFlags != -1 || broadcastFlags != 0 || broadcastFlags != -1)
                    if (triggerFlags != 0 || broadcastFlags != 0 || actionFlags != 0)
                    {
                        if (triggerFlags != 0 && triggerFlags != -1 && !trigger.Triggers.Exists(t => { return triggerFilter.Exists(o => t.TriggerType.ToString() == o); }))
                            return;

                        if (broadcastFlags != 0 && broadcastFlags != -1 && !trigger.Triggers.Exists(t => { return broadcastFilter.Exists(o => t.BroadcastType.ToString() == o); }))
                            return;

                        if (actionFlags != 0 && actionFlags != -1 && !trigger.Triggers.Exists(t => { return t.Events.Exists(e => actionFilter.Exists(o => e.EventType.ToString() == o)); }))
                            return;
                    }
                   
                    TriggerRow(trigger);
				});
			}

			EditorGUILayout.EndScrollView ();

			GUILayout.EndArea();
		}

        /**
         * Add a row to the broadcast stats list
         */ 
        void AddStatsBroadcastRow(List<BroadcastItem> broadcastlist)
        {
            broadcastlist.ForEach(delegate (BroadcastItem item)
            {
                if (item.total > 0)
                {
                    DrawUILine(Color.gray);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 130f;
                    EditorGUILayout.LabelField(item.getTypeName());
                    EditorGUIUtility.labelWidth = 1f;
                    EditorGUILayout.LabelField(item.total + " / " + item.rpc);
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        /***
         * Add a row to the trigger stats List
         */
        private void AddStatsTriggerRow(List<TriggerItem> trigger)
		{
            trigger.ForEach(delegate (TriggerItem item)
            {
                if (item.total > 0)
                {
                    DrawUILine(Color.gray);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 150f;
                    EditorGUILayout.LabelField(item.getTypeName());
                    EditorGUIUtility.labelWidth = 0.1f;
                    EditorGUILayout.LabelField(item.total.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            });
		}

        private void AddStatsActionRow(List<ActionItem> trigger)
        {
            trigger.ForEach(delegate (ActionItem item)
            {
                if (item.total > 0)
                {
                    DrawUILine(Color.gray);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 150f;
                    EditorGUILayout.LabelField(item.getTypeName());
                    EditorGUIUtility.labelWidth = 0.1f;
                    EditorGUILayout.LabelField(item.total.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            });
        }

        void TriggerRow(VRC_Trigger trigger)
		{
			if (!showPrefabs && trigger.gameObject.scene.name == null)
				return;

			int triggers = trigger.Triggers.Count;
			int receivers = trigger.Triggers.Sum (t => t.Events.Count);

            int rpc = trigger.Triggers
                .Where(t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                .Where(c => c.Events.Count >= 1)
                .Sum(su => su.Events.Sum(s => s.ParameterObjects.Count()));

			int broad = trigger.Triggers
                .Where (t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                .Count ();

            int receiver = 0;
            int emptyReceivers = 0;

            trigger.Triggers.ForEach(t =>
            {
                receiver += t.Events.Sum(s => s.ParameterObjects.Count());

                t.Events.ForEach(e => {

                    if(e.ParameterObjects.Count() == 0)
                    {
                        emptyReceivers += 1;
                    }

                    foreach(var o in e.ParameterObjects)
                    {
                        if(!o || o == null)
                        {
                            emptyReceivers += 1;
                        }
                    }
                    //emptyReceivers += e.ParameterObjects.ToList().Where(s => s == null).Count();
                    // Debug.Log(trigger.name + " - " + emptyReceivers);
                });
            });

			bool isCustomLocal = trigger.Triggers
				.Where (t => t.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
				.Where (t => t.TriggerType == VRC_Trigger.TriggerType.Custom)
                .Where (t => t.Events.Count() > 2).Count () == 0;

			bool isEnabled = true;
			if ((!trigger.gameObject.activeInHierarchy || !trigger.isActiveAndEnabled) && trigger.gameObject.scene.name != null)
				isEnabled = false;

			var guicolor_backup = GUI.backgroundColor;

            if (receiver == 0 || emptyReceivers > 0) {
                GUI.backgroundColor = Color.red;
            } else if (emptyTriggers && receiver != 0 || emptyTriggers && emptyReceivers == 0) {
                return;
            } else if (rpc > broad || !isCustomLocal) {
                GUI.backgroundColor = Color.yellow;
            } else if (canBeOptimized) {
                return;
            } else if (!isEnabled) {
                GUI.backgroundColor = Color.grey;
            } else if (trigger.gameObject.scene.name == null) {
                GUI.backgroundColor = Color.cyan; // CyanLaser
            }

            GUILayout.BeginHorizontal("helpbox");

            EditorGUILayout.ObjectField (trigger.gameObject, typeof(GameObject), true, GUILayout.Width(190));

			GUIStyle alignTextLeft = new GUIStyle( GUI.skin.label );
			alignTextLeft.alignment = TextAnchor.MiddleLeft;
			alignTextLeft.stretchWidth = true;

            /** Trigger List **/
            GUILayout.BeginVertical();

            if (expanded.ContainsKey(trigger.gameObject.GetInstanceID()) || alwaysExpand)
            {
                //foreach (string tt in triggerTypes)
                trigger.Triggers.ForEach(t => {

                    if (t.TriggerType == VRC_Trigger.TriggerType.Custom)
                    {
                        GUILayout.Label(t.TriggerType.ToString() + " (" + t.Name + ")", alignTextLeft, GUILayout.Width(200));
                    }
                    else
                    {
                        GUILayout.Label(t.TriggerType.ToString(), alignTextLeft, GUILayout.Width(200));
                    }
                });
            }
            else
            {
                GUILayout.Label("", alignTextLeft, GUILayout.Width(200));
            }

            GUILayout.EndVertical();

            /** Broadcast List **/
            GUILayout.BeginVertical();

            if (expanded.ContainsKey(trigger.gameObject.GetInstanceID()) || alwaysExpand)
            {
                trigger.Triggers.ForEach(t => {

                    GUILayout.Label(t.BroadcastType.ToString(), alignTextLeft, GUILayout.Width(240));

                });
            }
            else
            {
                GUILayout.Label("", alignTextLeft, GUILayout.Width(200));
            }

            GUILayout.EndVertical();

            /** Receivers List **/
            GUILayout.BeginVertical();

            if (expanded.ContainsKey(trigger.gameObject.GetInstanceID()) || alwaysExpand)
            {

                trigger.Triggers.ForEach(tr =>
                {
                    GUILayout.Label(tr.Events.Count.ToString(), alignTextLeft, GUILayout.Width(59));
                });
            }
            else
            {
                GUILayout.Label(receivers.ToString(), GUILayout.Width(50));
            }

            GUILayout.EndVertical();

            /** Begin RPC List **/
            GUILayout.BeginVertical();

            if (expanded.ContainsKey(trigger.gameObject.GetInstanceID()) || alwaysExpand)
            {
                trigger.Triggers.ForEach(tr =>
                {
                    if(tr.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                    {
                        int receiversCount = tr.Events.Sum(s => s.ParameterObjects.Count());

                        GUILayout.Label(receiversCount.ToString(), alignTextLeft, GUILayout.Width(50));
                    }
                    else
                    {
                        GUILayout.Label("0", alignTextLeft, GUILayout.Width(50));
                    }
                });
            }
            else
            {
                GUILayout.Label(rpc.ToString(), GUILayout.Width(50));
            }

            GUILayout.EndVertical();

			GUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                if(expanded.ContainsKey(trigger.gameObject.GetInstanceID()))
                {
                    expanded.Remove(trigger.gameObject.GetInstanceID());
                    Repaint();
                    
                }
                else
                {
                    expanded.Add(trigger.gameObject.GetInstanceID(), true);
                    Repaint();
                }
            }

            if (rpc > broad || !isCustomLocal || receiver == 0 || emptyReceivers > 0) {

				GUILayout.BeginVertical ("box");

                if (receiver == 0 || emptyReceivers > 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("A trigger contain empty action(s) or receiver(s).", alignTextLeft, GUILayout.MaxWidth(600));
                    if (GUILayout.Button("Click here for more info", GUI.skin.label))
                    {
                        Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger/wiki/Empty-Triggers");
                    }
                    GUILayout.EndHorizontal();
                }

                if (rpc > broad) {
					GUILayout.BeginHorizontal ();
					GUILayout.Label ("A trigger with Buffer Broadcast have multiple receiver.", alignTextLeft, GUILayout.MaxWidth(600));
					if(GUILayout.Button("Click here for more info", GUI.skin.label)) {
						Application.OpenURL("https://github.com/PhaxeNor/Trigger-Logger/wiki/Buffer-Triggers");
					}
					GUILayout.EndHorizontal ();

				}


				if (!isCustomLocal) {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("A Custom trigger with Buffer Broadcast has multiple receiver.", alignTextLeft, GUILayout.MaxWidth(600));
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

		public static void DrawUILine(Color color, int width = 6, int thickness = 1, int padding = 1)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
			r.height = thickness;
			r.y+=padding/2;
			r.x-=1;
			r.width +=width;
			EditorGUI.DrawRect(r, color);
		}

		void FillList()
		{
			broadcastTypes.initList ();
			triggerEvents.initList ();
            actionEvents.initList();

            triggerList.Clear();
            dynamicPrefabs.Clear();

			triggerList.AddRange (Resources.FindObjectsOfTypeAll<VRC_Trigger>());

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
                        if (action.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                            t.rpc += action.Events.Sum(su => su.ParameterObjects.Count());
                        return t;
					}).ToList();

					triggerEvents.list.Where(e => e.type == action.TriggerType).Select(t => {
						t.total += 1;
						return t;
					}).ToList();

                    action.Events.ForEach(ae =>
                    {
                        actionEvents.list.Where(e => e.type == ae.EventType).Select(t => {
                            t.total += 1;
                            return t;
                        }).ToList();
                    });
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
 