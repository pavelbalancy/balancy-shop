#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Balancy.Dictionaries;
using PluginInfo = Balancy.Editor.PluginUtils.PluginInfo;
using PluginInfoBase = Balancy.Editor.PluginUtils.PluginInfoBase;
using PluginsFile = Balancy.Editor.PluginUtils.PluginsFile;

namespace Balancy.Editor
{
    public class Balancy_Plugins
    {
        private static readonly GUIStyle LABEL_BOLD = new GUIStyle(GUI.skin.GetStyle("label")) {fontStyle = FontStyle.Bold};
        private static readonly GUIStyle LABEL_WRAP = new GUIStyle(GUI.skin.GetStyle("label")) {wordWrap = true};

        private static readonly GUILayoutOption LAYOUT_NAME = GUILayout.Width(100);
        private static readonly GUILayoutOption LAYOUT_VERSION = GUILayout.Width(50);
        private static readonly GUILayoutOption LAYOUT_BUTTON = GUILayout.Width(100);
        private static readonly GUILayoutOption LAYOUT_BUTTON_REFRESH = GUILayout.Width(30);
        
        private static WebUtils.RequestsConfig _config;

        enum Status
        {
            None,
            Downloading,
            Ready
        }
        
        private PluginsFile _pluginsLocal;
        private PluginsFile _pluginsOriginal;
        private PluginsFile _pluginsRemote;
        private Status _status;

        private void RenderPluginInfo(PluginInfo plugin, PluginInfo localInfo, Func<PluginInfo, bool> canInstall, Func<PluginInfo, bool> canRemove)
        {
            var localVersion = localInfo.Version;

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(new GUIContent
            {
                text = plugin.Name,
                tooltip = plugin.Description
            }, LABEL_BOLD, LAYOUT_NAME);
            GUILayout.Label(localVersion == null ? string.Empty : "v" + localVersion.ToString(), LAYOUT_VERSION);

            if (plugin.Installing)
            {
                GUI.enabled = true;
                var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                EditorGUI.ProgressBar(rect, plugin.InstallProgress, "Installing v" + plugin.Version.ToString());
                GUI.enabled = false;
            }
            else
            {
                if (localVersion != null && localVersion.IsHigherOrEqualThan(plugin.Version))
                    GUILayout.Label("Up to date");

                GUILayout.FlexibleSpace();

                if (localVersion == null || !localVersion.IsHigherOrEqualThan(plugin.Version))
                {
                    if (localVersion == null)
                    {
                        if (GUILayout.Button("Install v" + plugin.Version.ToString(), LAYOUT_BUTTON))
                        {
                            if (canInstall(plugin))
                                plugin.InstallPlugin();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Update v" + plugin.Version.ToString(), LAYOUT_BUTTON))
                        {
                            if (canInstall(plugin))
                                plugin.UpdatePlugin(localInfo);
                        }
                    }
                }

                if (plugin.CanBeRemoved && localVersion != null)
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("TreeEditor.Trash"), LAYOUT_BUTTON_REFRESH))
                    {
                        if (canRemove(plugin))
                            localInfo.RemovePlugin();
                    }
                }

                if (!string.IsNullOrEmpty(plugin.Documentation))
                {
                    if (GUILayout.Button(EditorGUIUtility.IconContent("_Help"), LAYOUT_BUTTON_REFRESH))
                        Application.OpenURL(plugin.Documentation);
                }
            }

