using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResidentialCityBlock : CityBlock {
    public List<Citizen> residents;

    void Start() {
        residents = new List<Citizen>();
    }

    public void AddResident(Citizen resident) {
        residents.Add(resident);
    }
}
