using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : BlockManager {
    [SerializeField] GameObject industrialCityBlockPrefab;
    [SerializeField] GameObject residentialCityBlockPrefab;
    [SerializeField] GameObject citizenPrefab;
    [SerializeField] GameObject citizenContainer;

    private const float blockChosenTimeThreshold = 0.1f; // Seconds between spawning a family
    private const int familyMin = 2;
    private const int familyMax = 5;

    private List<Coordinate> unoccupiedIndustrialBlocks;
    private List<Coordinate> unoccupiedResidentialBlocks;
    private List<Coordinate> occupiedIndustrialBlocks;
    private List<Coordinate> occupiedResidentialBlocks;

    private float timeSinceLastBlockChosen;

    protected override void Start() {
        unoccupiedIndustrialBlocks = new List<Coordinate>();
        unoccupiedResidentialBlocks = new List<Coordinate>();

        base.Start();

        occupiedIndustrialBlocks = new List<Coordinate>();
        occupiedResidentialBlocks = new List<Coordinate>();

        timeSinceLastBlockChosen = blockChosenTimeThreshold;
    }

    void Update() {
        if (unoccupiedResidentialBlocks.Count > 0) {
            if (timeSinceLastBlockChosen > blockChosenTimeThreshold) {
                AddFamily();
                timeSinceLastBlockChosen = 0;
            }
        }

        // int m = -1;
        // for (int i = 0; i < occupiedResidentialBlocks.Count; i++) {
        //  int j = GetBlock(occupiedResidentialBlocks[i]).GetComponent<ResidentialCityBlock>().residents.Count;
        //  if (j > m) m = j;
        // }
        // Debug.Log(m);

        // for (int i = 0; i < 400; i++) {
        //  // Vector3 d = GenerateSpawnPos();
        //  // Vector3 d = GetRandomPosInCoord(new Coordinate(3, 5));
        //  Debug.DrawLine(d, d + Vector3.up, Color.red);
        // }

        timeSinceLastBlockChosen += Time.deltaTime;
    }

    public override void BlockClicked(GameObject go) {
        // Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
    }

    public override void BlockHovered(GameObject go) {
        // Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
    }

    public override Path GetManhattanPath(Vector3 fromPos, Coordinate destCoord) {
        Vector3 toPos = GetLocalCenterPosForCoord(destCoord);
        Vector3 topLeftOfStartCoord = GetLocalPosForCoord(GetCoordForLocalPos(fromPos));
        Vector3 topLeftOfDestCoord = GetLocalPosForCoord(destCoord);
        // Debug.Log("Top left dest coord: " + topLeftOfDestCoord + " for coord " + destCoord);

        Vector3 road = GetClosestRoad(fromPos, topLeftOfStartCoord);
        Vector3 intersection = GetClosestIntersectionFromRoad(road, topLeftOfStartCoord);

        Vector3 endRoad = GetClosestRoad(toPos, topLeftOfDestCoord);
        Vector3 endIntersection = GetClosestIntersectionFromRoad(endRoad, topLeftOfDestCoord);

        // Debug.Log("Starting intersection: " + intersection);
        // Debug.Log("Ending intersection  : " + endIntersection);

        Vector3 delta = (endIntersection - intersection);
        // Debug.Log("Delta                : " + delta);
        Vector3 xResolve = intersection + Vector3.right * delta.x;

        // Vector3 ddd = Util.topLeftScreenToWorldPoint();
        // Debug.DrawLine(ddd + fromPos, ddd + road, Color.cyan);
        // Debug.DrawLine(ddd + road, ddd + intersection, Color.red);
        // Debug.DrawLine(ddd + toPos, ddd + endRoad, Color.cyan);
        // Debug.DrawLine(ddd + endIntersection, ddd + endRoad, Color.red);

        // Debug.DrawLine(intersection, xResolve, Color.white);
        // Debug.DrawLine(xResolve, xResolve + Vector3.up * delta.y, Color.white);

        Path p = new Path(new List<PathSegment>() {
            new PathSegment(fromPos, road, 0.5f,            "Nearest road        "),
            new PathSegment(road, intersection, 0.5f,       "Nearest intersection"),
            new PathSegment(intersection, xResolve,         "X delta             "),
            new PathSegment(xResolve, endIntersection,      "Y delta             "),
            new PathSegment(endIntersection, toPos, 0.5f,   "Destination         "),
        });

        // Debug.Log(p);

        return p;
    }

    public void UpdateSubwayPaths(List<Path> subwayPaths) {
        Debug.Log("Got subway paths:");
        subwayPaths.ForEach(p => {
            Debug.Log(p);
        });
    }

    protected override GameObject GetBlockPrefab(int x, int y) {
        if (y < data.numBlocks / 2) {
            unoccupiedIndustrialBlocks.Add(new Coordinate(x, y));
            return industrialCityBlockPrefab;
        } else {
            unoccupiedResidentialBlocks.Add(new Coordinate(x, y));
            return residentialCityBlockPrefab;
        }
    }

    private void AddFamily() {
        int r = Random.Range(0, unoccupiedResidentialBlocks.Count - 1);
        Coordinate houseCoord = unoccupiedResidentialBlocks[r];
        ResidentialCityBlock houseBlock = GetBlock(houseCoord).GetComponent<ResidentialCityBlock>();

        unoccupiedResidentialBlocks.Remove(houseCoord);
        occupiedResidentialBlocks.Add(houseCoord);

        int familySize = Random.Range(familyMin, familyMax);
        for (int i = 0; i < familySize; i++) {
            Citizen familyMember = AddCitizen(houseBlock);
            familyMember.SetSpawn(GenerateSpawnPos());
            houseBlock.AddResident(familyMember);
        }
    }

    private Citizen AddCitizen(ResidentialCityBlock home) {
        GameObject go = Instantiate(citizenPrefab, citizenContainer.transform);
        Citizen citizen = go.GetComponent<Citizen>();

        citizen.SetHome(home);
        citizen.SetPathPlanner(this);

        Coordinate workCoord;
        // If no work buildings exist or 25% chance
        if (occupiedIndustrialBlocks.Count == 0 || Random.Range(0, 100) < 25) {
            // Occupy new work building
            int r = Random.Range(0, unoccupiedIndustrialBlocks.Count - 1);
            workCoord = unoccupiedIndustrialBlocks[r];

            unoccupiedIndustrialBlocks.RemoveAt(r);
            occupiedIndustrialBlocks.Add(workCoord);
        } else {
            // Occupy existing work building
            int r = Random.Range(0, occupiedIndustrialBlocks.Count - 1);
            workCoord = occupiedIndustrialBlocks[r];
        }

        IndustrialCityBlock workBlock = GetBlock(workCoord).GetComponent<IndustrialCityBlock>();
        citizen.SetWork(workBlock);

        return citizen;
    }

    private Vector3 GenerateSpawnPos() {
        bool left = Random.Range(0, 2) == 0;
        int y = Random.Range(-1, -data.numBlocks);

        if (left) {
            return new Vector3(-data.halfBlockSizeUnit, y * data.totalBlockSizeUnit, 0);
        } else {
            return new Vector3(data.totalSizeUnit + data.halfBlockSizeUnit, y * data.totalBlockSizeUnit, 0);
        }
    }

    private Vector3 GetClosestRoad(Vector3 startLocalPos, Vector3 topLeftOfStartCoord) {
        Vector3 vup = new Vector3(startLocalPos.x, topLeftOfStartCoord.y, 0);
        Vector3 vdown = new Vector3(startLocalPos.x, topLeftOfStartCoord.y - data.totalBlockSizeUnit, 0);
        Vector3 vleft = new Vector3(topLeftOfStartCoord.x, startLocalPos.y, 0);
        Vector3 vright = new Vector3(topLeftOfStartCoord.x + data.totalBlockSizeUnit, startLocalPos.y, 0);

        return Util.getClosestVector3(startLocalPos, vup, vdown, vleft, vright);
    }

    private Vector3 GetClosestIntersectionFromRoad(Vector3 startLocalPos, Vector3 topLeftOfStartCoord) {
        Vector3 iTopLeft = topLeftOfStartCoord;
        Vector3 iTopRight = topLeftOfStartCoord + Vector3.right * data.totalBlockSizeUnit;
        Vector3 iBottomLeft = topLeftOfStartCoord + Vector3.down * data.totalBlockSizeUnit;
        Vector3 iBottomRight = topLeftOfStartCoord + (Vector3.right + Vector3.down) * data.totalBlockSizeUnit;

        return Util.getClosestVector3(startLocalPos, iTopLeft, iTopRight, iBottomLeft, iBottomRight);
    }
}
