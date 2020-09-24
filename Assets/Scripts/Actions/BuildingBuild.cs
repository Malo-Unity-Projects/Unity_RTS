using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildingBuild : Action
{
    public Building buildingToBuild;
    //public BuildingLayout buildingLayout;
    public GameObject buildingLayout;
    public Gatherer gatherer;
    Unit unit;

    PlayerInventory player;

    void Start()
    {
        unit = this.gameObject.GetComponent<Unit>();
    }

    public override void OnClick()
    {
        if (!player)
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        if (!player.CheckIfUnlocked(requiredUpgrades)) {
            return ;
        }

        foreach (Resources.ResourceCost cost in resourcesCosts) {
            foreach (Resources.ResourceStruct resourceStruct in player.ownedResources) {
                if (cost.resourceType == resourceStruct.resourceType && cost.resourceCost > resourceStruct.resourceQuantity) {
                    Debug.Log("Too poor");
                    return ;   
                }
            }
        }

        GameObject newLayout = Instantiate(buildingLayout);
        newLayout.GetComponent<BuildingLayout>().buildingToBuild = buildingToBuild;
        newLayout.GetComponent<BuildingLayout>().buildingHeight = buildingToBuild.heightSpawn;
        newLayout.GetComponent<BuildingLayout>().onUseGainedUpgrades = onUseGainedUpgrades;
        newLayout.GetComponent<BuildingLayout>().timeToBuild = timeToBuild;
        newLayout.GetComponent<BuildingLayout>().resourcesCosts = resourcesCosts;
        newLayout.GetComponent<BuildingLayout>().buildingGatherer = gatherer;
        //newLayout.GetComponent<BuildingLayout>().authorizedMaterial = new Material(playerInventory.authorizedMaterial);
        //newLayout.GetComponent<BuildingLayout>().unauthorizedMaterial = new Material(playerInventory.unauthorizedMaterial);
        
        GameObject.Find("GameManager").GetComponent<InputController>().isBuilding = true;
    }
}