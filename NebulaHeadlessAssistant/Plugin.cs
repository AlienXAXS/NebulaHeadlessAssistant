using BepInEx;
using System.Reflection;

[assembly: ObfuscateAssembly(assemblyIsPrivate: true, StripAfterObfuscation = true)]

namespace NebulaHeadlessAssistant
{
    [BepInPlugin(PluginInfo.PLUGIN_ID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("dsp.nebula-multiplayer")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Log.Init(new BepInExLogger(Logger));
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loading!");

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dsp.nebula-multiplayer"))
            {
                Log.LogInfo("Stage 2");
                NebulaHeadlessAssistant.Instance.OnAwake();
            }
            else
            {
                Log.LogInfo(">> Cannot load, nebula mod not found");
            }
        }
    }
}