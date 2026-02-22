using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

// Todo: color each line (cylinder) in CreateDrillPath() according to ore value (from assay)
// Todo: make is to the very first node is larger/different color (at least distinguishable)
// Todo: scale final drill path because right now it is huge
// Todo: (not in this file) but read in CSVs instead of hardcoding sample drill path


public class AssaySegment
{
    // assay data
    public float from; 
    public float to;   
    public double oreValue;
}

public class SurveyDirection
{
    // survey data
    
    public float at; // depth of measurement
    public float az; // direction : 0=north
    public float dip; // steepness of hole: 90= straight down
}

public class DrillPathHandler : MonoBehaviour
{
    public GameObject DrillNodePrefab;
    public GameObject DrillPathPrefab;
    public List<AssaySegment> segments = new List<AssaySegment>();
    public List<SurveyDirection> directions = new List<SurveyDirection>();

    public float lineRadius = 2f;
    public float jointScale = 1.0f;

    void Start()
    {
        LoadSampleData();
        CreateDrillPath();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // uses assay (mineral segments) and survey (drill direction) to plot
    // drill path joints, and lines connecting them colored according to their
    // ore value

    void CreateDrillPath()
    {

        // for every i, i+1 segments, make a cylinder connecting
        // the two points. 

        // place it in the midpoint and scale the cylinder's y
        // by (segment_length/2), this will make it look like a 
        // continous line

        // color each individual segment according to its ore value

        for (int i = 0; i < segments.Count; i++)
        {
            AssaySegment segment = segments[i];

            // Get start/end 3d points for this assay segment
            Vector3 startPoint = GetPositionAtDepth(segment.from);
            Vector3 endPoint = GetPositionAtDepth(segment.to);

            // Spawn node at beginning/end of assay segment (black sphere currently)
            GameObject joint = Instantiate(DrillNodePrefab, startPoint, Quaternion.identity, this.transform);
            joint.transform.localScale = Vector3.one * jointScale;

            // Draw the line between the nodes  (will be a cylinder placed halfway between points, then stretched)
            float distance = Vector3.Distance(startPoint, endPoint);

            GameObject line = Instantiate(DrillPathPrefab, this.transform);
            line.transform.position = (startPoint + endPoint) / 2f;

            // rotate the cylinder to point at end point
            line.transform.up = endPoint - startPoint;

            // Scale it to match the distance
            line.transform.localScale = new Vector3(lineRadius, distance / 2f, lineRadius);

            // if last segment, put a node at the end.

            if (i == segments.Count - 1)
            {
                GameObject finalJoint = Instantiate(DrillNodePrefab, endPoint, Quaternion.identity, this.transform);
                finalJoint.transform.localScale = Vector3.one * jointScale;
            }
        }
    
        
    }

    // For GetPositionAtDepth I had a less efficient idea where it would create a cube, rotate 
    // the cube according to survey data, and then move 'foward' relative to the rotated cube. this code
    // made with AI does it without creating that cube so I used that instead.

    Vector3 GetPositionAtDepth(float targetDepth)
    {
        Vector3 currentPos = Vector3.zero; // Starts at local 0,0,0
        float currentCalculatedDepth = 0f;

        for (int i = 0; i < directions.Count; i++)
        {
            SurveyDirection currentSurvey = directions[i];

            // Figure out where this survey angle stops applying
            float nextSurveyDepth = targetDepth; 
            
            if (i < directions.Count - 1 && directions[i + 1].at < targetDepth)
            {
                nextSurveyDepth = directions[i + 1].at;
            }

            // Calculate the distance we travel at this specific angle
            float distanceToMove = nextSurveyDepth - currentCalculatedDepth;

            if (distanceToMove > 0)
            {
                // Create the rotation: Dip is X-axis, Azimuth is Y-axis
                Quaternion rotation = Quaternion.Euler(currentSurvey.dip, currentSurvey.az, 0);
                
                // Move forward by the distance
                Vector3 directionVector = rotation * Vector3.forward;
                currentPos += directionVector * distanceToMove;
                
                currentCalculatedDepth = nextSurveyDepth;
            }

            // If we've reached the depth we asked for, stop calculating
            if (currentCalculatedDepth >= targetDepth)
                break;
        }

        return currentPos;
    }

    void LoadSampleData()
    {

        // survey data:
        // B1-100 has multiple survey entries for its corkscrew path
        directions.Add(new SurveyDirection{at=0f, az=0f, dip=90f});
        directions.Add(new SurveyDirection{at=50f, az=344f, dip=89.8f});
        directions.Add(new SurveyDirection{at=150f, az=13f, dip=88.8f});
        directions.Add(new SurveyDirection{at=250f, az=43f, dip=87.8f});
        directions.Add(new SurveyDirection{at=350f, az=55f, dip=86f});
        directions.Add(new SurveyDirection{at=450f, az=61f, dip=85.5f});
        directions.Add(new SurveyDirection{at=550f, az=95f, dip=84.8f});
        directions.Add(new SurveyDirection{at=650f, az=116f, dip=84.5f});
        directions.Add(new SurveyDirection{at=750f, az=113f, dip=84.5f});
        directions.Add(new SurveyDirection{at=850f, az=119f, dip=84f});
        directions.Add(new SurveyDirection{at=950f, az=133f, dip=84.3f});
        directions.Add(new SurveyDirection{at=1050f, az=138f, dip=85.3f});
        directions.Add(new SurveyDirection{at=1150f, az=158f, dip=86f});
        directions.Add(new SurveyDirection{at=1250f, az=169f, dip=85.5f});
        directions.Add(new SurveyDirection{at=1350f, az=181f, dip=85f});
        directions.Add(new SurveyDirection{at=1450f, az=182f, dip=86.5f});
        directions.Add(new SurveyDirection{at=1550f, az=272f, dip=88.5f});
        directions.Add(new SurveyDirection{at=1650f, az=299f, dip=86.5f});
        directions.Add(new SurveyDirection{at=1737.5f, az=285f, dip=86f});

        // assay data:
        // hardcoded the assay data of B1-100 with AI, later it will just
        // be read in from assay.csv
        segments.Clear(); // Clear the list in case this is called multiple times

        segments.Add(new AssaySegment { from = 0f, to = 422f, oreValue = 0.0 });
        segments.Add(new AssaySegment { from = 422f, to = 432f, oreValue = 0.02 });
        segments.Add(new AssaySegment { from = 432f, to = 442f, oreValue = 0.02 });
        segments.Add(new AssaySegment { from = 442f, to = 445f, oreValue = 0.25 });
        segments.Add(new AssaySegment { from = 445f, to = 455f, oreValue = 0.059999999 });
        segments.Add(new AssaySegment { from = 455f, to = 461f, oreValue = 0.140000001 });
        segments.Add(new AssaySegment { from = 461f, to = 465f, oreValue = 0.280000001 });
        segments.Add(new AssaySegment { from = 465f, to = 469f, oreValue = 0.170000002 });
        segments.Add(new AssaySegment { from = 469f, to = 475f, oreValue = 0.129999995 });
        segments.Add(new AssaySegment { from = 475f, to = 485f, oreValue = 0.100000001 });
        segments.Add(new AssaySegment { from = 485f, to = 494f, oreValue = 0.370000005 });
        segments.Add(new AssaySegment { from = 494f, to = 504f, oreValue = 0.090000004 });
        segments.Add(new AssaySegment { from = 504f, to = 514f, oreValue = 0.07 });
        segments.Add(new AssaySegment { from = 514f, to = 550f, oreValue = 0.0 });
        segments.Add(new AssaySegment { from = 550f, to = 560f, oreValue = 0.119999997 });
        segments.Add(new AssaySegment { from = 560f, to = 565f, oreValue = 0.779999971 });
        segments.Add(new AssaySegment { from = 565f, to = 575f, oreValue = 0.349999994 });
        segments.Add(new AssaySegment { from = 575f, to = 585f, oreValue = 0.239999995 });
        segments.Add(new AssaySegment { from = 585f, to = 595f, oreValue = 0.209999993 });
        segments.Add(new AssaySegment { from = 595f, to = 605f, oreValue = 0.289999992 });
        segments.Add(new AssaySegment { from = 605f, to = 615f, oreValue = 0.400000006 });
        segments.Add(new AssaySegment { from = 615f, to = 625f, oreValue = 0.479999989 });
        segments.Add(new AssaySegment { from = 625f, to = 635f, oreValue = 0.129999995 });
        segments.Add(new AssaySegment { from = 635f, to = 645f, oreValue = 0.460000008 });
        segments.Add(new AssaySegment { from = 645f, to = 655f, oreValue = 0.430000007 });
        segments.Add(new AssaySegment { from = 655f, to = 665f, oreValue = 0.119999997 });
        segments.Add(new AssaySegment { from = 665f, to = 675f, oreValue = 0.140000001 });
        segments.Add(new AssaySegment { from = 675f, to = 685f, oreValue = 0.370000005 });
        segments.Add(new AssaySegment { from = 685f, to = 695f, oreValue = 0.219999999 });
        segments.Add(new AssaySegment { from = 695f, to = 705f, oreValue = 0.07 });
        segments.Add(new AssaySegment { from = 705f, to = 715f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 715f, to = 750f, oreValue = 0.0 });
        segments.Add(new AssaySegment { from = 750f, to = 756f, oreValue = 0.829999983 });
        segments.Add(new AssaySegment { from = 756f, to = 763f, oreValue = 0.050000001 });
        segments.Add(new AssaySegment { from = 763f, to = 771f, oreValue = 0.629999995 });
        segments.Add(new AssaySegment { from = 771f, to = 776f, oreValue = 0.079999998 });
        segments.Add(new AssaySegment { from = 776f, to = 778f, oreValue = 0.439999998 });
        segments.Add(new AssaySegment { from = 778f, to = 788f, oreValue = 0.07 });
        segments.Add(new AssaySegment { from = 788f, to = 798f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 798f, to = 903f, oreValue = 0.0 });
        segments.Add(new AssaySegment { from = 903f, to = 913f, oreValue = 0.02 });
        segments.Add(new AssaySegment { from = 913f, to = 923f, oreValue = 0.039999999 });
        segments.Add(new AssaySegment { from = 923f, to = 934f, oreValue = 0.280000001 });
        segments.Add(new AssaySegment { from = 934f, to = 945f, oreValue = 0.100000001 });
        segments.Add(new AssaySegment { from = 945f, to = 955f, oreValue = 0.090000004 });
        segments.Add(new AssaySegment { from = 955f, to = 961f, oreValue = 0.029999999 });
        segments.Add(new AssaySegment { from = 961f, to = 965f, oreValue = 0.239999995 });
        segments.Add(new AssaySegment { from = 965f, to = 975f, oreValue = 0.150000006 });
        segments.Add(new AssaySegment { from = 975f, to = 985f, oreValue = 0.310000002 });
        segments.Add(new AssaySegment { from = 985f, to = 995f, oreValue = 0.129999995 });
        segments.Add(new AssaySegment { from = 995f, to = 1001f, oreValue = 0.270000011 });
        segments.Add(new AssaySegment { from = 1001f, to = 1011f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 1011f, to = 1021f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 1021f, to = 1031f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 1031f, to = 1229f, oreValue = 0.0 });
        segments.Add(new AssaySegment { from = 1229f, to = 1239f, oreValue = 0.119999997 });
        segments.Add(new AssaySegment { from = 1239f, to = 1249f, oreValue = 0.02 });
        segments.Add(new AssaySegment { from = 1249f, to = 1255f, oreValue = 0.039999999 });
        segments.Add(new AssaySegment { from = 1255f, to = 1265f, oreValue = 0.239999995 });
        segments.Add(new AssaySegment { from = 1265f, to = 1275f, oreValue = 0.119999997 });
        segments.Add(new AssaySegment { from = 1275f, to = 1285f, oreValue = 0.379999995 });
        segments.Add(new AssaySegment { from = 1285f, to = 1295f, oreValue = 0.280000001 });
        segments.Add(new AssaySegment { from = 1295f, to = 1300f, oreValue = 0.620000005 });
        segments.Add(new AssaySegment { from = 1300f, to = 1305f, oreValue = 0.050000001 });
        segments.Add(new AssaySegment { from = 1305f, to = 1310f, oreValue = 0.790000021 });
        segments.Add(new AssaySegment { from = 1310f, to = 1315f, oreValue = 0.860000014 });
        segments.Add(new AssaySegment { from = 1315f, to = 1320f, oreValue = 0.689999998 });
        segments.Add(new AssaySegment { from = 1320f, to = 1325f, oreValue = 0.219999999 });
        segments.Add(new AssaySegment { from = 1325f, to = 1333f, oreValue = 0.029999999 });
        segments.Add(new AssaySegment { from = 1333f, to = 1340f, oreValue = 0.870000005 });
        segments.Add(new AssaySegment { from = 1340f, to = 1345f, oreValue = 0.439999998 });
        segments.Add(new AssaySegment { from = 1345f, to = 1353f, oreValue = 0.349999994 });
        segments.Add(new AssaySegment { from = 1353f, to = 1357f, oreValue = 0.100000001 });
        segments.Add(new AssaySegment { from = 1357f, to = 1365f, oreValue = 0.419999987 });
        segments.Add(new AssaySegment { from = 1365f, to = 1370f, oreValue = 0.579999983 });
        segments.Add(new AssaySegment { from = 1370f, to = 1376f, oreValue = 0.74000001 });
        segments.Add(new AssaySegment { from = 1376f, to = 1385f, oreValue = 0.319999993 });
        segments.Add(new AssaySegment { from = 1385f, to = 1395f, oreValue = 0.170000002 });
        segments.Add(new AssaySegment { from = 1395f, to = 1405f, oreValue = 0.620000005 });
        segments.Add(new AssaySegment { from = 1405f, to = 1415f, oreValue = 0.180000007 });
        segments.Add(new AssaySegment { from = 1415f, to = 1425f, oreValue = 0.239999995 });
        segments.Add(new AssaySegment { from = 1425f, to = 1435f, oreValue = 0.319999993 });
        segments.Add(new AssaySegment { from = 1435f, to = 1445f, oreValue = 0.230000004 });
        segments.Add(new AssaySegment { from = 1445f, to = 1450f, oreValue = 0.280000001 });
        segments.Add(new AssaySegment { from = 1450f, to = 1455f, oreValue = 0.720000029 });
        segments.Add(new AssaySegment { from = 1455f, to = 1463f, oreValue = 0.769999981 });
        segments.Add(new AssaySegment { from = 1463f, to = 1467f, oreValue = 0.219999999 });
        segments.Add(new AssaySegment { from = 1467f, to = 1475f, oreValue = 1.50999999 });
        segments.Add(new AssaySegment { from = 1475f, to = 1480f, oreValue = 0.600000024 });
        segments.Add(new AssaySegment { from = 1480f, to = 1485f, oreValue = 1.27999997 });
        segments.Add(new AssaySegment { from = 1485f, to = 1490f, oreValue = 1.16999996 });
        segments.Add(new AssaySegment { from = 1490f, to = 1495f, oreValue = 1.07000005 });
        segments.Add(new AssaySegment { from = 1495f, to = 1500f, oreValue = 0.850000024 });
        segments.Add(new AssaySegment { from = 1500f, to = 1505f, oreValue = 0.239999995 });
        segments.Add(new AssaySegment { from = 1505f, to = 1510f, oreValue = 1.91999996 });
        segments.Add(new AssaySegment { from = 1510f, to = 1515f, oreValue = 1.39999998 });
        segments.Add(new AssaySegment { from = 1515f, to = 1520f, oreValue = 1.67999995 });
        segments.Add(new AssaySegment { from = 1520f, to = 1525f, oreValue = 1.52999997 });
        segments.Add(new AssaySegment { from = 1525f, to = 1530f, oreValue = 1.24000001 });
        segments.Add(new AssaySegment { from = 1530f, to = 1535f, oreValue = 2.1500001 });
        segments.Add(new AssaySegment { from = 1535f, to = 1540f, oreValue = 1.15999997 });
        segments.Add(new AssaySegment { from = 1540f, to = 1545f, oreValue = 1.64999998 });
        segments.Add(new AssaySegment { from = 1545f, to = 1550f, oreValue = 1.75999999 });
        segments.Add(new AssaySegment { from = 1550f, to = 1555f, oreValue = 2.13000011 });
        segments.Add(new AssaySegment { from = 1555f, to = 1560f, oreValue = 0.850000024 });
        segments.Add(new AssaySegment { from = 1560f, to = 1565f, oreValue = 0.75999999 });
        segments.Add(new AssaySegment { from = 1565f, to = 1570f, oreValue = 1.24000001 });
        segments.Add(new AssaySegment { from = 1570f, to = 1575f, oreValue = 1.19000006 });
        segments.Add(new AssaySegment { from = 1575f, to = 1580f, oreValue = 1.63 });
        segments.Add(new AssaySegment { from = 1580f, to = 1585f, oreValue = 1.37 });
        segments.Add(new AssaySegment { from = 1585f, to = 1590f, oreValue = 1.47000003 });
        segments.Add(new AssaySegment { from = 1590f, to = 1595f, oreValue = 1.16999996 });
        segments.Add(new AssaySegment { from = 1595f, to = 1600f, oreValue = 1.04999995 });
        segments.Add(new AssaySegment { from = 1600f, to = 1605f, oreValue = 1.53999996 });
        segments.Add(new AssaySegment { from = 1605f, to = 1610f, oreValue = 1.23000002 });
        segments.Add(new AssaySegment { from = 1610f, to = 1615f, oreValue = 0.479999989 });
        segments.Add(new AssaySegment { from = 1615f, to = 1620f, oreValue = 0.639999986 });
        segments.Add(new AssaySegment { from = 1620f, to = 1625f, oreValue = 0.860000014 });
        segments.Add(new AssaySegment { from = 1625f, to = 1630f, oreValue = 0.819999993 });
        segments.Add(new AssaySegment { from = 1630f, to = 1635f, oreValue = 0.75 });
        segments.Add(new AssaySegment { from = 1635f, to = 1640f, oreValue = 0.649999976 });
        segments.Add(new AssaySegment { from = 1640f, to = 1645f, oreValue = 0.5 });
        segments.Add(new AssaySegment { from = 1645f, to = 1650f, oreValue = 0.519999981 });
        segments.Add(new AssaySegment { from = 1650f, to = 1655f, oreValue = 0.519999981 });
        segments.Add(new AssaySegment { from = 1655f, to = 1660f, oreValue = 0.560000002 });
        segments.Add(new AssaySegment { from = 1660f, to = 1665f, oreValue = 0.639999986 });
        segments.Add(new AssaySegment { from = 1665f, to = 1670f, oreValue = 0.75999999 });
        segments.Add(new AssaySegment { from = 1670f, to = 1675f, oreValue = 0.819999993 });
        segments.Add(new AssaySegment { from = 1675f, to = 1680f, oreValue = 0.5 });
        segments.Add(new AssaySegment { from = 1680f, to = 1685f, oreValue = 0.670000017 });
        segments.Add(new AssaySegment { from = 1685f, to = 1690f, oreValue = 0.319999993 });
        segments.Add(new AssaySegment { from = 1690f, to = 1695f, oreValue = 0.430000007 });
        segments.Add(new AssaySegment { from = 1695f, to = 1700f, oreValue = 0.540000021 });
        segments.Add(new AssaySegment { from = 1700f, to = 1705f, oreValue = 0.330000013 });
        segments.Add(new AssaySegment { from = 1705f, to = 1710f, oreValue = 0.430000007 });
        segments.Add(new AssaySegment { from = 1710f, to = 1715f, oreValue = 0.419999987 });
        segments.Add(new AssaySegment { from = 1715f, to = 1720f, oreValue = 0.409999996 });
        segments.Add(new AssaySegment { from = 1720f, to = 1725f, oreValue = 0.75 });
        segments.Add(new AssaySegment { from = 1725f, to = 1733f, oreValue = 0.519999981 });
        segments.Add(new AssaySegment { from = 1733f, to = 1739f, oreValue = 0.389999986 });
        segments.Add(new AssaySegment { from = 1739f, to = 1745f, oreValue = 0.400000006 });
        segments.Add(new AssaySegment { from = 1745f, to = 1755f, oreValue = 0.25 });
        segments.Add(new AssaySegment { from = 1755f, to = 1761f, oreValue = 0.469999999 });
        segments.Add(new AssaySegment { from = 1761f, to = 1771f, oreValue = 0.01 });
        segments.Add(new AssaySegment { from = 1771f, to = 1785f, oreValue = 0.01 });
    }
}