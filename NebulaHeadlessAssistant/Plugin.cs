using BepInEx;
using System.Reflection;

[assembly: ObfuscateAssembly(assemblyIsPrivate: true, StripAfterObfuscation = true)]

namespace NebulaHeadlessAssistant
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("dsp.nebula-multiplayer")]
    public class Plugin : BaseUnityPlugin
    {
        [Obfuscation(Exclude = true)]
        private void Awake()
        {
            Log.Init(new BepInExLogger(Logger));

            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loading!");

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dsp.nebula-multiplayer"))
            {
                NebulaHeadlessAssistant.Instance.OnAwake();
            }
            else
            {
                Log.LogInfo(">> Cannot load, nebula mod not found");
            }
        }
    }
}