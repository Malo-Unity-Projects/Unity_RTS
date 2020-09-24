using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CancelBuilding : MonoBehaviour
{
    public List<Resources.ResourceCost> resources;
    public Building refBuilding;
    public PlayerInventory playerInventory;
    public string actionName;
    public string description;
    bool isOverflown;

    public Text nameText;
    public Text descriptionText;

    void Update()
    {
        if (isOverflown) {
            Vector2 anchoredPos = Input.mousePosition / playerInventory.canvas.GetComponent<RectTransform>().localScale.x;

            if (anchoredPos.x + playerInventory.buttonPanel.GetComponent<RectTransform>().rect.width > playerInventory.canvas.GetComponent<RectTransform>().rect.width) {
                anchoredPos.x = playerInventory.canvas.GetComponent<RectTransform>().rect.width - playerInventory.buttonPanel.GetComponent<RectTransform>().rect.width;
            }
            if (anchoredPos.y + playerInventory.buttonPanel.GetComponent<RectTransform>().rect.height > playerInventory.canvas.GetComponent<RectTransform>().rect.height) {
                anchoredPos.y = playerInventory.canvas.GetComponent<RectTransform>().rect.height - playerInventory.buttonPanel.GetComponent<RectTransform>().rect.height;
            }
            anchoredPos.y += 5;
            playerInventory.buttonPanel.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
        }
    }

    public void SetButton()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        nameText = playerInventory.buttonPanel.transform.Find("NameText").GetComponent<Text>();
        descriptionText = playerInventory.buttonPanel.transform.Find("DescriptionText").GetComponent<Text>();
    }

    public void OnClick()
    {
        foreach (Resources.ResourceCost cost in resources) {
            for (int i = 0; i != playerInventory.ownedResources.Count; i++) {
                if (cost.resourceType == playerInventory.ownedResources[i].resourceType) {
                    Resources.ResourceStruct newResource = new Resources.ResourceStruct();
                    newResource.maxResource = playerInventory.ownedResources[i].maxResource;
                    newResource.resourceQuantity = playerInventory.ownedResources[i].resourceQuantity + (int)(cost.resourceCost * 0.7f);
                    newResource.resourceType = playerInventory.ownedResources[i].resourceType;
                    playerInventory.ownedResources[i] = newResource;
                }
            }
        }

        refBuilding.buildingGatherer.currentAction = Unit.Action.IDLE;
        playerInventory.buildingUnitDisplay.SetActive(false);
        playerInventory.buttonPanel.SetActive(false);
        if (refBuilding.isSelected && refBuilding.isOwnUnit) {
            GameObject.Find("GameManager").GetComponent<InputController>().selectedUnits.RemoveAt(0);
        }
        Destroy(refBuilding.gameObject);
        Destroy(gameObject);
    }

    public void OnPointerEnter()
    {
        playerInventory.actionTimeDisplay.SetActive(false);
        playerInventory.buttonPanel.SetActive(true);
        nameText.text = actionName;
        descriptionText.text = description;
        isOverflown = true;
    }

    public void OnPointerExit()
    {
        isOverflown = false;
        playerInventory.buttonPanel.transform.position = new Vector3(playerInventory.canvas.GetComponent<RectTransform>().rect.width, 0, 0);
        playerInventory.buttonPanel.SetActive(false);
        playerInventory.actionTimeDisplay.SetActive(true);
    }
}
