using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KerbalismCompanionCalculator.UI
{
    public class ToolbarWindow : MonoBehaviour
    {
        public RectTransform rect;

        protected Toggle EnablePlanner;
        protected Toggle DetectAntennas;
        protected Toggle DetectRelays;

        protected Dropdown CelestialBodies;

        protected Slider DistanceSlider;
        private Text DistanceSliderLabel;

        private GameObject DataRate;
        private Text CurrentDataRateLabel;
        private Text MaxDataRateLabel;
        private Text SignalStrengthLabel;
        private Text ActiveTransmittersLabel;

        private float CurrentSliderValue;

        public List<ModuleDataTransmitter> moduleList = new List<ModuleDataTransmitter>();
        public List<CelestialBody> celestialBodies = new List<CelestialBody>();

        public float DSNLevel;
        public double DSNPower;

        public void Awake()
        {
            rect = this.GetComponent<RectTransform>();

            EnablePlanner = GameObject.Find("EnablePlanner").GetComponent<Toggle>();
            DetectAntennas = GameObject.Find("DetectAntennas").GetComponent<Toggle>();
            DetectAntennas.onValueChanged.AddListener(delegate { onDetectAntennasValueChanged(DetectAntennas); });
            DetectRelays = GameObject.Find("DetectRelays").GetComponent<Toggle>();
            DetectRelays.onValueChanged.AddListener(delegate { onDetectRelaysValueChanged(DetectRelays); });

            CelestialBodies = GameObject.Find("PlannerPlanetDropdown").GetComponent<Dropdown>();
            CelestialBodies.onValueChanged.AddListener(delegate { onDropdownMenuValueChanged(CelestialBodies.value); });

            DistanceSlider = GameObject.Find("PlannerDistanceSlider").GetComponent<Slider>();
            DistanceSlider.onValueChanged.AddListener(delegate { onSliderValueChanged(DistanceSlider.value); });
            DistanceSliderLabel = GameObject.Find("CurrentDistance").GetComponent<Text>();

            DataRate = GameObject.Find("PlannerDataRate");
            CurrentDataRateLabel = DataRate.GetChild("CurrentDataRate").GetComponent<Text>();
            MaxDataRateLabel = DataRate.GetChild("MaxDataRate").GetComponent<Text>();
            SignalStrengthLabel = GameObject.Find("PlannerStrength").GetChild("signalstrength").GetComponent<Text>();
            ActiveTransmittersLabel = GameObject.Find("PlannerActiveTransmitters").GetChild("activeantennas").GetComponent<Text>();

            DSNLevel = ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation);
            DSNPower = GameVariables.Instance.GetDSNRange(DSNLevel);

            Bodies.ResearchBodiesAPI = Bodies.VerifyResearchBodiesAPI();
            celestialBodies = Bodies.GetCelestialBodies();

            FillDropdownMenu(celestialBodies);

            ChangeSliderValues(Convert.ToSingle(Bodies.getMinDistance(FlightGlobals.GetHomeBody(), celestialBodies[0])),
                               Convert.ToSingle(Bodies.getMaxDistance(FlightGlobals.GetHomeBody(), celestialBodies[0])));

        }

        private void onDetectRelaysValueChanged(Toggle detectRelays)
        {
            KerbalismCompanionCalculator.Instance.detectRelays = detectRelays.isOn;
            KerbalismCompanionCalculator.Instance.ReloadVessel();
        }

        private void onDetectAntennasValueChanged(Toggle detectAntennas)
        {
            KerbalismCompanionCalculator.Instance.detectAntennas = detectAntennas.isOn;
            KerbalismCompanionCalculator.Instance.ReloadVessel();
        }

        private void onDropdownMenuValueChanged(int value)
        {
            ChangeSliderValues(Convert.ToSingle(Bodies.getMinDistance(FlightGlobals.GetHomeBody(), celestialBodies[value])),
                               Convert.ToSingle(Bodies.getMaxDistance(FlightGlobals.GetHomeBody(), celestialBodies[value])));

        }

        private void FillDropdownMenu(List<CelestialBody> celestialBodies)
        {
            List<String> celestialBodiesNames = new List<String>();

            foreach (CelestialBody body in celestialBodies)
            {
                celestialBodiesNames.Add(body.name);
            }

            CelestialBodies.ClearOptions();
            CelestialBodies.AddOptions(celestialBodiesNames);
        }

        private void onSliderValueChanged(float value)
        {
            CurrentSliderValue = value;
            DistanceSliderLabel.text = $"{getDistance(value)}";
        }

        private void ChangeSliderValues(float min, float max)
        {
            float current = (min + max) / 2;

            DistanceSlider.minValue = min;
            DistanceSlider.maxValue = max;
            DistanceSlider.value = current;
            DistanceSliderLabel.text = $"{getDistance(current)}";
        }

        private string getDistance(float currentValue)
        {
            if (currentValue < 1000000000)
                return Convert.ToString(Math.Round(Math.Abs(currentValue) / 1000000, 1)) + "Mm";
            else
                return Convert.ToString(Math.Round(Math.Abs(currentValue) / 1000000000, 1)) + "Gm";
        }



        private void Update()
        {
            if (EnablePlanner.isOn)
            {
                CurrentDataRateLabel.text = $"{Math.Round(AntennaCalculator.getTrueKBKerbalsimDataRateOverDistance(moduleList, CurrentSliderValue, DSNPower),1)} kB/s";
                MaxDataRateLabel.text = $"{Math.Round(AntennaCalculator.getKerbalismDataRate(moduleList) * 1000, 1)} kB/s";
                SignalStrengthLabel.text = $"{Math.Round(AntennaCalculator.getSignalStrenght(moduleList, CurrentSliderValue, DSNPower) * 100, 2)} %";
                ActiveTransmittersLabel.text = $"{moduleList.Count()}";
            }
        }
    }
}
