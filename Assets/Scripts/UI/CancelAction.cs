using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAction : MonoBehaviour
{
    public List<Resources.ResourceCost> resources;
    public Building refBuilding;

    public void OnClick()
    {
        PlayerInventory playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        foreach (Resources.ResourceCost cost in resources) {
            for (int i = 0; i != playerInventory.ownedResources.Count; i++) {
                if (cost.resourceType == playerInventory.ownedResources[i].resourceType) {
                    Resources.ResourceStruct newResource = new Resources.ResourceStruct();
                    newResource.maxResource = playerInventory.ownedResources[i].maxResource;
                    newResource.resourceQuantity = playerInventory.ownedResources[i].resourceQuantity + cost.resourceCost;
                    newResource.resourceType = playerInventory.ownedResources[i].resourceType;
                    playerInventory.ownedResources[i] = newResource;
                }
            }
        }

        if (name == "UnitImageBackground") {
            refBuilding.isCancelled = true;
            return ;
        }

        int index = gameObject.transform.GetSiblingIndex();

        refBuilding.unitsQueue.RemoveAt(index-1);
        Destroy(playerInventory.buildingUnitIconTemplate.transform.parent.GetChild(index).gameObject);
    }
}
