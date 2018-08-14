using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CityManager : BlockManager {
    [SerializeField] GameObject industrialCityBlockPrefab;
    [SerializeField] GameObject residentialCityBlockPrefab;
    [SerializeField] GameObject citizenPrefab;
    [SerializeField] GameObject citizenContainer;
    [SerializeField] CityData cityData;

    private List<Coordinate> unoccupiedIndustrialBlocks;
    private List<Coordinate> unoccupiedResidentialBlocks;
    private List<Coordinate> occupiedIndustrialBlocks;
    private List<Coordinate> occupiedResidentialBlocks;

    private float timeSinceLastBlockChosen;

    private Coordinate[,] closestSubway;
    private bool[,] visited;
    private Dictionary<Coordinate, Path> subwayPaths;

    protected override void Start() {
        unoccupiedIndustrialBlocks = new List<Coordinate>();
        unoccupiedResidentialBlocks = new List<Coordinate>();

        base.Start();

        occupiedIndustrialBlocks = new List<Coordinate>();
        occupiedResidentialBlocks = new List<Coordinate>();

        timeSinceLastBlockChosen = cityData.famiilySpawnTime;

        ResetSubwayGrid();
    }

    void Update() {
        if (unoccupiedResidentialBlocks.Count > 0) {
            if (timeSinceLastBlockChosen > cityData.famiilySpawnTime) {
                AddFamily();
                timeSinceLastBlockChosen = 0;
            }
        }

        timeSinceLastBlockChosen += Time.deltaTime;
    }

    public override void BlockClicked(GameObject go) {
        // Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
    }

    public override void BlockHovered(GameObject go) {
        // Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
    }

    private Path mPath(Vector3 fromPos, Coordinate destCoord) {
        Vector3 toPos = GetLocalCenterPosForCoord(destCoord);
        Vector3 topLeftOfStartCoord = GetLocalPosForCoord(GetCoordForLocalPos(fromPos));
        Vector3 topLeftOfDestCoord = GetLocalPosForCoord(destCoord);

        Vector3 road = GetClosestRoad(fromPos, topLeftOfStartCoord);
        Vector3 intersection = GetClosestIntersectionFromRoad(road, topLeftOfStartCoord);

        Vector3 endRoad = GetClosestRoad(toPos, topLeftOfDestCoord);
        Vector3 endIntersection = GetClosestIntersectionFromRoad(endRoad, topLeftOfDestCoord);

        Vector3 delta = (endIntersection - intersection);
        Vector3 xResolve = intersection + Vector3.right * delta.x;

        // Vector3 ddd = Util.topLeftScreenToWorldPoint();
        // Debug.DrawLine(ddd + fromPos, ddd + road, Color.cyan);
        // Debug.DrawLine(ddd + road, ddd + intersection, Color.red);
        // Debug.DrawLine(ddd + toPos, ddd + endRoad, Color.cyan);
        // Debug.DrawLine(ddd + endIntersection, ddd + endRoad, Color.red);

        // Debug.DrawLine(intersection, xResolve, Color.white);
        // Debug.DrawLine(xResolve, xResolve + Vector3.up * delta.y, Color.white);

        Path p = new Path(new List<PathSegment>() {
            new PathSegment(fromPos, road, Vector3.Distance(fromPos, road) / data.PPU, "To road"),
            new PathSegment(road, intersection, Vector3.Distance(road, intersection) / data.PPU, "To intersection"),
            new PathSegment(intersection, xResolve, "x resolve"),
            new PathSegment(xResolve, endIntersection, "y resolve"),
            new PathSegment(endIntersection, toPos, Vector3.Distance(endIntersection, toPos) / data.PPU, "Destination"),
        });

        // Debug.Log(p);

        return p;
    }

    public override Path GetManhattanPath(Vector3 fromPos, Coordinate destCoord) {
        Path walkingPath = mPath(fromPos, destCoord);

        Coordinate fromCoord = GetCoordForLocalPos(fromPos);
        if (!IsValidCoord(fromCoord) || subwayPaths.Count == 0) return walkingPath;

        Coordinate closestSubwayCoord = closestSubway[fromCoord.x, fromCoord.y];
        Path subwayPath = mPath(fromPos, closestSubwayCoord);
        subwayPath += subwayPaths[closestSubwayCoord];
        subwayPath += mPath(subwayPaths[closestSubwayCoord].Ending(), destCoord);

        for (int i = 0; i < walkingPath.Count() - 1; i++) {
            Debug.DrawLine(walkingPath.DestinationAt(i) + Util.topLeftScreenToWorldPoint(), walkingPath.DestinationAt(i + 1) + Util.topLeftScreenToWorldPoint(), (i == 0) ? Color.red : Color.red);
        }
        for (int i = 0; i < subwayPath.Count() - 1; i++) {
            Debug.DrawLine(subwayPath.DestinationAt(i) + Util.topLeftScreenToWorldPoint(), subwayPath.DestinationAt(i + 1) + Util.topLeftScreenToWorldPoint(), (i == 0) ? Color.green : new Color(0, 1, 0, 1.0f));
        }

        if (walkingPath.weight < subwayPath.weight) {
            Debug.Log("Walking to work (" + walkingPath.weight + " < " + subwayPath.weight + ")");
            Debug.Log(walkingPath);
            Debug.Log(subwayPath);
            Debug.Break();
            return walkingPath;
        } else {
            Debug.Log("Taking the subway at " + closestSubwayCoord + " to work (" + subwayPath.weight + " < " + walkingPath.weight + ")");
            Debug.Break();
            return subwayPath;
        }
    }

    public void UpdateSubwayPaths(List<Path> subwayPaths) {
        // Debug.Log("Got subway paths:");
        // subwayPaths.ForEach(p => {
        //     Debug.Log(p);
        // });

        ResetSubwayGrid();

        Queue<Coordinate> q = new Queue<Coordinate>();
        subwayPaths.ForEach(p => {
            Coordinate c = GetCoordForLocalPos(p.Beginning());
            closestSubway[c.x, c.y] = c;
            this.subwayPaths[c] = p;
            visited[c.x, c.y] = true;
            q.Enqueue(c);
        });

        while (q.Count > 0) {
            Coordinate c = q.Dequeue();
            List<Coordinate> ns = new List<Coordinate>();

            if (c.y - 1 >= 0) { ns.Add(new Coordinate(c.x, c.y - 1)); }             // Up
            if (c.y + 1 < data.numBlocks) { ns.Add(new Coordinate(c.x, c.y + 1)); } // Down
            if (c.x - 1 >= 0) { ns.Add(new Coordinate(c.x - 1, c.y)); }             // Left
            if (c.x + 1 < data.numBlocks) { ns.Add(new Coordinate(c.x + 1, c.y)); } // Right

            ns.ForEach(nc => {
                if (!visited[nc.x, nc.y]) {
                    closestSubway[nc.x, nc.y] = closestSubway[c.x, c.y];
                    visited[nc.x, nc.y] = true;
                    q.Enqueue(nc);
                }
            });
        }
    }

    void OnDrawGizmosSelected() {
        if (!Application.isPlaying) return;

        for (int x = 0; x < data.numBlocks; x++) {
            for (int y = 0; y < data.numBlocks; y++) {
                Vector3 pos = GetLocalPosForCoord(new Coordinate(x, y));
                pos -= Vector3.left * Util.topLeftScreenToWorldPoint().x - Vector3.up * 23;
                Handles.Label(pos, closestSubway[x, y].ToString());
            }
        }
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

    private void ResetSubwayGrid() {
        closestSubway = new Coordinate[data.numBlocks, data.numBlocks];
        visited = new bool[data.numBlocks, data.numBlocks];
        subwayPaths = new Dictionary<Coordinate, Path>();
    }

    private void AddFamily() {
        int r = Random.Range(0, unoccupiedResidentialBlocks.Count - 1);
        Coordinate houseCoord = unoccupiedResidentialBlocks[r];
        ResidentialCityBlock houseBlock = GetBlock(houseCoord).GetComponent<ResidentialCityBlock>();

        unoccupiedResidentialBlocks.Remove(houseCoord);
        occupiedResidentialBlocks.Add(houseCoord);

        int familySize = Random.Range(cityData.familyMin, cityData.familyMax);
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
