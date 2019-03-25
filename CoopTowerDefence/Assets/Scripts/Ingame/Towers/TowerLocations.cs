using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The code for getting the positions and area's towers and traps can be placed.
public class TowerLocations : MonoBehaviour
{
    public TowerPlacementLocation[] placementLocations;
    public float towerSize = 1;
    public LayerMask detectMask;
    public bool disableGizmos;

    //Getting the nearest snapping point to a given position.
    /// Before you use this you need to calculate in what area the point is located.
    /// To do this you can use the void IsInBetween() for each available area.
    /// It then gets all points by using GetAreaPoints() and then calculates the nearest point by making a list of the distances.
    public Vector3 GetPlacementPoint( Vector3 position, int index)
    {
        Vector3[] points = GetAreaPoints(index);
        List<float> distances = new List<float>();
        foreach (Vector3 point in points)
            distances.Add(Vector3.Distance(position, point));
        int lowestIndex = 0;
        for (int i = 1; i < distances.Count; i++)
            if (distances[i] <= distances[lowestIndex])
                lowestIndex = i;
        return points[lowestIndex];
    }

    //Gives you the position for placing an object on the right spot
    /// Before you use this you need to calculate in what area the point is located.
    /// To do this you can use the void IsInBetween() for each available area.
    /// It looks at the scale for the area it is in and then shoots an raycast to the ground to get the location.
    public Vector3 StraightDownPoint(Vector3 position, int index)
    {
        TowerPlacementLocation loc = placementLocations[index];
        Vector3 scale = GetCornerScale(loc.position1_green, loc.position2_yellow);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(position + Vector3.up * scale.y, Vector3.down, out hit, scale.y, detectMask))
            return hit.point;
        return new Vector3();
    }

    //Gives you the snapping points for the given area
    /// It first gets the center and the scale of the given area it is in.
    /// Afterwards it starts in an corner and uses a forloop to go through each posible position in the given are.
    /// Once it has gone through each point it returns a vector3 array with the spots.
    public Vector3[] GetAreaPoints(int index)
    {
        TowerPlacementLocation loc = placementLocations[index];
        List<Vector3> points = new List<Vector3>();
        Vector3 center = GetCenter(loc.position1_green, loc.position2_yellow) + loc.centerOffset;
        Vector3 scale = GetCornerScale(loc.position1_green, loc.position2_yellow);

        /// Gets the maximum of points that fits in the given area.
        Vector2 maximum = new Vector2(Mathf.Floor(scale.x / towerSize), Mathf.Floor(scale.z / towerSize));
        for (int x = 0; x < maximum.x; x++)
            for (int z = 0; z < maximum.y; z++)
            {
                Vector3 position = new Vector3(x * towerSize + (towerSize / 2), 0, z * towerSize + towerSize / 2) + center - (scale / 2) + loc.Toweroffset;
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(position + Vector3.up * scale.y, Vector3.down, out hit, scale.y,detectMask))
                    points.Add(hit.point);
            }
        return points.ToArray();
    }

    //Calculates the center between 2 points
    /// This is just a lerp to get the point in between 2 points.
    public static Vector3 GetCenter(Vector3 pos1, Vector3 pos2)
    {
        return Vector3.Lerp(pos1, pos2, 0.5f);
    }

    //Calculates the scale for the 2 given corners
    /// It gets the diffrence between for each axis of the 2 given points.
    public static Vector3 GetCornerScale(Vector3 corner1, Vector3 corner2)
    {
        Vector3 scale = new Vector3();
        scale.x = Mathf.Abs(corner1.x - corner2.x);
        scale.y = Mathf.Abs(corner1.y - corner2.y);
        scale.z = Mathf.Abs(corner1.z - corner2.z);
        return scale;
    }

    //Checks if the given float is between 2 values
    /// It first looks wich float is higher than the other
    /// Then it checks if the given float is in between the 2 other floats.
    public static bool FloatInBetween(float checkFloat, float float1, float float2)
    {
        float higher = (float1 > float2) ? float1 : float2;
        float lower = (float1 < float2) ? float1 : float2;
        if (checkFloat < lower || checkFloat > higher)
            return false;
        return true;
    }

    //Checks if an point is in in the given area.
    /// Uses the void FloatInBetween for each axis.
    public static bool IsInBetween(Vector3 checkPos, Vector3 pos1, Vector3 pos2)
    {
        //X
        if (pos1.x == pos2.x && checkPos.x != pos1.x)
            return false;
        else if(pos1.x != pos2.x)
            if (!FloatInBetween(checkPos.x, pos1.x, pos2.x))
                return false;
        //Y
        if (pos1.y == pos2.y && checkPos.y != pos1.y)
            return false;
        else if (pos1.y != pos2.y)
            if (!FloatInBetween(checkPos.y, pos1.y, pos2.y))
                return false;
        //Z
        if (pos1.z == pos2.z && checkPos.z != pos1.z)
            return false;
        else if (pos1.z != pos2.z)
            if (!FloatInBetween(checkPos.z, pos1.z, pos2.z))
                return false;
        return true;
    }

    //Gizmos
    /// Draws gizmos for the area's and available spots to make it easier to place the area's in the inspector.
    public void OnDrawGizmosSelected()
    {
        if(placementLocations != null && !disableGizmos)
            for (int i = 0; i < placementLocations.Length; i++)
            {
                TowerPlacementLocation loc = placementLocations[i];
                Gizmos.color = (loc.placementType == TowerPlacementLocation.PlacementType.Tower) ? Color.blue : Color.red;
                Gizmos.DrawWireCube(GetCenter(loc.position1_green, loc.position2_yellow) + loc.centerOffset, GetCornerScale(loc.position1_green, loc.position2_yellow));
                Gizmos.color = Color.green;
                Gizmos.DrawCube(loc.position1_green + loc.centerOffset, Vector3.one * 0.08f);
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(loc.position2_yellow + loc.centerOffset, Vector3.one * 0.08f);
                if(loc.placementType == TowerPlacementLocation.PlacementType.Trap)
                {
                    Gizmos.color = Color.green;
                    foreach (Vector3 point in GetAreaPoints(i))
                        Gizmos.DrawSphere(point, towerSize / 4);
                }
            }
    }

    //information Contructor for each placeable location
    /// This just makes it easier to make it easier to change the values.
    [System.Serializable]
    public class TowerPlacementLocation
    {
        [Header("Position")]
        public Vector3 centerOffset;
        public Vector3 position1_green = Vector3.one;
        public Vector3 position2_yellow = -Vector3.one;
        [Header("OtherOptions")]
        public Vector3 Toweroffset = Vector3.zero;
        public PlacementType placementType;
        public enum PlacementType { Tower, Trap}
    }
}
