using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DungeonType
{
    Caverns, Halls, WindingHalls
}

public class DungeonManager : MonoBehaviour
{
    public GameObject[] gameItems, gameEnemies, gameRoundedEdges;
    public GameObject floorPrefab, wallPrefab, tileSpawnerPrefab, exitPrefab, stonePrefab;
    public bool useRoundedEdges;
    public DungeonType dungeonType;
    [HideInInspector] public float minX, maxX, minY, maxY;
    public int width, numStoneClumps, stoneClumpSize;
    public int inBounds, bufferArea;

    [Range(10, 1000)] public int totalFloorCount;
    [Range(0,100)] public int itemSpawnChance, enemySpawnChance, roomSpawn;
    
    Vector2 overlapSize = Vector2.one * 0.8f;
    LayerMask floorMask, wallMask;
    List<Vector2> stonePos;

    private void Start()
    {
        //switch (dungeonType)
        //{
        //    case DungeonType.Caverns:
        //        RandomWalker();
        //        break;
        //    case DungeonType.Halls:
        //        HallWalker();
        //        break;
        //    case DungeonType.WindingHalls:
        //        WindingWalker();
        //        break;
        //}
        CreateSand();
        //CreateStoneClumps();
        floorMask = LayerMask.GetMask("Floor");
        wallMask = LayerMask.GetMask("Wall");
    }


