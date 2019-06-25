using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using VRCSDK2;
using UnityEngine.Networking;

/**
 * Author: Christian "PhaxeNor" Wiig
 * Website: https://github.com/PhaxeNor/Trigger-Logger
 */
namespace VRCPrefabs.TriggerLogger
{
    public class TriggerLogger : EditorWindow
    {
        public List<VRC_Trigger> triggerList = new List<VRC_Trigger>();
        public TriggerList triggers = new TriggerList();
        public List<string> triggerTypes = new List<string>();

        public List<GameObject> dynamicPrefabs = new List<GameObject>();

        public BroadcastList broadcasts = new BroadcastList();
        public List<string> broadcasTypes = new List<string>();

        public ActionList actions = new ActionList();
        public List<string> actionsTypes = new List<string>();

        public List<string> sendRpcMethods = new List<string>();
        public List<string> RpcMethodsFilter = new List<string>();
        public int RpcMethodsFlags = -1;

        [MenuItem("VRC Prefabs/Trigger Logger")]
        public static void ShowMenu()
        {
            EditorWindow window = GetWindow<TriggerLogger>("Trigger Logger");
            window.maxSize = new Vector2(2000, 9999);
            window.minSize = new Vector2(1000, 320);
            window.Show();
        }

        FilterWindow filterWindow = new FilterWindow();
        StatsWindow statsWindow = new StatsWindow();
        ExtraWindow extraWindow = new ExtraWindow();
        TriggersWindow triggersWindow = new TriggersWindow();

        public List<string> triggerFilter = new List<string>();
        public List<string> broadcastFilter = new List<string>();
        public List<string> actionFilter = new List<string>();

        // Settings
        public bool ShowPrefabs = false;
        public bool ShowEmpty = true;
        public bool ShowErrors = true;
        public bool StatsShowZero = true;
        public bool HideNonErrors = false;
        public string query = "";
        public string SendRPCquery = "";

        public int triggerFlags = -1;
        public int broadcastFlags = -1;
        public int actionsFlags = -1;
        public int rpcFlags = -1;

        void OnEnable()
        {
            triggerTypes = Enum.GetNames(typeof(VRC_Trigger.TriggerType)).Select(x => x.ToString()).ToList();
            broadcasTypes = Enum.GetNames(typeof(VRC_EventHandler.VrcBroadcastType)).Select(x => x.ToString()).ToList();
            actionsTypes = Enum.GetNames(typeof(VRC_EventHandler.VrcEventType)).Select(x => x.ToString()).ToList();

            BuildLogger();
        }

        void OnFocus()
        {
            if (EditorPrefs.HasKey("TriggerLogger.ShowPrefabs")) ShowPrefabs = EditorPrefs.GetBool("TriggerLogger.ShowPrefabs");
            if (EditorPrefs.HasKey("TriggerLogger.ShowEmpty")) ShowEmpty = EditorPrefs.GetBool("TriggerLogger.ShowEmpty");
            if (EditorPrefs.HasKey("TriggerLogger.StatsShowZero")) StatsShowZero = EditorPrefs.GetBool("TriggerLogger.StatsShowZero");

            BuildLogger();
        }

        void OnLostFocus()
        {
            savePrefs();
        }

        void OnDestroy()
        {
            savePrefs();
        }

        void savePrefs()
        {
            EditorPrefs.SetBool("TriggerLogger.ShowPrefabs", ShowPrefabs);
            EditorPrefs.SetBool("TriggerLogger.ShowEmpty", ShowEmpty);
            EditorPrefs.SetBool("TriggerLogger.StatsShowZero", StatsShowZero);
        }

        int tab = 0;

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 300, position.height), new GUIStyle(GUI.skin.box));

            tab = GUILayout.Toolbar(tab, new string[] { "Triggers", "Stats", "Extra" });
            
            switch(tab)
            {
                case 0: filterWindow.drawWindow(this); break;
                case 1: statsWindow.drawWindow(this); break;
                case 2: extraWindow.drawWindow(this); break;
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(300, 0, position.width - 300, position.height));

            triggersWindow.drawWindow(this);

            GUILayout.EndArea();
        }

        void OnSelectionChange()
        {
            this.Repaint();
        }

        void BuildLogger()
        {
            triggers.generateList();
            broadcasts.generateList();
            actions.generateList();

            triggerList.Clear();
            triggerList.AddRange(Resources.FindObjectsOfTypeAll<VRC_Trigger>());

            triggerList.ForEach(t =>
            {
                if (t.gameObject.scene.name == null) return;

                t.Triggers.ForEach(tr =>
                {
                    broadcasts.list.Where(l => l.Type == tr.BroadcastType).Select(b =>
                    {
                        b.Total += 1;
                        if (tr.BroadcastType != VRC_EventHandler.VrcBroadcastType.Local)
                        {
                            b.RPC += tr.Events.Sum(e => e.ParameterObjects.Count());
                        }

                        return b;
                    }).ToList();

                    triggers.list.Where(tl => tl.Type == tr.TriggerType).Select(l =>
                    {
                        return l.Total += 1;
                    }).ToList();

                    tr.Events.ForEach(e =>
                    {
                        actions.list.Where(a => a.Type == e.EventType).Select(s => { return s.Total += 1; }).ToList();

                        if (e.EventType == VRC_EventHandler.VrcEventType.SendRPC)
                        {
                            sendRpcMethods.Add(e.ParameterString);
                        }
                    });
                });
            });

            triggerList.Reverse();
        }

        static string GetCurrentVersion()
        {
            string currentVersion = "";
            string versionTextPath = Application.dataPath + "/VRCPrefabs/Scripts/TriggerLogger/VERSION.md";
            if (System.IO.File.Exists(versionTextPath))
            {
                string[] versionFileLines = System.IO.File.ReadAllLines(versionTextPath);
                if (versionFileLines.Length > 0)
                    currentVersion = versionFileLines[0];
            }
            return currentVersion;
        }
    }
}