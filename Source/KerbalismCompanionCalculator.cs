using KERBALISM;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;
using UnityEngine.UI;

namespace KerbalismCompanionCalculator
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class KerbalismCompanionCalculator : MonoBehaviour
    {
        private ApplicationLauncher toolbar = ApplicationLauncher.Instance;
        private ApplicationLauncherButton toolbarButton;
        private Texture buttontexture = GameDatabase.Instance.GetTexture("KerbalismCompanionCalculator/Icon/AppButton_inactive", false);
        private bool isAppLauncherInit = false;

        private GameObject K3InfoWindow;
        private UI.ToolbarWindow kalkulatorUI;

        public bool detectRelays = true;
        public bool detectAntennas = true;

        public static KerbalismCompanionCalculator Instance { get; private set; }

        void Start()
        {
            Instance = this;

            K3InfoWindow = Instantiate(AssetLoader.K3InfoWindow, MainCanvasUtil.MainCanvasRect.transform);
            kalkulatorUI = K3InfoWindow.AddComponent<UI.ToolbarWindow>();
            K3InfoWindow.SetActive(false);   

            GameEvents.onEditorLoad.Add(onEditorLoad);
            GameEvents.onEditorShipModified.Add(onEditorShipModified);
            GameEvents.onEditorUndo.Add(onEditorShipModified);

            ReloadVessel();

            if (!isAppLauncherInit)     
                GameEvents.onGUIApplicationLauncherReady.Add(createAppLauncher);
        }

        private void onEditorLoad(ShipConstruct shiploaded, CraftBrowserDialog.LoadType loadType) => ReloadVessel();
        private void onEditorShipModified(ShipConstruct ship) => ReloadVessel();

        public void ReloadVessel()
        {
            List<ModuleDataTransmitter> moduleList = new List<ModuleDataTransmitter>();
            foreach (Part part in EditorLogic.fetch.ship.parts)
            {
                if (part.HasModuleImplementing<ModuleDataTransmitter>() && !part.HasModuleImplementing<ModuleCommand>())
                {
                    foreach (ModuleDataTransmitter module in part.Modules.GetModules<ModuleDataTransmitter>())
                    {
                        if ((module.antennaType == AntennaType.DIRECT || module.antennaType == AntennaType.INTERNAL) && detectAntennas)
                            moduleList.Add(module);
                        else if (module.antennaType == AntennaType.RELAY && detectRelays)
                            moduleList.Add(module);

                        PlannerController plannerController = part.Modules.GetModule<PlannerController>();

                        if (!plannerController.considered)
                        {
                            moduleList.RemoveAt(moduleList.Count - 1);
                        }
                    }
                }
                kalkulatorUI.moduleList = moduleList;
            }
        }

        public void createAppLauncher()
        {
            toolbarButton = toolbar.AddModApplication(onTrue: ShowGUI, onFalse: HideGUI, onHover: null, onHoverOut: null, onEnable: null, onDisable: Destroy, visibleInScenes: ApplicationLauncher.AppScenes.VAB, texture: buttontexture);
        }

        public void ShowGUI()
        {
            kalkulatorUI.rect.position = toolbarButton.GetAnchorUL();
            kalkulatorUI.rect.position = new Vector3(kalkulatorUI.rect.position.x, kalkulatorUI.rect.position.y + ((kalkulatorUI.rect.rect.height / 2) * GameSettings.UI_SCALE));
            K3InfoWindow.SetActive(true);
            toolbarButton.SetTexture(GameDatabase.Instance.GetTexture("KerbalismCompanionCalculator/Icon/AppButton_active", false));
        }

        public void HideGUI()
        {
            K3InfoWindow.SetActive(false);
            toolbarButton.SetTexture(GameDatabase.Instance.GetTexture("KerbalismCompanionCalculator/Icon/AppButton_inactive", false));
        }

        public void Destroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(createAppLauncher);
            isAppLauncherInit = false;
            ApplicationLauncher.Instance.RemoveModApplication(toolbarButton);
            Destroy(K3InfoWindow);
        }
    }
}


