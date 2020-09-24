using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySelectedUnit : MonoBehaviour
{
    public string description;
    bool isOverflown;
    public GameObject entity;
    public GameObject buttonPanel;
    public PlayerInventory playerInventory;

    public Image unitIcon;

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

    public void SetDisplayInfos(GameObject pannelRef, GameObject selectedEntity, string entityDescription)
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        buttonPanel = pannelRef;
        description = entityDescription;
        entity = selectedEntity;
        unitIcon.sprite = selectedEntity.GetComponent<Entity>().entityIcon;
    }

    public void OnPointerEnter()
    {
        isOverflown = true;
        buttonPanel.gameObject.SetActive(true);
        buttonPanel.transform.Find("NameText").GetComponent<Text>().text = entity.GetComponent<Entity>().entityName;
        buttonPanel.transform.Find("DescriptionText").GetComponent<Text>().text = description;
    }

    public void OnPointerExit()
    {
        buttonPanel.transform.position = new Vector3(playerInventory.canvas.GetComponent<RectTransform>().rect.width, 0, 0);
        buttonPanel.gameObject.SetActive(false);
        isOverflown = false;
    }

    public void OnClick()
    {
        InputController inputController = GameObject.Find("GameManager").GetComponent<InputController>();

        buttonPanel.SetActive(false);
        inputController.DeselectUnits();
        inputController.selectedUnits.Add(entity);
        inputController.selectedUnits[0].GetComponent<Entity>().isSelected = false;
        inputController.selectedUnits[0].transform.Find("SelectionCylinder").gameObject.SetActive(true);
        inputController.selectedUnitsIndex = 0;
        inputController.SetUnitUI();
    }
}
