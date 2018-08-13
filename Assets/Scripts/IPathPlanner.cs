using UnityEngine;

public interface IPathPlanner {
    Path GetManhattanPath(Vector3 fromLocalPos, Coordinate destCoord);
    Vector3 GetRandomPosInCoord(Coordinate coord);
    float GetManhattanDistanceToCoord(Vector3 fromLocalPos, Coordinate destCoord);

}
