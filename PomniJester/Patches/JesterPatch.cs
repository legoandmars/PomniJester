using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace PomniJester.Patches
{
    [HarmonyPatch(typeof(JesterAI))]
    internal class JesterPatch
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        public static void JesterStart(JesterAI __instance)
        {

            if (Plugin.JesterTexture == null || Plugin.JesterClip == null || Plugin.JesterHat == null || Plugin.JesterPopClip == null)
            {
                Plugin.Logger.LogWarning("Pomni Jester replacement assets null, returning");
                return;
            }

            var hatParent = __instance.transform.Find("MeshContainer/AnimContainer/metarig/BoxContainer/spine.004/spine.005/spine.006/UpperJaw");
            if (hatParent == null)
            {
                Plugin.Logger.LogWarning("Could not find jester hat parent, returning");
                return;
            }

            Material? matReference = null;
            // swap skin
            // matReference is needed because HDRP/Lit shaders exported through assetbundles aren't *quite* right
            // Other shaders (such as custom & unlit shaders) SOMETIMES work, but we'll need an HDRP/Lit reference to use the shader in this case
            var materials = __instance.GetComponentsInChildren<Renderer>(true).SelectMany(x => x.sharedMaterials).ToList();
            foreach (var material in materials)
            {
                if (material.name.StartsWith("JesterTex"))
                {
                    // replace
                    material.mainTexture = Plugin.JesterTexture;
                    matReference = material;
                }
            }

            // swap audio
            __instance.popGoesTheWeaselTheme = Plugin.JesterClip;
            __instance.popUpSFX = Plugin.JesterPopClip;

            var hat = UnityEngine.Object.Instantiate(Plugin.JesterHat);
            hat.transform.SetParent(hatParent);
            hat.transform.localPosition = Vector3.zero;
            hat.transform.localRotation = Quaternion.identity;
            hat.transform.localScale = Vector3.one;

            if (matReference == null) return;
            foreach (var renderer in hat.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial.shader = matReference.shader;
            }
        }
    }
}
