using System;
using System.Collections.Generic;
using System.Linq;

namespace Photon.Hive.Plugin.WebHooks
{
    public class PluginFactory : IPluginFactory
    {
        public IGamePlugin Create(IPluginHost gameHost, string pluginName, Dictionary<string, string> config, out string errorMsg)
        {
            System.Reflection.Assembly asm = typeof(PluginFactory).Assembly;
            Type type = asm.GetExportedTypes().FirstOrDefault(t => t.GetInterfaces().Contains(typeof(IGamePlugin)) && t.Name.Equals(pluginName, StringComparison.CurrentCultureIgnoreCase));

            if (type == null)
            {
                errorMsg = $"The plugin [{pluginName}] not found";
                return null;
            }

            var plugin = (IGamePlugin)Activator.CreateInstance(type);
            //var plugin = new WebHooksPlugin();


            if (plugin.SetupInstance(gameHost, config, out errorMsg))
            {
                return plugin;
            }
            return null;
        }
    }
}
