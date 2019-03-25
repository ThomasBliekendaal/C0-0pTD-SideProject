using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The code for placing the towers
public class TowerPlacement : Photon.MonoBehaviour
{
    bool isPlacing;
    public Transform cam;
    public float placementRange;
    public Material nonPlacedMaterial;
    public Material ubstructedMaterial;
    public GameObject previewObject;
    public string[] towerSelectInputs;
    public TowerScriptableObject[] availableTowers;
    int currentlyPlacingInput;
    bool canPlace;
    public string managerTag;
    TowerLocations towerLocations;
    public LayerMask towerTrapMask;

    //The start void
    /// It checks if it can find the manager with the Towerlocations.
    public void Start()
    {
        if (photonView.isMine)
        {
            towerLocations = GameObject.FindWithTag(managerTag).GetComponent<TowerLocations>();
            if (towerLocations == null)
                Debug.LogError("You are missing the refrence to *TowerLocations*");
        }
    }

    //Update void
    /// Checks if the player is yours.
    public void Update()
    {
        if (photonView.isMine)
        {
            CheckPlacement();
        }
    }

    //PlacementCheck
    /// 
    public void CheckPlacement()
    {
        for (int i = 0; i < towerSelectInputs.Length; i++)
            if (Input.GetButtonDown(towerSelectInputs[i]) && availableTowers.Length > i)
            {
                isPlacing = true;
                currentlyPlacingInput = i;
                previewObject.GetComponent<MeshFilter>().mesh = availableTowers[i].previewMesh;
                break;
            }
        if (isPlacing)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                isPlacing = false;
                previewObject.SetActive(false);
            }
            else
            {
                RaycastHit hit = new RaycastHit();
                Debug.DrawRay(cam.transform.position, cam.transform.forward * placementRange);
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, placementRange))
                {
                    bool foundSpot = false;
                    Vector3 spot = new Vector3();
                    for (int i = 0; i < towerLocations.placementLocations.Length; i++)
                    {
                        Vector3 pos1 = towerLocations.placementLocations[i].position1_green + towerLocations.placementLocations[i].centerOffset;
                        Vector3 pos2 = towerLocations.placementLocations[i].position2_yellow + towerLocations.placementLocations[i].centerOffset;
                        if (TowerLocations.IsInBetween(hit.point, pos1, pos2) && towerLocations.placementLocations[i].placementType == availableTowers[currentlyPlacingInput].towerType)
                        {
                            spot = (availableTowers[currentlyPlacingInput].towerType == TowerLocations.TowerPlacementLocation.PlacementType.Trap) ? towerLocations.GetPlacementPoint(hit.point, i) : towerLocations.StraightDownPoint(hit.point, i); ;
                            foundSpot = true;
                            break;
                        }
                    }
                    if (foundSpot)
                    {
                        previewObject.SetActive(true);
                        previewObject.transform.position = spot;
                        previewObject.transform.rotation = Quaternion.identity;
                        canPlace = !Physics.CheckBox(spot, Vector3.one * towerLocations.towerSize * 0.4f, Quaternion.identity, towerTrapMask);
                        previewObject.GetComponent<Renderer>().material = (canPlace) ? nonPlacedMaterial : ubstructedMaterial;
                        if (Input.GetButtonDown("Fire1") && canPlace)
                        {
                            isPlacing = false;
                            PhotonNetwork.Instantiate(availableTowers[currentlyPlacingInput].towerName, spot, Quaternion.identity, 0);
                            previewObject.SetActive(false);
                        }
                    }
                    else
                        previewObject.SetActive(false);
                }
                else
                {
                    previewObject.SetActive(false);
                }
            }
        }
    }
}
