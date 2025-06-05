#if !BALANCY_CREATOR && !BALANCY_SERVER
using System;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Balancy.Editor
{
    public class Balancy_Symbols : EditorWindow
    {
        const string BalancyDefine = "BALANCY";

        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            // AddBalancySymbols(BalancyDefine);
            CheckBalancyMainClass();
        }
        
        private static void AddBalancySymbols(string newDefines)
        {
            var newDefineSymbols = newDefines
                .Split(';')
                .Select(d => d.Trim())
                .ToList();
            
            foreach (BuildTarget target in System.Enum.GetValues(typeof(BuildTarget)))
            {
                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

                if (group == BuildTargetGroup.Unknown)
                {
                    continue;
                }

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);

                var defineSymbols = defines
                    .Split(';')
                    .Select(d => d.Trim())
                    .ToList();

                string definesToAdd = string.Empty;
                
                foreach (var newD in newDefineSymbols)
                {
                    bool found = defineSymbols.Any(symbol => newD.Equals(symbol));

                    if (!found)
                        definesToAdd += ";" + newD;
                }

                if (!string.IsNullOrEmpty(definesToAdd))
                {
                    try
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(group,  defines + definesToAdd);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogErrorFormat("Could not add Balancy define symbols for build target: {0} group: {1}, {2}", target, group, e);
                    }
                }
            }
        }

        private static void CheckBalancyMainClass()
        {
#if !BALANCY_DEV
            var type = Type.GetType("Balancy.Main, Balancy");
            bool classExists = (null != type);
            if (!classExists)
                PluginUtils.GenerateCodeForStartFileExternal();
#endif
        }
    }
}
#endif