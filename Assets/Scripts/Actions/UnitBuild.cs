using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBuild : Action
{
    public Unit unitToBuild;
    public Building building;

    public PlayerInventory player;
    
    void Start()
    {
        building = this.gameObject.GetComponent<Building>();
    }

    public override void OnClick()
    {

        if (!player) {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        }

        if (!player.CheckIfUnlocked(requiredUpgrades) || building.unitsQueue.Count >= building.queueMaxLength-1) {
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

        foreach (Resources.ResourceCost cost in resourcesCosts) {
            //foreach (Resources.ResourceStruct resourceStruct in player.ownedResources) {
            for (int i = 0; i != player.ownedResources.Count; i++) {
                //if (cost.resourceType == resourceStruct.resourceType) {
                if (cost.resourceType == player.ownedResources[i].resourceType) {
                    Resources.ResourceStruct newResource = new Resources.ResourceStruct();
                    newResource.maxResource = player.ownedResources[i].maxResource;
                    newResource.resourceQuantity = player.ownedResources[i].resourceQuantity - cost.resourceCost;
                    newResource.resourceType = player.ownedResources[i].resourceType;
                    player.ownedResources[i] = newResource;
                }
            }
        }
        building.AddUnitToQueue(unitToBuild, timeToBuild, resourcesCosts, actionSprite);
    }
}
