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

	private const float blockChosenTimeThreshold = 3000f; // Seconds between spawning a family

	private List<Coordinate> unoccupiedIndustrialBlocks;
	private List<Coordinate> unoccupiedResidentialBlocks;
	private List<Coordinate> occupiedIndustrialBlocks;
	private List<Coordinate> occupiedResidentialBlocks;

	private float timeSinceLastBlockChosen;

	protected override void Start() {
		unoccupiedIndustrialBlocks = new List<Coordinate>();
		unoccupiedResidentialBlocks = new List<Coordinate>();

		base.Start();

		for (int x = 0; x < data.numBlocks; x++) {
			for (int y = 0; y < data.numBlocks; y++) {
				if (y < data.numBlocks / 2) {
					// Industrial
					unoccupiedIndustrialBlocks.Add(new Coordinate(x, y));
				} else {
					// Residential
					unoccupiedResidentialBlocks.Add(new Coordinate(x, y));
				}
			}
		}

		occupiedIndustrialBlocks = new List<Coordinate>();
		occupiedResidentialBlocks = new List<Coordinate>();

		timeSinceLastBlockChosen = blockChosenTimeThreshold;
	}

	void Update() {
		if (unoccupiedResidentialBlocks.Count > 0) {
			if (timeSinceLastBlockChosen > blockChosenTimeThreshold) {
				// ChoosePairOfBlocks();
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
		unoccupiedResidentialBlocks.RemoveAt(r);
		occupiedResidentialBlocks.Add(houseCoord);

		ResidentialCityBlock houseBlock = GetBlock(houseCoord).GetComponent<ResidentialCityBlock>();
		int familySize = Random.Range(1, 1);
		for (int i = 0; i < familySize; i++) {
			Citizen familyMember = AddCitizen(houseBlock);
			familyMember.transform.position = (Random.Range(0, 2) == 0 ? leftCitizenSpawnPos : rightCitizenSpawnPos);
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
