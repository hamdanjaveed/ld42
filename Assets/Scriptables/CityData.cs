using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "Data/City")]
public class CityData : ScriptableObject {
    public int familyMin = 2;
    public int familyMax = 5;

    public float famiilySpawnTime = 1f;

    public float secondsPerHour = 0.5f;
}