    private void Update()
    {
        if (Application.isEditor && Input.GetKeyDown(KeyCode.Backspace))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void CreateSand()
    {
        //for (int i = -width; i <= width; i++)
        //{
        //    for (int j = -width; j < width; j++)
        //    {
        //        GameObject goTile = Instantiate(floorPrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
        //        goTile.name = floorPrefab.name;
        //        goTile.transform.SetParent(GameObject.Find("Floor Tiles").transform);
        //    }
        //}

        for (int i = -inBounds; i <= inBounds; i++)
        {
            for (int j = -inBounds; j <= inBounds; j++)
            {
                //// is there a floor
                //Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(i, j), overlapSize, 0, floorMask);
                //if (hitFloor)
                //{
                //        Debug.Log("y");
                //        // check if against wall
                //        Collider2D hitTop = Physics2D.OverlapBox(new Vector2(i, j + 1), overlapSize, 0, wallMask);
                //        Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(i, j - 1), overlapSize, 0, wallMask);
                //        Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(i - 1, j), overlapSize, 0, wallMask);
                //        Collider2D hitRight = Physics2D.OverlapBox(new Vector2(i + 1, j), overlapSize, 0, wallMask);
                //    //SpawnRandomItem(hitFloor, hitTop, hitBottom, hitLeft, hitRight);
                //        // SpawnRandomEnemy(hitFloor, hitTop, hitBottom, hitLeft, hitRight);

                //}
                SpawnRandomEnemy(i, j);
            }
        }
        

    }

    void SpawnRandomEnemy(int i, int j)
    {

            // if open space, spawn enemy with chance
            int roll = Random.Range(1, 101);
            if (roll <= enemySpawnChance)
            {
                int itemIdx = Random.Range(0, gameEnemies.Length);
                GameObject goEnemy = Instantiate(gameEnemies[itemIdx], new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                goEnemy.name = gameEnemies[itemIdx].name;

                // make the items the child of each floor
                goEnemy.transform.SetParent(GameObject.Find("Enemies").transform);
            }
        
    }

    bool isInBounds(Vector2 v)
    {
        if (v.x >= -inBounds && v.x <= inBounds && v.y >= -inBounds && v.y <= inBounds)
        {
            return true;
        }
        return false;
    }

    void CreateStoneClumps()
    {
        for (int i = 0; i < numStoneClumps; i++)
        {
            List<Vector2> spawned = new List<Vector2>();
            Vector2 pos = new Vector2(Random.Range(-inBounds, inBounds), Random.Range(-inBounds, inBounds));
            while (spawned.Count != stoneClumpSize)
            {
                Vector3 randDir = RandomDirection();
                if (isInBounds(pos + new Vector2(randDir.x, randDir.y)))
                {
                    pos += new Vector2(randDir.x, randDir.y);
                    Collider2D hitPreexistingWall = Physics2D.OverlapBox(pos, overlapSize, 0, wallMask);
                    if (!hitPreexistingWall)
                    {
                        spawned.Add(pos);
                        GameObject goTile = Instantiate(wallPrefab, pos, Quaternion.identity) as GameObject;
                        goTile.name = wallPrefab.name;
                        goTile.transform.SetParent(GameObject.Find("Stone").transform);
                        
                    }

                }
            }
            foreach (Vector2 loc in spawned)
            {
                Debug.Log(loc);
                RoundedEdges((int)loc.x, (int)loc.y);
            }
        }
        
    }


    //void RandomWalker()
    //{
    // List<Vector3> floorList = new List<Vector3>();
    //    Vector3 curPos = Vector3.zero;

    //    floorList.Add(curPos);


    //    while (floorList.Count <= totalFloorCount)
    //    {
    //        curPos += RandomDirection();


    //        if (!inFloorList(curPos))
    //        {
    //            floorList.Add(curPos);
    //        }

    //    }


    //    StartCoroutine(DelayProgress());
    //}

    //void HallWalker()
    //{
    //    Vector3 curPos = Vector3.zero;

    //    floorList.Add(curPos);


    //    while (floorList.Count <= totalFloorCount)
    //    {
    //        Vector3 walkDir = RandomDirection();
    //        int walkLength = Random.Range(9, 18);

    //        curPos = RandomHike(curPos);

    //        SpawnRandomRoom(curPos);


    //    }

    //    StartCoroutine(DelayProgress());
    //}

    //// mix between roomwalker and hallway walker which always spawn a room at end of hike
    //void WindingWalker()
    //{
    //    Vector3 curPos = Vector3.zero;

    //    floorList.Add(curPos);


    //    while (floorList.Count <= totalFloorCount)
    //    {
    //        int roll = Random.Range(0, 100);
    //        curPos = RandomHike(curPos);
    //        if (roll < roomSpawn)
    //        {
    //            SpawnRandomRoom(curPos);
    //        }

    //    }


    //    StartCoroutine(DelayProgress());
    //}

    //Vector3 RandomHike(Vector3 myPos)
    //{
    //    Vector3 walkDir = RandomDirection();
    //    int walkLength = Random.Range(9, 18);
    //    for (int i = 0; i < walkLength; i++)
    //    {
    //        if (!inFloorList(myPos))
    //        {
    //            floorList.Add(myPos);
    //        }
    //        if (i != walkLength - 1)
    //        {
    //            myPos += walkDir;
    //        }
    //    }
    //    return myPos;
    //}


    //void SpawnRandomRoom(Vector3 myPos)
    //{
    //    // spawn a random room at the end of the hallway
    //    // let width be the size of the room in both directions, with us in the ceter
    //    int width = Random.Range(1, 5);
    //    int height = Random.Range(1, 5);
    //    for (int w = -width; w <= width; w++)
    //    {
    //        for (int h = -height; h < height; h++)
    //        {
    //            floorList.Add(myPos + new Vector3(w, h));
    //        }
    //    }
    //}

    //bool inFloorList(Vector3 myPos)
    //{
    //    // no duplicates
    //    for (int i = 0; i < floorList.Count; i++)
    //    {
    //        if (Vector3.Equals(myPos, floorList[i]))
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    Vector3 RandomDirection()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                return Vector3.up;
            case 2:
                return Vector3.right;
            case 3:
                return Vector3.down;
            case 4:
                return Vector3.left;
        }
        return Vector3.one;
    }

    //IEnumerator DelayProgress()
    //{
    //    // initialize tileSpawners at all these locations
    //    for (int i = 0; i < floorList.Count; i++)
    //    {
    //        GameObject goTile = Instantiate(tileSpawnerPrefab, floorList[i], Quaternion.identity) as GameObject;
    //        goTile.name = tileSpawnerPrefab.name;
    //        goTile.transform.SetParent(transform);
    //    }

    //    // wait until there are no more tile spawners
    //    while (FindObjectsOfType<TileSpawner>().Length > 0)
    //    {
    //        yield return null;
    //    }

    //    CreateExitDoorway();

    //    // loop over entire map
    //    for (int i = (int) minX - 2; i < (int) maxX + 2; i++)
    //    {
    //        for (int j = (int) minY - 2; j < (int) maxY + 2; j++)
    //        {
    //            //if floor

