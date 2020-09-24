using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BuildingLayout : MonoBehaviour
{
    public Building buildingToBuild;
    public float buildingHeight;
    public int timeToBuild;
    public LayerMask ignoreLayer1;
    public LayerMask ignoreLayer2;
    public List<Resources.Upgrade> onUseGainedUpgrades;
    public List<Resources.ResourceCost> resourcesCosts;
    PlayerInventory playerInventory;

    InputController inputController;

    bool canBuild;

    public Gatherer buildingGatherer;

    //public Material authorizedMaterial;
    //public Material unauthorizedMaterial;

    void Start()
    {
        inputController = GameObject.Find("GameManager").GetComponent<InputController>();
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        inputController.isBuilding = true;
        canBuild = true;
    }

    void FixedUpdate()
    {
        Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(castPoint, out hit, 100f, ~ignoreLayer1 & ~ignoreLayer2) && hit.collider.tag != "Layout") {
            transform.position = new Vector3(hit.point.x, buildingHeight, hit.point.z);
        }
        Debug.Log("fixed update");
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log("down");
            InstantiateBuilding();
        }
    }

    void InstantiateBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        PointerEventData pointerData = new PointerEventData (EventSystem.current)
        {
            pointerId = -1,
        };

        List<RaycastResult> results = new List<RaycastResult>();

        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, results);

        if (Physics.Raycast(ray, out hit)) {

            if (canBuild) {
                
                Building newBuilding = Instantiate(buildingToBuild);
                if (onUseGainedUpgrades.Count != 0) {
                    playerInventory.AddUpgrades(onUseGainedUpgrades);
                    if (inputController.selectedUnits.Count != 0) {
                        inputController.UpdateUnitUI();
                    }
                }
                RemoveCostFromPlayer();
                newBuilding.InitBuilding(new Vector3(hit.point.x, buildingHeight, hit.point.z), buildingGatherer, resourcesCosts, timeToBuild);
                buildingGatherer.currentAction = Unit.Action.BUILDING;
                buildingGatherer.buildingBeingBuild = newBuilding;
                buildingGatherer.Move(buildingGatherer.agent, newBuilding.transform.position);
                inputController.EnableLeftClick();
                //inputController.isBuilding = false;
                Destroy(gameObject);
            } else {
                Debug.Log("cannot build");
            }
        }
    }

    public void RemoveCostFromPlayer()
    {
        //PlayerInventory playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        foreach (Resources.ResourceCost cost in resourcesCosts) {
            for (int i = 0; i != playerInventory.ownedResources.Count; i++) {
                if (cost.resourceType == playerInventory.ownedResources[i].resourceType) {
                    Resources.ResourceStruct newResource = new Resources.ResourceStruct();
                    newResource.maxResource = playerInventory.ownedResources[i].maxResource;
                    newResource.resourceQuantity = playerInventory.ownedResources[i].resourceQuantity - cost.resourceCost;
                    newResource.resourceType = playerInventory.ownedResources[i].resourceType;
                    playerInventory.ownedResources[i] = newResource;
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("enter");
        gameObject.GetComponent<MeshRenderer>().material = playerInventory.unauthorizedMaterial;
        canBuild = false;
    }

    void OnTriggerExit(Collider collider)
    {
        Debug.Log("exit");
        gameObject.GetComponent<MeshRenderer>().material = playerInventory.authorizedMaterial;
        canBuild = true;
    }
}
