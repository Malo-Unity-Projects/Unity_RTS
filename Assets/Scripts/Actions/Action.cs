using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Action : MonoBehaviour
{
    public string actionName;
    public int timeToBuild;
    public List<Resources.ResourceCost> resourcesCosts;
    public List<Resources.Upgrade> requiredUpgrades;
    public List<Resources.Upgrade> onUseGainedUpgrades;
    public string description;

    public bool isOverflown = true;

    public Text nameText;    
    public Text descriptionText;
    public Sprite actionSprite;

    public PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
    }

    void Update()
    {
    }

    public virtual void OnClick()
    {
    }

    public void SetButtonAction(GameObject buttonTemplate, Action refAction)
    {
        GameObject actionButton = Instantiate(buttonTemplate);
        actionButton.transform.SetParent(buttonTemplate.transform.parent);
        actionButton.GetComponent<Button>().onClick.AddListener(OnClick);
        
        EventTrigger.Entry onPointerEnterEntry = new EventTrigger.Entry();
        onPointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        onPointerEnterEntry.callback.AddListener((data) => OnPointerEnter());
        actionButton.GetComponent<EventTrigger>().triggers.Add(onPointerEnterEntry);
        
        EventTrigger.Entry onPointerExitEntry = new EventTrigger.Entry();
        onPointerExitEntry.eventID = EventTriggerType.PointerExit;
        onPointerExitEntry.callback.AddListener((data) => OnPointerExit());
        actionButton.GetComponent<EventTrigger>().triggers.Add(onPointerExitEntry);

        description = refAction.description;
        actionName = refAction.actionName;
        actionButton.SetActive(true);
        if (!playerInventory) {
            playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
            nameText = playerInventory.buttonPanel.transform.Find("NameText").GetComponent<Text>();
            descriptionText = playerInventory.buttonPanel.transform.Find("DescriptionText").GetComponent<Text>();
        }
    }

    public void OnPointerEnter()
    {
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
        descriptionText.text = description;
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
        playerInventory.buttonPanel.transform.position = new Vector3(playerInventory.canvas.GetComponent<RectTransform>().rect.width, 0, 0);
        playerInventory.buttonPanel.SetActive(false);
    }
}
