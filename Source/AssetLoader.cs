using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace KerbalismCompanionCalculator
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class AssetLoader : MonoBehaviour
    {
        public static GameObject K3InfoWindow;

        public void Awake()
        {
            string path = Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "KerbalismCompanionCalculator", "Assets");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(path, "kerbalismcompanioncalculator"));

            if (assetBundle == null)
            {
                Debug.Log($"[KerbalismCompanionCalculator] { Path.Combine(path, "kerbalismcompanioncalculator")} could not be found or loaded.");
                return;
            }

            K3InfoWindow = assetBundle.LoadAsset<GameObject>("K3InfoWindow");
            Debug.Log($"[KerbalismCompanionCalculator] { Path.Combine(path, "kerbalismcompanioncalculator")} loaded.");
        }

    }
}
