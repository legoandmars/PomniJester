using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace PomniJester
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger;

        public static Texture JesterTexture;
        public static GameObject JesterHat; // needs shader swapped out at runtime
        public static AudioClip JesterClip;
        public static AudioClip JesterPopClip;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            LoadAssets();
        }

        private void LoadAssets()
        {
            // find file named "jestervisuals" in the same folder as the plugin
            var bundlePath = Path.Join(Path.GetDirectoryName(this.Info.Location), "jestervisuals");
            var bundle = AssetBundle.LoadFromFile(bundlePath);
            var asset = bundle.LoadAsset<GameObject>("assets/prefabs/jestervisuals.prefab");

            // little hardcoded, didn't wanna set up a class
            JesterTexture = asset.transform.GetChild(0).GetComponent<Renderer>().sharedMaterial.mainTexture;
            JesterHat = asset.transform.GetChild(1).gameObject;
            JesterClip = asset.transform.GetChild(2).GetComponent<AudioSource>().clip;
            JesterPopClip = asset.transform.GetChild(3).GetComponent<AudioSource>().clip;
        }
    }
}