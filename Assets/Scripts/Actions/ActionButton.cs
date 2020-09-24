using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionButton : MonoBehaviour
{
    bool isOverflown;
    public PlayerInventory playerInventory;

    string description;
    string actionName;
    int time;
    public List<Resources.ResourceCost> resourcesCosts;

    public Text descriptionText;
    public Text nameText;
    public Text timeText;
    public Image actionImage;

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

    public void SetButtonAction(Action refAction)
    {
        EventTrigger.Entry onPointerEnterEntry = new EventTrigger.Entry();
        onPointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        onPointerEnterEntry.callback.AddListener((data) => OnPointerEnter());
        gameObject.GetComponent<EventTrigger>().triggers.Add(onPointerEnterEntry);
        
        EventTrigger.Entry onPointerExitEntry = new EventTrigger.Entry();
        onPointerExitEntry.eventID = EventTriggerType.PointerExit;
        onPointerExitEntry.callback.AddListener((data) => OnPointerExit());
        gameObject.GetComponent<EventTrigger>().triggers.Add(onPointerExitEntry);

        description = refAction.description;
        resourcesCosts = refAction.resourcesCosts;
        actionName = refAction.actionName;
        actionImage.sprite = refAction.actionSprite;
        time = refAction.timeToBuild;
        if (!playerInventory) {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            nameText = playerInventory.buttonPanel.transform.Find("NameText").GetComponent<Text>();
            descriptionText = playerInventory.buttonPanel.transform.Find("DescriptionText").GetComponent<Text>();
            timeText = playerInventory.buttonPanel.transform.Find("TimeDisplay").Find("TimeText").GetComponent<Text>();
        }
    }

    public void OnPointerEnter()
    {
        string costDisplay = "";
        PlayerInventory.ResourceIcon resource;

        playerInventory.buttonPanel.SetActive(true);
        nameText.text = actionName;
        foreach (Resources.ResourceCost cost in resourcesCosts) {
            GameObject newCostDisplay = Instantiate(playerInventory.costDisplayTemplate);
            newCostDisplay.transform.SetParent(playerInventory.costDisplayTemplate.transform.parent);

            resource = playerInventory.resourcesIcons.Find(x => x.resourceType == cost.resourceType);
            
            newCostDisplay.transform.GetChild(0).GetComponent<Image>().sprite = resource.resourceIcon;
            newCostDisplay.transform.GetChild(1).GetComponent<Text>().text = "" + cost.resourceCost;
            newCostDisplay.SetActive(true);
        }
        timeText.text = "" + time;
        playerInventory.buttonPanel.transform.Find("TimeDisplay").gameObject.SetActive(true);
        descriptionText.text = description;
        //descriptionTextMesh.SetText(description);
        isOverflown = true;
    }

    public void OnPointerExit()
    {
        foreach (Transform child in playerInventory.costDisplayTemplate.transform.parent) {
            if (child.name.EndsWith("(Clone)")) {
                Destroy(child.gameObject);
            }
        }
        isOverflown = false;
        playerInventory.buttonPanel.transform.Find("TimeDisplay").gameObject.SetActive(false);
        playerInventory.buttonPanel.transform.position = new Vector3(playerInventory.canvas.GetComponent<RectTransform>().rect.width, 0, 0);
        playerInventory.buttonPanel.SetActive(false);
    }
}
