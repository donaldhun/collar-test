using System.Collections.Generic;
using UnityEngine;
using System.Globalization;


[System.Serializable]
public class CollarData
{
    // ID of drill, and location of the start of drill path
    public string BHID;

    public Vector3 collarPosition;
}

public static class CollarParser 
{

    public static List<CollarData> LoadAndParseCSV()
    {
        // The x/y values are in the millions which can't be displayed well in unity, so scale the positions to fit in the scene
        const float positionScaler = 0.01f;

        List<CollarData> collarList = new List<CollarData>();

        // We'll have to change how we load the CSV's later but for now load it from resources
        TextAsset csvFile = Resources.Load<TextAsset>("VR-AND-AR-main/collar");

        if (csvFile == null)
        {
            Debug.LogError("Error loading collar.csv!");
            return collarList;
        }

        // Split csv into lines, removes entry lines
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);


        // And store in list
        for (int i = 1; i < lines.Length; i++)
        {
            string[] collarRows = lines[i].Split(',');

            CollarData currentCollar = new CollarData();
            currentCollar.BHID = collarRows[0];

            
            float x = float.Parse(collarRows[1]) * positionScaler;
            float z = float.Parse(collarRows[2]) * positionScaler;
            float y = float.Parse(collarRows[3]) * positionScaler;

            currentCollar.collarPosition.x = x;
            currentCollar.collarPosition.y = y;
            currentCollar.collarPosition.z = z;

            // Add to our list
            collarList.Add(currentCollar);
        
        }

        Debug.Log($"{collarList.Count} collar positions loaded");
        return collarList;
    }
}
