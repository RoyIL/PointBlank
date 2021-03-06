﻿using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PointBlank.API;
using PointBlank.API.Plugins;
using PointBlank.API.Services;
using PointBlank.API.DataManagment;
using SM = PointBlank.Framework.ServiceManager;

namespace PointBlank.Services.PluginManager
{
    [Service("PluginManager", true)]
    internal class PluginManager : Service
    {
        #region Info
        public static readonly string ConfigurationPath = ServerInfo.ConfigurationsPath + "/Plugins"; // Set the plugins configuration path
        #endregion

        #region Variables
        private static List<PluginWrapper> _plugins = new List<PluginWrapper>(); // List of plugins
        private static List<Assembly> _libraries = new List<Assembly>(); // List of libraries
        #endregion

        #region Properties
        public static PluginWrapper[] Plugins { get { return _plugins.ToArray(); } } // Returns the plugins
        public static Assembly[] Libraries { get { return _libraries.ToArray();  } } // Returns the libraries

        public UniversalData UniConfig { get; private set; } // The universal config data
        public JsonData JSONConfig { get; private set; } // The JSON config data
        #endregion

        #region Override Functions
        public override void Load()
        {
            Logging.Log("Loading plugins...");
            if (!Directory.Exists(ServerInfo.LibrariesPath))
                Directory.CreateDirectory(ServerInfo.LibrariesPath); // Create libraries directory
            if (!Directory.Exists(ServerInfo.PluginsPath))
                Directory.CreateDirectory(ServerInfo.PluginsPath); // Create plugins directory
            if (!Directory.Exists(ConfigurationPath))
                Directory.CreateDirectory(ConfigurationPath); // Create plugins directory

            // Setup the config
            UniConfig = new UniversalData(SM.ConfigurationPath + "\\PluginManager");
            JSONConfig = UniConfig.GetData(EDataType.JSON) as JsonData;
            LoadConfig();

            foreach (string library in Directory.GetFiles(ServerInfo.LibrariesPath, "*.dll")) // Get all the dll files in libraries directory
                _libraries.Add(Assembly.Load(File.ReadAllBytes(library))); // Load and add the library
            foreach(string plugin in Directory.GetFiles(ServerInfo.PluginsPath, "*.dll")) // Get all the dll files in plugins directory
            {
                try
                {
                    PluginWrapper wrapper = new PluginWrapper(plugin); // Create the plugin wrapper

                    _plugins.Add(wrapper); // Add the wrapper
                    if (!wrapper.Load() && !PluginConfiguration.ContinueOnError)
                        break;
                }
                catch (Exception ex)
                {
                    Logging.LogError("Error initializing plugin!", ex);
                    if (!PluginConfiguration.ContinueOnError)
                        break;
                }
            }
            Logging.Log("Plugins loaded!");
        }

        public override void Unload()
        {
            Logging.Log("Unloading plugins...");
            foreach (PluginWrapper wrapper in _plugins)
                wrapper.Unload(); // Unload the wrapper
            SaveConfig();
            Logging.Log("Plugins unloaded!");
        }
        #endregion

        #region Public Functions
        public PluginWrapper GetWrapper(Plugin plugin)
        {
            //return Plugins.First(a => a.Plugins.ContainsValue(plugin));
            return Plugins.First(a => a.PluginClass == plugin);
        }
        #endregion

        #region Private Functions
        private void LoadConfig()
        {
            if (UniConfig.CreatedNew)
            {
                PluginConfiguration.AutoUpdate = false;
                PluginConfiguration.ContinueOnError = true;
                PluginConfiguration.NotifyUpdates = true;

                JSONConfig.Document.Add("AutoUpdate", (PluginConfiguration.AutoUpdate ? "true" : "false"));
                JSONConfig.Document.Add("ContinueOnError", (PluginConfiguration.ContinueOnError ? "true" : "false"));
                JSONConfig.Document.Add("NotifyUpdates", (PluginConfiguration.NotifyUpdates ? "true" : "false"));
            }
            else
            {
                PluginConfiguration.AutoUpdate = ((string)JSONConfig.Document["AutoUpdate"] == "true");
                PluginConfiguration.ContinueOnError = ((string)JSONConfig.Document["ContinueOnError"] == "true");
                PluginConfiguration.NotifyUpdates = ((string)JSONConfig.Document["NotifyUpdates"] == "true");
            }
        }

        private void SaveConfig()
        {
            UniConfig.Save();
        }
        #endregion
    }
}
