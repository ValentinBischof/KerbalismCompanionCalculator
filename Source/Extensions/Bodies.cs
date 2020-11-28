using KERBALISM;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KerbalismCompanionCalculator
{
    public static class Bodies
    {

        public static bool ResearchBodiesAPI = false;

        /// <summary>
        /// Returns List of all Bodies orbiting Sun & Homebody
        /// </summary>
        /// <returns>CelestialBody</returns>
        public static List<CelestialBody> GetCelestialBodies()
        {
            List<CelestialBody> celestialBodies = new List<CelestialBody>();
            if (!ResearchBodiesAPI)
            {
                foreach (CelestialBody celestialBody in FlightGlobals.Bodies[0].orbitingBodies)
                {
                    if (celestialBody != FlightGlobals.GetHomeBody())
                        celestialBodies.Add(celestialBody);
                }

                foreach (CelestialBody celestialBody in FlightGlobals.Bodies[FlightGlobals.GetHomeBodyIndex()].orbitingBodies)
                    celestialBodies.Add(celestialBody);
                return celestialBodies;
            }
            else
            {
                Dictionary<CelestialBody, RBWrapper.CelestialBodyInfo> RBbodies = RBWrapper.RBactualAPI.CelestialBodies;
                foreach (CelestialBody celestialBody in FlightGlobals.Bodies[0].orbitingBodies)
                {
                    if (celestialBody != FlightGlobals.GetHomeBody())
                    { 
                        if (RBbodies.ContainsKey(celestialBody))
                        {
                            RBWrapper.CelestialBodyInfo RBbodyInfo = RBbodies[celestialBody];
                            if (RBbodyInfo.isResearched)
                                celestialBodies.Add(celestialBody);
                        }
                            
                    }      
                    
                }
                foreach (CelestialBody celestialBody in FlightGlobals.Bodies[FlightGlobals.GetHomeBodyIndex()].orbitingBodies)
                {
                    RBWrapper.RBactualAPI.CelestialBodies.TryGetValue(celestialBody, out RBWrapper.CelestialBodyInfo celestialBodyInfo);
                    if (celestialBodyInfo.isResearched)
                    {
                        Debug.Log(celestialBody.name);
                        celestialBodies.Add(celestialBody);
                    }     
                }
                    
                return celestialBodies;
            }
            
        }

        /// <summary>
        /// Returns Body by given Input
        /// </summary>
        /// <param name="name"></param>
        /// <returns>CelestialBody</returns>
        public static CelestialBody getBodybyName(string inputname)
        {
            CelestialBody search = FlightGlobals.GetHomeBody();
            foreach (CelestialBody body in FlightGlobals.Bodies)
            {
                if (body.name == inputname)
                {
                     search = body;
                }
            }

            return search;
        }

        /// <summary>
        /// Returns the Maximum distance between homebody & targetbody
        /// </summary>
        /// <param name="mainbody"></param>
        /// <param name="targetbody"></param>
        /// <returns></returns>
        public static double getMaxDistance(CelestialBody homeBody, CelestialBody targetBody)
        {
            if (FlightGlobals.Bodies[FlightGlobals.GetHomeBodyIndex()].orbitingBodies.Contains(targetBody))
                return targetBody.orbit.PeR +1;
            else
                return (homeBody.orbit.ApR + targetBody.orbit.ApR);
            

        }

        /// <summary>
        /// Returns the Minimum distance between homebody & tagetbody
        /// </summary>
        /// <param name="mainbody"></param>
        /// <param name="targetbody"></param>
        /// <returns></returns>
        public static double getMinDistance(CelestialBody homeBody, CelestialBody targetBody)
        {
            if (FlightGlobals.Bodies[FlightGlobals.GetHomeBodyIndex()].orbitingBodies.Contains(targetBody))
                return targetBody.orbit.PeR;

            if (homeBody.orbit.PeR > targetBody.orbit.PeR)
            {
                return (homeBody.orbit.PeR - targetBody.orbit.PeR);
            }
            else
            {
                return (targetBody.orbit.PeR - homeBody.orbit.PeR);
            }
        }

        public static bool VerifyResearchBodiesAPI()
        {
            AssemblyLoader.LoadedAssembly ResearchBodies = AssemblyLoader.loadedAssemblies.SingleOrDefault(a => a.dllName == "ResearchBodies");

            if (ResearchBodies != null)
            {
               RBWrapper.InitRBWrapper();

                if (RBWrapper.APIRBReady)
                    return true;
                else return false;
            }
            else return false;
        }
    }
}
