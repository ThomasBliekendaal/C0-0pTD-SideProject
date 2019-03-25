using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ScriptableObject for the towers and traps.
[CreateAssetMenu(fileName = "TowerScriptableObject", menuName = "TowerScriptableObject")]
public class TowerScriptableObject : ScriptableObject
{
    public string towerName;
    [TextArea]
    public string towerDescription;
    public Mesh previewMesh;
    public TowerLocations.TowerPlacementLocation.PlacementType towerType;
}
