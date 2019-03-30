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
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(position + Vector3.up * loc.height, Vector3.down, out hit, loc.height + (loc.height * 0.01f), detectMask))
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
        if (loc.gridPlacementCorner == null) return new Vector3[0];
        Vector3 otherCorner = GetOtherCornerPosition(loc.gridPlacementCorner.position, loc.tileAmount, loc.height, towerSize, loc.invertX, loc.invertZ);
        List<Vector3> points = new List<Vector3>();
        Vector3 center = GetCenter(loc.gridPlacementCorner.position, otherCorner);
        Vector3 scale = GetCornerScale(loc.gridPlacementCorner.position, otherCorner);

        /// Gets the maximum of points that fits in the given area.
        Vector2 maximum = loc.tileAmount;
        for (int x = 0; x < maximum.x; x++)
            for (int z = 0; z < maximum.y; z++)
            {
                Vector3 position = new Vector3(x * towerSize + (towerSize / 2), 0, z * towerSize + towerSize / 2) + center - (scale / 2);
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(position + Vector3.up * scale.y, Vector3.down, out hit, scale.y + (loc.height * 0.01f), detectMask))
                    points.Add(hit.point);
            }
        return points.ToArray();
    }

    //Calculates the center between 2 points
    /// This is just a lerp to get the point in between 2 points.
    public static Vector3 GetCenter(Vector3 corner1, Vector3 corner2)
    {
        return Vector3.Lerp(corner1, corner2, 0.5f);
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
        pos1 = new Vector3(pos1.x, pos1.y - 0.05f, pos1.z);
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

    //Calculates the other corner
    public static Vector3 GetOtherCornerPosition(Vector3 corner, Vector2 amount, float height, float tileSize, bool invertX, bool invertZ)
    {
        return corner + new Vector3(amount.x * tileSize * (invertX ? 1 : -1), height, amount.y * tileSize * (invertZ ? 1 : -1));
    }

    //Gizmos
    /// Draws gizmos for the area's and available spots to make it easier to place the area's in the inspector.
    public void OnDrawGizmosSelected()
    {
        if(placementLocations != null && !disableGizmos)
            for (int i = 0; i < placementLocations.Length; i++)
            {
                TowerPlacementLocation loc = placementLocations[i];
                if (loc.gridPlacementCorner == null) continue;
                Vector3 otherCorner = GetOtherCornerPosition(loc.gridPlacementCorner.position, loc.tileAmount, loc.height, towerSize, loc.invertX, loc.invertZ);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(GetCenter(loc.gridPlacementCorner.position,otherCorner), GetCornerScale(loc.gridPlacementCorner.position, otherCorner));
                if(towerSize >= 0.1)
                    foreach (Vector3 point in GetAreaPoints(i))
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireCube(point, new Vector3(1,0,1) * towerSize);
                    }
            }
    }

    //information Contructor for each placeable location
    /// This just makes it easier to make it easier to change the values.
    [System.Serializable]
    public class TowerPlacementLocation
    {
        [Header("Position")]
        public Transform gridPlacementCorner;
        public Vector2 tileAmount;
        public float height;
        public bool invertX;
        public bool invertZ;
        public enum PlacementType { Tower, Trap}
    }
}
