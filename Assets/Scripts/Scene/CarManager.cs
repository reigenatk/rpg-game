using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
   
    [SerializeField] float spawnRateOfCars; // how many seconds between each car spawn
    [SerializeField] List<GameObject> carPrefabsToSpawn;
    [SerializeField] List<int> carSpawnRarity;

    [SerializeField] List<CarRoute> routes;
    [SerializeField] List<int> routeSpawnRarity;

    public bool enableCars;

    [System.Serializable]
    public class CarRoute
    {
        public Vector3 startLocation;
        public Vector3 endLocation;
        public SceneName sceneToPlayIn;
        public string animationBoolName; // what animation bool should we trigger to play an animation? is it Drive left or right? (maybe ill add up down if i have time :P)
    }
    // Start is called before the first frame update
    void Start()
    {
        // we will use invoke repeating to repeatitively call a function that spawns cars.
        InvokeRepeating("SpawnCar", 2.0f, spawnRateOfCars);
    }

    // Update is called once per frame
    void SpawnCar()
    {
        if (enableCars == false) return;
        Debug.Log("Trying to spawn a car...");
        int carIdx = getRandomIdx(carSpawnRarity);
        List<CarRoute> workingRoutes = new List<CarRoute>();
        List<int> workingRouteRarity = new List<int>();
        for (int i = 0; i < routes.Count; i++)
        {
            
            CarRoute r = routes[i];
            int rarityOfThisRoute = routeSpawnRarity[i];
            if (r.sceneToPlayIn == LevelLoader.Instance.getCurScene())
            {
                workingRoutes.Add(r);
                workingRouteRarity.Add(rarityOfThisRoute);
            }
            
        }
        if (workingRoutes.Count == 0)
        {
            // this will happen when we aren't on a map that spawns cars. Just return
            return;
        }
        int routeIdx = getRandomIdx(workingRouteRarity);

        GameObject newCar = Instantiate(carPrefabsToSpawn[carIdx], workingRoutes[routeIdx].startLocation, Quaternion.identity, this.transform);
        newCar.GetComponent<Car>().Drive(workingRoutes[routeIdx].startLocation, workingRoutes[routeIdx].endLocation, workingRoutes[routeIdx].animationBoolName);
    }

    // returns an idx
    int getRandomIdx(List<int> rarity)
    {
        int totalRaritySum = 0;
        foreach (int x in rarity)
        {
            totalRaritySum += x;
        }

        int hitvalue = Random.Range(0, totalRaritySum + 1);
        int prev = 0;
        int cur = 0;
        for (int i = 0; i < rarity.Count; i++)
        {
            cur += rarity[i];
            if (hitvalue <= cur && hitvalue >= prev)
            {
                return i;
            }
            prev = cur;
        }
        return -1; // should be impososible
    }
}
