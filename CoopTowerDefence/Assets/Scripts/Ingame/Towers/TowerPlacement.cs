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
    public int rotateAmount;

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
    /// Checks if you are trying to place an object, or trying to check if you are able to place one.
    public void CheckPlacement()
    {
        for (int i = 0; i < towerSelectInputs.Length; i++)
            if (Input.GetButtonDown(towerSelectInputs[i]) && availableTowers.Length > i)
            {
                isPlacing = true;
                currentlyPlacingInput = i;
                previewObject.GetComponent<MeshFilter>().mesh = availableTowers[i].previewMesh;
                previewObject.transform.localScale = availableTowers[i].objectScale;
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
                    int area = 0;
                    bool foundSpot = false;
                    Vector3 spot = new Vector3();
                    Vector3 pos1 = new Vector3();
                    Vector3 pos2 = new Vector3();
                    for (int i = 0; i < towerLocations.placementLocations.Length; i++)
                    {
                        TowerLocations.TowerPlacementLocation loc = towerLocations.placementLocations[i];
                        pos1 = loc.gridPlacementCorner.position;
                        pos2 = TowerLocations.GetOtherCornerPosition(loc.gridPlacementCorner.position,loc.tileAmount,loc.height,towerLocations.towerSize,loc.invertX,loc.invertZ);
                        if (TowerLocations.IsInBetween(hit.point, pos1, pos2))
                        {
                            spot = (availableTowers[currentlyPlacingInput].towerType == TowerLocations.TowerPlacementLocation.PlacementType.Trap) ? towerLocations.GetPlacementPoint(hit.point, i) : towerLocations.StraightDownPoint(hit.point, i);
                            foundSpot = true;
                            area = i;
                            break;
                        }
                    }
                    if (foundSpot)
                    {
                        //Rotation
                        if (Input.GetAxis("Mouse ScrollWheel") != 0)
                        {
                            rotateAmount += (Input.GetAxis("Mouse ScrollWheel") > 0) ? 1 : -1;
                            Debug.Log(rotateAmount);
                        }

                        canPlace = true;
                        //X offset check
                        if (availableTowers[currentlyPlacingInput].tileOffsetX != Vector2.zero)
                            for (int x = Mathf.RoundToInt(availableTowers[currentlyPlacingInput].tileOffsetX.x); x < Mathf.RoundToInt(availableTowers[currentlyPlacingInput].tileOffsetX.y + 1); x++)
                            {
                                Vector3 checkPos = spot + (previewObject.transform.right * towerLocations.towerSize * x);
                                //Z offset check
                                if (availableTowers[currentlyPlacingInput].tileOffsetY != Vector2.zero)
                                    for (int z = Mathf.RoundToInt(availableTowers[currentlyPlacingInput].tileOffsetY.x); z < Mathf.RoundToInt(availableTowers[currentlyPlacingInput].tileOffsetY.y + 1); z++)
                                    {
                                        if (z == 0)
                                            continue;
                                        Vector3 otherCheckPos = checkPos + (previewObject.transform.forward * towerLocations.towerSize * z);
                                        if (Physics.CheckBox(otherCheckPos, Vector3.one * towerLocations.towerSize * 0.1f, Quaternion.identity, towerTrapMask))
                                            canPlace = false;

                                        if (!TowerLocations.IsInBetween(otherCheckPos, pos1, pos2))
                                            canPlace = false;
                                    }

                                if (x == 0)
                                    continue;
                                if (Physics.CheckBox(checkPos, Vector3.one * towerLocations.towerSize * 0.1f, Quaternion.identity, towerTrapMask))
                                    canPlace = false;

                                if (!TowerLocations.IsInBetween(checkPos, pos1, pos2))
                                    canPlace = false;
                            }

                        if (Physics.CheckBox(spot, Vector3.one * towerLocations.towerSize * 0.1f, Quaternion.identity, towerTrapMask))
                            canPlace = false;
                        previewObject.SetActive(true);
                        previewObject.transform.position = spot;
                        previewObject.transform.rotation = Quaternion.identity;
                        previewObject.GetComponent<Renderer>().material = (canPlace) ? nonPlacedMaterial : ubstructedMaterial;
                        previewObject.transform.Rotate(Vector3.up * rotateAmount * 90);
                        if (Input.GetButtonDown("Fire1") && canPlace)
                        {
                            isPlacing = false;
                            GameObject p = PhotonNetwork.Instantiate(availableTowers[currentlyPlacingInput].towerName, spot, Quaternion.identity, 0);
                            p.transform.Rotate(Vector3.up * rotateAmount * 90);
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