            GUILayout.EndHorizontal();
        }

        public Balancy_Plugins(EditorWindow parent)
        {
            _config = new WebUtils.RequestsConfig(Constants.GeneralConstants.CDN_URLS, 15, 3);
            PluginUtils.onUpdatePluginInfo = UpdateLocalPluginInfo;
            PluginUtils.onRemovePluginInfo = RemoveLocalPluginInfo;
            PluginUtils.onRedraw = parent.Repaint;
            PluginUtils.UpdateLocalPath(parent);
            Refresh();
        }

        private void UpdateLocalPluginInfo(PluginInfo pluginInfo)
        {
            _pluginsLocal.UpdatePluginInfo(pluginInfo);
            SynchLocalPluginsInfo();
        }

        private void RemoveLocalPluginInfo(PluginInfo pluginInfo)
        {
            _pluginsLocal.RemovePluginInfo(pluginInfo);
            SynchLocalPluginsInfo();
        }

        private void SynchLocalPluginsInfo()
        {
            PluginUtils.onRedraw();
            SaveLocalFile();
            GenerateCodeForStartFilePrivate();
        }

        private void SaveLocalFile()
        {
            _pluginsLocal.SaveLocally();
        }

        private void GenerateCodeForStartFilePrivate()
        {
            var code = _pluginsRemote.GetMainCode();
            PluginUtils.GenerateCodeForStartFile(_pluginsLocal, code);
        }

        public void Render()
        {
            GUI.enabled = true;

            GUILayout.BeginVertical(EditorStyles.helpBox);

            RenderHeader();

            if (_pluginsRemote == null)
            {
                if (_status == Status.Ready)
                    GUILayout.Label("Something went wrong. Please try to refresh the page.");
                else
                    GUILayout.Label("Updating...");
            }
            else
                RenderAllPlugins();

            GUILayout.EndVertical();
        }

        private void RenderHeader()
        {
            GUILayout.BeginHorizontal(EditorStyles.label);
            GUILayout.Label("Additional Plugins");
            GUI.enabled = _status != Status.Downloading;
            if (GUILayout.Button(EditorGUIUtility.IconContent("Refresh"), LAYOUT_BUTTON_REFRESH))
                Refresh();
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        private bool IsAnythingInstalling()
        {
            foreach (var plugin in _pluginsRemote.Plugins)
                if (plugin.Installing)
                    return true;
            return false;
        }

        private void RenderAllPlugins()
        {
            if (_pluginsLocal.EditorInfo.Version.IsHigherOrEqualThan(_pluginsRemote.EditorInfo.MinVersion))
            {
                if (!_pluginsOriginal.EditorInfo.Version.IsHigherOrEqualThan(_pluginsRemote.EditorInfo.Version))
                    RenderNormalEditorUpdate();

                RenderEditorUpdateMessage();

                GUI.enabled = !IsAnythingInstalling();
                foreach (var plugin in _pluginsRemote.Plugins)
                {
                    var local = _pluginsLocal.GetOrCreatePluginInfo(plugin.Name);
                    RenderPluginInfo(plugin, local, CanInstall, CanRemove);
                }
            }
            else
                RenderForceEditorUpdate();
        }

        private void RenderEditorUpdateMessage()
        {
            if (_pluginsRemote.EditorInfo.Message == null || string.IsNullOrEmpty(_pluginsRemote.EditorInfo.Message.Text))
                return;

            if (!_pluginsLocal.EditorInfo.Version.IsHigherOrEqualThan(_pluginsRemote.EditorInfo.Message.MinVersion) ||
                !_pluginsRemote.EditorInfo.Message.MaxVersion.IsHigherOrEqualThan(_pluginsLocal.EditorInfo.Version)) return;

            GUI.color = Color.cyan;
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(_pluginsRemote.EditorInfo.Message.Text, LABEL_WRAP);
            GUI.color = Color.white;

            if (_pluginsRemote.EditorInfo.Message.Buttons != null)
            {
                GUILayout.BeginHorizontal(EditorStyles.label);
                foreach (var btn in _pluginsRemote.EditorInfo.Message.Buttons)
                {
                    if (GUILayout.Button(btn.title, LAYOUT_BUTTON))
                        Application.OpenURL(btn.Url);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void RenderNormalEditorUpdate()
        {
            GUI.color = Color.green;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("New version of Balancy plugin is now available! Hooray!");
            GUI.color = Color.white;
            if (GUILayout.Button("Update v" + _pluginsRemote.EditorInfo.Version.ToString(), LAYOUT_BUTTON))
                Application.OpenURL(_pluginsRemote.EditorInfo.DownloadUrl);
            GUILayout.EndHorizontal();
        }

        private void RenderForceEditorUpdate()
        {
            GUI.color = Color.red;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("Please update Balancy plugin to the newest version!");
            GUI.color = Color.white;
            if (GUILayout.Button("Update v" + _pluginsRemote.EditorInfo.Version.ToString(), LAYOUT_BUTTON))
                Application.OpenURL(_pluginsRemote.EditorInfo.DownloadUrl);
            GUILayout.EndHorizontal();
        }

        private bool CanInstall(PluginInfo pluginInfo)
        {
            var deps = GetListOfMissingDependencies(pluginInfo);

            if (string.IsNullOrEmpty(deps))
                return true;

            EditorUtility.DisplayDialog("Warning", deps, "Ok");
            return false;
        }

        private bool CanRemove(PluginInfo pluginInfo)
        {
            var deps = GetListOfInversedMissingDependencies(pluginInfo);

            if (string.IsNullOrEmpty(deps))
                return true;

            EditorUtility.DisplayDialog("Warning", deps, "Ok");
            return false;
        }

        private string GetListOfMissingDependencies(PluginInfo pluginInfo)
        {
            List<PluginInfoBase> missingDeps = new List<PluginInfoBase>();
            foreach (var dep in pluginInfo.Dependencies)
            {
                var localPlugin = _pluginsLocal.GetPluginInfo(dep.Name);
                if (localPlugin?.Version == null || !localPlugin.Version.IsHigherOrEqualThan(dep.Version))
                    missingDeps.Add(dep);
            }

            if (missingDeps.Count == 0)
                return null;

            string depStr = "Please Install the next plugins first:\n";
            foreach (var dep in missingDeps)
                depStr += string.Format("{0} : v{1}\n", dep.Name, dep.Version.ToString());

            return depStr;
        }

        private string GetListOfInversedMissingDependencies(PluginInfo pluginInfo)
        {
            List<PluginInfoBase> missingDeps = new List<PluginInfoBase>();
            foreach (var plugin in _pluginsLocal.Plugins)
            {
                if (plugin.Dependencies == null)
                    continue;

                foreach (var dep in plugin.Dependencies)
                {
                    if (string.Equals(pluginInfo.Name, dep.Name))
                        missingDeps.Add(plugin);
                }
            }

            if (missingDeps.Count == 0)
                return null;

            string depStr = "Other plugins depend on this one:\n";
            foreach (var dep in missingDeps)
                depStr += string.Format("{0} : v{1}\n", dep.Name, dep.Version.ToString());

            return depStr;
        }

        private void Refresh()
        {
            _status = Status.Downloading;
            _pluginsRemote = null;

            _pluginsLocal = PluginUtils.GetLocalPlugins();
            _pluginsOriginal = PluginUtils.GetOriginalPlugins();

            if (_pluginsLocal == null)
            {
                _pluginsLocal = PluginUtils.GetOriginalPlugins();
                SaveLocalFile();
            }
            else
                SynchVersionsFromOriginalFile();

            
#if LOCAL_PLUGINS_TEST
        _pluginsRemote = PluginUtils.GetRemotePluginsForTest();
        _status = Status.Ready;
#else
            LoadRemoteConfig();
#endif
        }

        private void SynchVersionsFromOriginalFile()
        {
            _pluginsLocal.EditorInfo = _pluginsOriginal.EditorInfo;
            for (int i = 0; i < _pluginsLocal.Plugins.Count; i++)
            {
                var local = _pluginsLocal.Plugins[i];
                var original = _pluginsOriginal.GetPluginInfo(local.Name);
                if (original != null)
                {
                    if (original.Version.IsHigherOrEqualThan(local.Version))
                        _pluginsLocal.Plugins[i] = original;
                }
            }
        }

        private async void LoadRemoteConfig()
        {
            var result = await Loader.TryLoadFile(PluginUtils.PLUGINS_ADDRESS_REMOTE, _config);
            if (string.IsNullOrEmpty(result))
            {
                EditorUtility.DisplayDialog("Error", "Something went wrong", "Ok");
            }
            else
            {
                var text = result;
                _pluginsRemote = JsonConvert.DeserializeObject<PluginsFile>(text);
                _status = Status.Ready;
            }
        }
    }
}
#endif