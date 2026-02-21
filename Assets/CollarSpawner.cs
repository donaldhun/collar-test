using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class CollarSpawner : MonoBehaviour
{

    public List<CollarData> collarList;

    public GameObject DRILL_BIT;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collarList = CollarParser.LoadAndParseCSV();
        SpawnDrillBits();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnDrillBits()
    {
        foreach(CollarData currentCollar in collarList) {
            GameObject newDrillBit = Instantiate(DRILL_BIT, currentCollar.collarPosition, Quaternion.identity);
            newDrillBit.name = currentCollar.BHID;
        }
    }
}
