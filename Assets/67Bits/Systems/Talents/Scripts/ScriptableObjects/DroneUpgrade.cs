using UnityEngine;

namespace Talents
{
    [CreateAssetMenu(fileName = "NewDroneUpgrade", menuName = "ScriptableObject/Upgrades/DroneUpgrade")]
    public class DroneUpgrade : UpgradeBase
    {
        //public DroneBase.DroneType droneType = DroneBase.DroneType.PewShot;
        //public GameObject dronePrefab = null;
        public override void ApplyUpgrade(Transform t)
        {
            //OrbitManager orbitManager = t.GetComponent<OrbitManager>();


            //if (orbitManager.PlayerHasDrone(droneType))
            //{
            //    orbitManager.LevelUpDrone(droneType);
            //}
            //else
            //{
            //    GameObject drone = Instantiate(dronePrefab, orbitManager.transform);
            //    orbitManager.AddOrbitingObject(drone);
            //    orbitManager.AddDroneType(droneType);
            //}
        }

        public override void RemoveUpgrade(Transform t)
        {

        }
    }
}