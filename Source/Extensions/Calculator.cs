using KERBALISM;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace KerbalismCompanionCalculator
{
    public static class AntennaCalculator
    {
        /// <summary>
        /// Returns Vessel Power from all Antennas
        /// </summary>
        /// <param name="moduleList"></param>
        /// <returns>double</returns>
        public static double getVesselPower(List<ModuleDataTransmitter> moduleList)
        {
            double vesselpower = 0.0;
            double combinedvesselpower = 0.0;
            double highestvesselpower = 0.0;
            float rangeModifier = HighLogic.CurrentGame.Parameters.CustomParams<CommNet.CommNetParams>().rangeModifier;

            if (moduleList.Count == 1)
            {
                foreach(ModuleDataTransmitter module in moduleList)
                    vesselpower = module.antennaPower * rangeModifier;
                
            }
            else if (moduleList.Count > 1)
            {
                foreach(ModuleDataTransmitter module in moduleList)
                {
                    combinedvesselpower += module.antennaPower * rangeModifier;
                    if (module.antennaPower * rangeModifier > highestvesselpower) 
                        highestvesselpower = module.antennaPower * rangeModifier;
                    
                }
                vesselpower = highestvesselpower * Math.Pow(combinedvesselpower / highestvesselpower, getExponent(moduleList));
            }
            return vesselpower;
        }

        /// <summary>
        /// Returns Exponent for Combined Vessel Power Calculation
        /// </summary>
        /// <param name="moduleList"></param>
        /// <returns>double</returns>
        public static double getExponent(List<ModuleDataTransmitter> moduleList)
        {
            double Pn = 0.0;
            double Cn = 0.0;

            foreach (ModuleDataTransmitter module in moduleList)
            {
                Pn += module.antennaPower * module.antennaCombinableExponent;
                Cn += module.antennaPower;
            }
            return (Pn / Cn);
        }

        /// <summary>
        /// Returns Max Range of Connection
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="DSNPower"></param>
        /// <returns></returns>
        public static double getRange(List<ModuleDataTransmitter> moduleList, double DSNPower)
        {
            return (Math.Sqrt(DSNPower * getVesselPower(moduleList)));

        }

        /// <summary>
        /// Returns relative distance between Link Points (Relative Distance)
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="targetDistance"></param>
        /// <param name="DSNPower"></param>
        /// <returns>double</returns>
        public static double getRelativeDistance(List<ModuleDataTransmitter> moduleList, double targetDistance, double DSNPower)
        {
            if (getRange(moduleList, DSNPower) < targetDistance)
                return 0;
            else
                return Math.Abs(1d - Math.Abs(targetDistance / getRange(moduleList, DSNPower)));
        }

        /// <summary>
        /// Returns Signal Strength
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="targetDistance"></param>
        /// <param name="DSNPower"></param>
        /// <returns>double</returns>
        public static double getSignalStrenght(List<ModuleDataTransmitter> moduleList, double targetDistance, double DSNPower)
        {
            double relativeDistance = getRelativeDistance(moduleList, targetDistance, DSNPower);
            double signalstrenght = (3 - 2 * relativeDistance) * relativeDistance * relativeDistance;
            if (signalstrenght < 0)
                return 0;
            else
                return signalstrenght;
        }

        /// <summary>
        /// Returns Max Data Rate in Mb/s
        /// </summary>
        /// <param name="moduleList"></param>
        /// <returns>double</returns>
        public static double getKerbalismDataRate(List<ModuleDataTransmitter> moduleList)
        {
            double combinedRate = 1;
            foreach (ModuleDataTransmitter module in moduleList)
            {
                combinedRate *= module.DataRate * PreferencesScience.Instance.transmitFactor;
            }

            if (moduleList.Count == 0)
            {
                return 0;
            }
            else
            {
                return Math.Pow(combinedRate, (double)1d / moduleList.Count);
            }
        
        }

        /// <summary>
        /// Returns Data Rate over Distance in Mb/s
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="targetDistance"></param>
        /// <param name="DSNPower"></param>
        /// <returns>double</returns>
        public static double getKerbalismDataRateOverDistance(List<ModuleDataTransmitter> moduleList, double targetDistance, double DSNPower)
        {
            return getKerbalismDataRate(moduleList) * Math.Pow(getSignalStrenght(moduleList, targetDistance, DSNPower), Sim.DataRateDampingExponent);
            
        }

        /// <summary>
        /// Returns Data Rate over Distance in Kb/s
        /// </summary>
        /// <param name="moduleList"></param>
        /// <param name="targetDistance"></param>
        /// <param name="DSNPower"></param>
        /// <returns>double</returns>
        public static double getTrueKBKerbalsimDataRateOverDistance(List<ModuleDataTransmitter> moduleList, double targetDistance, double DSNPower)
        {
            return 1000 * (getKerbalismDataRateOverDistance(moduleList, targetDistance, DSNPower));
        }
    }
}
