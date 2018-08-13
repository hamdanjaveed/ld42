using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlockManager : MonoBehaviour, IBlockHandler, IPathPlanner {
    [SerializeField] protected BlockData data;

    private GameObject[,] blocks;

    protected virtual void Start () {
        transform.position = Util.topLeftScreenToWorldPoint(); // Make the top left (0, 0)

        Vector3 halfBlockOffsetUnit = (Vector3.right + Vector3.down) * data.halfBlockSizeUnit;

        GameObject blockContainer = new GameObject();
        blockContainer.name = "Blocks";
        blockContainer.transform.parent = transform;
        blockContainer.transform.localPosition = Vector3.zero;

        blocks = new GameObject[data.numBlocks, data.numBlocks];
        for (int x = 0; x < data.numBlocks; x++) {
            for (int y = 0; y < data.numBlocks; y++) {
                GameObject block = Instantiate(GetBlockPrefab(x, y), blockContainer.transform) as GameObject;
                block.transform.localPosition = halfBlockOffsetUnit + Vector3.right * data.totalBlockSizeUnit * x + Vector3.down * data.totalBlockSizeUnit * y;
                block.name = "Block (" + x + ", " + y + ")";

                Block blockBlock = block.GetComponent<Block>();
                blockBlock.pos = new Coordinate(x, y);
                blockBlock.blockHandler = this;

                blocks[x, y] = block;
            }
        }
    }

    protected GameObject GetBlock(Coordinate c) {
        return blocks[c.x, c.y];
    }

    protected GameObject GetBlock(int x, int y) {
        return blocks[x, y];
    }

    protected Vector3 GetLocalPosForCoord(Coordinate c) {
        return new Vector3(c.x * data.totalBlockSizeUnit, -c.y * data.totalBlockSizeUnit, 0);
    }

    protected Vector3 GetLocalCenterPosForCoord(Coordinate c) {
        Vector3 localPos = GetLocalPosForCoord(c);
        return new Vector3(localPos.x + data.halfBlockSizeUnit, localPos.y - data.halfBlockSizeUnit, 0);
    }

    protected Coordinate GetCoordForLocalPos(Vector3 p) {
        return new Coordinate(Mathf.FloorToInt(p.x / data.totalBlockSizeUnit), Mathf.FloorToInt(-p.y / data.totalBlockSizeUnit));
    }

    protected abstract GameObject GetBlockPrefab(int x, int y);

    public abstract void BlockClicked(GameObject go);
    public abstract void BlockHovered(GameObject go);

    public abstract Path GetManhattanPath(Vector3 fromLocalPos, Coordinate destCoord);

    public Vector3 GetRandomPosInCoord(Coordinate coord) {
        Vector3 topLeft = GetLocalPosForCoord(coord);

        float dx = Random.Range(data.halfGapSizeUnit, data.totalBlockSizeUnit - data.halfGapSizeUnit);
        float dy = Random.Range(data.halfGapSizeUnit, data.totalBlockSizeUnit - data.halfGapSizeUnit);

        return new Vector3(topLeft.x + dx, topLeft.y - dy, 0);
    }

    public float GetManhattanDistanceToCoord(Vector3 fromLocalPos, Coordinate destCoord) {
        return Util.manhattanDistance(GetCoordForLocalPos(fromLocalPos), destCoord);
    }
}