    //            Collider2D hitFloor = Physics2D.OverlapBox(new Vector2(i, j), overlapSize, 0, floorMask);
    //            if (hitFloor)
    //            {
    //                // if it isn't the exit doorway
    //                if (!(Vector2.Equals(hitFloor.transform.position, floorList[floorList.Count - 1])))
    //                {
    //                    // check if against wall
    //                    Collider2D hitTop = Physics2D.OverlapBox(new Vector2(i, j+1), overlapSize, 0, wallMask);
    //                    Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(i, j - 1), overlapSize, 0, wallMask);
    //                    Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(i-1, j), overlapSize, 0, wallMask);
    //                    Collider2D hitRight = Physics2D.OverlapBox(new Vector2(i+1, j), overlapSize, 0, wallMask);
    //                    SpawnRandomItem(hitFloor, hitTop, hitBottom, hitLeft, hitRight);
    //                    SpawnRandomEnemy(hitFloor, hitTop, hitBottom, hitLeft, hitRight);
    //                }
    //            }

    //            RoundedEdges(i, j);
    //        }
    //    }
    //}

    void RoundedEdges(int i, int j)
    {
        if (useRoundedEdges)
        {
      
            Collider2D hitWall = Physics2D.OverlapBox(new Vector2(i, j), overlapSize, 0, wallMask);
            // add rounded edges to each side if no wall

            if (!hitWall)
            {
                Collider2D hitTop = Physics2D.OverlapBox(new Vector2(i, j + 1), overlapSize, 0, wallMask);
                Collider2D hitBottom = Physics2D.OverlapBox(new Vector2(i, j - 1), overlapSize, 0, wallMask);
                Collider2D hitLeft = Physics2D.OverlapBox(new Vector2(i - 1, j), overlapSize, 0, wallMask);
                Collider2D hitRight = Physics2D.OverlapBox(new Vector2(i + 1, j), overlapSize, 0, wallMask);
                int bitVal = 0;
                if (!hitTop)
                {
                    bitVal += 1;
                }
                if (!hitRight)
                {
                    bitVal += 2;
                }
                if (!hitBottom)
                {
                    bitVal += 4;
                }
                if (!hitLeft)
                {
                    bitVal += 8;
                }
                if (bitVal > 0)
                {
                    GameObject round_edge = Instantiate(gameRoundedEdges[bitVal], new Vector2(i, j), Quaternion.identity) as GameObject;
                    round_edge.name = gameRoundedEdges[bitVal].name;
                    round_edge.transform.SetParent(GameObject.Find("Rounded Edges").transform);
                }
            }
        }
    }

    void SpawnRandomItem(Collider2D hitFloor, Collider2D hitTop, Collider2D hitBottom, Collider2D hitLeft, Collider2D hitRight)
    {
        // if against a wall
        // the last two conditions are to avoid spawning in hallways
        if (hitTop || hitBottom || hitLeft || hitRight && !(hitTop && hitBottom) && !(hitLeft && hitRight))
        {
            // spawn random item according to chance
            int roll = Random.Range(1, 101);
            if (roll <= itemSpawnChance)
            {
                int itemIdx = Random.Range(0, gameItems.Length);
                GameObject goItem = Instantiate(gameItems[itemIdx], hitFloor.transform.position, Quaternion.identity) as GameObject;
                goItem.name = gameItems[itemIdx].name;

                // make the items the child of each floor
                goItem.transform.SetParent(hitFloor.transform);
            }
        }
    }

    //void SpawnRandomEnemy(Collider2D hitFloor, Collider2D hitTop, Collider2D hitBottom, Collider2D hitLeft, Collider2D hitRight)
    //{
    //    if (!hitTop && !hitRight && !hitBottom && !hitLeft)
    //    {
    //        // if open space, spawn enemy with chance
    //        int roll = Random.Range(1, 101);
    //        if (roll <= enemySpawnChance)
    //        {
    //            int itemIdx = Random.Range(0, gameEnemies.Length);
    //            GameObject goEnemy = Instantiate(gameEnemies[itemIdx], hitFloor.transform.position, Quaternion.identity) as GameObject;
    //            goEnemy.name = gameEnemies[itemIdx].name;

    //            // make the items the child of each floor
    //            goEnemy.transform.SetParent(GameObject.Find("Enemies").transform);
    //        }
    //    }
    //}

    //void CreateExitDoorway()
    //{
    //    Vector3 doorPos = floorList[floorList.Count - 1];
    //    GameObject goExitDoor = Instantiate(exitPrefab, doorPos, Quaternion.identity) as GameObject;
    //    goExitDoor.name = exitPrefab.name;
    //    goExitDoor.transform.SetParent(transform);
    //}
}
