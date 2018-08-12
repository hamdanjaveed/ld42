using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : BlockManager {
	[SerializeField] GameObject industrialCityBlockPrefab;
	[SerializeField] GameObject residentialCityBlockPrefab;
	[SerializeField] GameObject citizenPrefab;
	[SerializeField] GameObject citizenContainer;

	[SerializeField] Vector3 leftCitizenSpawnPos;
	[SerializeField] Vector3 rightCitizenSpawnPos;

	private const float blockChosenTimeThreshold = 1000000f; // Seconds between spawning a family
	private const int familyMin = 1;
	private const int familyMax = 1;

	private List<Coordinate> unoccupiedIndustrialBlocks;
	private List<Coordinate> unoccupiedResidentialBlocks;
	private List<Coordinate> occupiedIndustrialBlocks;
	private List<Coordinate> occupiedResidentialBlocks;

	private float timeSinceLastBlockChosen;

	protected override void Start() {
		unoccupiedIndustrialBlocks = new List<Coordinate>();
		unoccupiedResidentialBlocks = new List<Coordinate>();

		base.Start();

		// for (int x = 0; x < data.numBlocks; x++) {
		// 	for (int y = 0; y < data.numBlocks; y++) {
		// 		if (y < data.numBlocks / 2) {
		// 			// Industrial
		// 			unoccupiedIndustrialBlocks.Add(new Coordinate(x, y));
		// 		} else {
		// 			// Residential
		// 			// unoccupiedResidentialBlocks.Add(new Coordinate(x, y));
		// 		}
		// 	}
		// }

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
		// 	int j = GetBlock(occupiedResidentialBlocks[i]).GetComponent<ResidentialCityBlock>().residents.Count;
		// 	if (j > m) m = j;
		// }
		// Debug.Log(m);

		timeSinceLastBlockChosen += Time.deltaTime;
	}

	public override void BlockClicked(GameObject go) {
		// Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
	}

	public override void BlockHovered(GameObject go) {
		// Debug.Log("Block clicked in city: " + go.GetComponent<Block>().pos);
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
		// Debug.Log("Chose " + houseCoord + " with index " + r + " and total " + unoccupiedResidentialBlocks.Count);
		// List<Coordinate> allFound = unoccupiedResidentialBlocks.FindAll(p => p == houseCoord);
		// Debug.Log("Found " + allFound.Count + " copies");
		unoccupiedResidentialBlocks.Remove(houseCoord);
		// int ind = unoccupiedResidentialBlocks.FindIndex(p => p == houseCoord);
		// if (ind != -1) {
		// 	Debug.Log("This was duped: " + houseCoord + " at index " + ind + " with total " + unoccupiedResidentialBlocks.Count);
		// 	Debug.Break();
		// }
		occupiedResidentialBlocks.Add(houseCoord);

		ResidentialCityBlock houseBlock = GetBlock(houseCoord).GetComponent<ResidentialCityBlock>();
		int familySize = Random.Range(familyMin, familyMax);
		for (int i = 0; i < familySize; i++) {
			Citizen familyMember = AddCitizen(houseBlock);
			familyMember.SetSpawn(Random.Range(0, 2) == 0 ? leftCitizenSpawnPos : rightCitizenSpawnPos);
			houseBlock.AddResident(familyMember);
		}
	}

	private Citizen AddCitizen(ResidentialCityBlock home) {
		GameObject go = Instantiate(citizenPrefab);
		go.transform.parent = citizenContainer.transform;
		Citizen citizen = go.GetComponent<Citizen>();

		citizen.SetHome(home);

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
}
