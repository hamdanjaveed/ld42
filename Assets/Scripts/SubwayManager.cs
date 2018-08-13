using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayManager : BlockManager, IBlockHandler {
    private const float subwayWeightModifier = 0.5f;

    [SerializeField] GameObject subwayBlockPrefab;
    [SerializeField] GameObject pathControls;
    [SerializeField] GameObject pathConfirmButton;

    [SerializeField] CityManager cityManager;

    private List<Coordinate> currentPath = new List<Coordinate>();
    private List<Coordinate> pathToEdit = new List<Coordinate>();
    private List<List<Coordinate>> paths = new List<List<Coordinate>>();

    protected override void Start() {
        base.Start();
    }

    public void CompletePath() {
        if (currentPath.Count == 0) {
            // Debug.Log("No path to complete!");
        } else {
            paths.Add(new List<Coordinate>(currentPath));
            currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().confirmed = true);
            currentPath.Clear();
            pathToEdit.Clear();

            pathConfirmButton.SetActive(false);
            pathControls.SetActive(false);
        }
    }

    public void CancelPath() {
        currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().state = SubwayBlock.State.EMPTY);
        currentPath.Clear();

        if (pathToEdit.Count > 0) {
            currentPath = new List<Coordinate>(pathToEdit);
            currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().confirmed = true);
            UpdatePath();

            paths.Add(new List<Coordinate>(currentPath));
            currentPath.Clear();
            pathToEdit.Clear();
        }

        pathConfirmButton.SetActive(false);
        pathControls.SetActive(false);
    }

    public override void BlockClicked(GameObject go) {
        SubwayBlock block = go.GetComponent<SubwayBlock>();
        if (currentPath.Count == 0) {
            // Not editing a current path
            if (block.state == SubwayBlock.State.EMPTY) {
                // Start a new path
                currentPath.Add(block.pos);

                pathControls.SetActive(true);
                pathConfirmButton.SetActive(false);
            } else {
                // Edit an existing path

                // Find which path we're editing
                pathToEdit = paths.Find(path => path.Contains(block.pos));

                if (pathToEdit.Count == 0) {
                    // Debug.Log("Couldn't find path");
                } else {
                    currentPath = new List<Coordinate>(pathToEdit);
                    paths.Remove(pathToEdit);

                    currentPath.ForEach(coord => GetBlock(coord).GetComponent<SubwayBlock>().confirmed = false);

                    pathControls.SetActive(true);
                    pathConfirmButton.SetActive(true);
                }
            }
        } else {
            // Building a current path
            if (block.state == SubwayBlock.State.EMPTY) {
                // Continue building current path
                Block previousBlock = GetBlock(currentPath[currentPath.Count - 1]).GetComponent<Block>();
                if (block.isInLineWith(previousBlock)) {
                    List<Coordinate> newPath = previousBlock.getPathToBlock(block);
                    int ind = newPath.FindIndex(p => GetBlock(p).GetComponent<SubwayBlock>().state != SubwayBlock.State.EMPTY);
                    if (ind != -1) {
                        // Debug.Log("Path conflicts with previous path! " + ind);
                    } else {
                        newPath.ForEach(p => currentPath.Add(p));
                        pathConfirmButton.SetActive(true);
                    }
                } else {
                    // Debug.Log("Path must be a straight line!");
                }
            } else {
                // Truncate current path
                int i = currentPath.FindIndex(coord => coord == block.pos);
                if (i != -1) {
                    currentPath.GetRange(i + 1, currentPath.Count - i - 1).ForEach(coord => {
                        GetBlock(coord).GetComponent<SubwayBlock>().state = SubwayBlock.State.EMPTY;
                    });

                    currentPath.RemoveRange(i + 1, currentPath.Count - i - 1);

                    pathConfirmButton.SetActive(currentPath.Count > 1);
                }
            }
        }

        UpdatePath();
    }

    public override void BlockHovered(GameObject go) {
        // Debug.Log("Block clicked in subway: " + go.GetComponent<Block>().pos);
    }

    public override Path GetManhattanPath(Vector3 fromLocalPos, Coordinate destCoord) {
        return Path.Empty();
    }

    protected override GameObject GetBlockPrefab(int x, int y) {
        return subwayBlockPrefab;
    }

    private void UpdatePath() {
        // string s = "";
        // currentPath.ForEach(p => s += p + " ");
        // Debug.Log("Updating path with " + currentPath.Count + " blocks: " + s);
        if (currentPath.Count == 1) {
            GetBlock(currentPath[0]).GetComponent<SubwayBlock>().state = SubwayBlock.State.NODE;
        } else {
            // 0 or 2+ blocks
            for (int i = 0; i < currentPath.Count; i++) {
                Block block = GetBlock(currentPath[i]).GetComponent<Block>();
                SubwayBlock subwayBlock = GetBlock(currentPath[i]).GetComponent<SubwayBlock>();
                if (i == 0 || i == currentPath.Count - 1) {
                    // Ends
                    SubwayBlock.Direction dir;
                    if (i == 0) {
                        // Beginning
                        Block nextBlock = GetBlock(currentPath[i + 1]).GetComponent<Block>();
                        dir = block.getDirectionRelativeTo(nextBlock);
                    } else {
                        // Ending
                        Block previousBlock = GetBlock(currentPath[i - 1]).GetComponent<Block>();
                        dir = block.getDirectionRelativeTo(previousBlock);
                    }

                    switch (dir) {
                        case SubwayBlock.Direction.UP:
                            subwayBlock.state = SubwayBlock.State.NODE_UP;
                            break;
                        case SubwayBlock.Direction.RIGHT:
                            subwayBlock.state = SubwayBlock.State.NODE_RIGHT;
                            break;
                        case SubwayBlock.Direction.LEFT:
                            subwayBlock.state = SubwayBlock.State.NODE_LEFT;
                            break;
                        case SubwayBlock.Direction.DOWN:
                            subwayBlock.state = SubwayBlock.State.NODE_DOWN;
                            break;
                    }
                } else {
                    // Middle
                    Block previousBlock = GetBlock(currentPath[i - 1]).GetComponent<Block>();
                    Block nextBlock = GetBlock(currentPath[i + 1]).GetComponent<Block>();
                    SubwayBlock.Direction previousDir = block.getDirectionRelativeTo(previousBlock);
                    SubwayBlock.Direction nextDir = block.getDirectionRelativeTo(nextBlock);

                    // Horizontal
                    if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.RIGHT) || (previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.LEFT)) {
                        subwayBlock.state = SubwayBlock.State.HORIZONTAL;
                    } else if ((previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.UP)) {
                        subwayBlock.state = SubwayBlock.State.VERTICAL;
                    } else if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.LEFT)) {
                        subwayBlock.state = SubwayBlock.State.CORNER_DOWN_LEFT;
                    } else if ((previousDir == SubwayBlock.Direction.LEFT && nextDir == SubwayBlock.Direction.UP) || (previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.LEFT)) {
                        subwayBlock.state = SubwayBlock.State.CORNER_LEFT_UP;
                    } else if ((previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.DOWN) || (previousDir == SubwayBlock.Direction.DOWN && nextDir == SubwayBlock.Direction.RIGHT)) {
                        subwayBlock.state = SubwayBlock.State.CORNER_RIGHT_DOWN;
                    } else if ((previousDir == SubwayBlock.Direction.RIGHT && nextDir == SubwayBlock.Direction.UP) || (previousDir == SubwayBlock.Direction.UP && nextDir == SubwayBlock.Direction.RIGHT)) {
                        subwayBlock.state = SubwayBlock.State.CORNER_UP_RIGHT;
                    } else {
                        Debug.Log("OOPS!");
                    }
                }
            }
        }
    }
}
