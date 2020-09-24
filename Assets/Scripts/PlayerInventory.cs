using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public int nbUnits;
    public int maxUnits;
    public List<Resources.ResourceStruct> ownedResources;
    //public List<GameObject> ownedUnits;

    public List<Resources.Upgrade> ownedUpgrades;

    public enum Race { SOLARIAN, GALAID };
    public Race chosenRace;

    public List<GameObject> drops;

    /* Display */
    public Canvas canvas;
    public GameObject buildingUnitDisplay;
    public GameObject buildingUnitIconTemplate;
    public Slider progressSlider;
    public Text unitNameText;
    public Image unitImageDisplay;
    public Text nbUnitsText;
    public GameObject cancelButtonTemplate;

    public GameObject buttonPanel;
    public GameObject actionTimeDisplay;
    public GameObject costDisplayTemplate;
    public GameObject resourceDisplayTemplate;
    Dictionary<Resources.ResourceType, GameObject> resourcesDisplay;

    [System.Serializable]
    public struct ResourceIcon {
        public Resources.ResourceType resourceType;
        public Sprite resourceIcon;
    }
    public List<ResourceIcon> resourcesIcons;
    public Material authorizedMaterial;
    public Material unauthorizedMaterial;

    void Awake()
    {
        /* Selon la race, différentes ressources à utiliser */
        switch (chosenRace) {
            case (Race.SOLARIAN):
                break;
            case (Race.GALAID):
                break;
        }
        drops = new List<GameObject>();
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        for (int i = 0; i != buildings.Length; i++) {
            if (buildings[i].GetComponent<Building>().isOwnUnit && buildings[i].GetComponent<Building>().isDrop) {
                drops.Add(buildings[i]);
            }
        }

        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");

        foreach (GameObject unit in units) {
            if (unit.GetComponent<Unit>().isOwnUnit) {
                //ownedUnits.Add(unit);
                nbUnits++;
            }
        }
        resourcesDisplay = new Dictionary<Resources.ResourceType, GameObject>();
        InitDisplayResources();

        cancelButtonTemplate.GetComponent<CancelBuilding>().SetButton();
    }

    void Update()
    {
        UpdateDisplayResources();
        nbUnitsText.text = nbUnits + " / " + maxUnits;
    }

    void InitDisplayResources()
    {
        ResourceIcon icon;

        foreach (Resources.ResourceStruct resource in ownedResources) {
            GameObject newResourceDisplay = Instantiate(resourceDisplayTemplate);
            newResourceDisplay.transform.SetParent(resourceDisplayTemplate.transform.parent);
            icon = resourcesIcons.Find(x => x.resourceType == resource.resourceType);
            newResourceDisplay.GetComponent<DisplayUI>().SetResourceUI(resource, icon.resourceIcon);
            newResourceDisplay.gameObject.SetActive(true);
            resourcesDisplay.Add(resource.resourceType, newResourceDisplay);
        }
    }

    void UpdateDisplayResources()
    {
        ResourceIcon icon;

        foreach (Resources.ResourceStruct resource in ownedResources) {
            foreach (KeyValuePair<Resources.ResourceType, GameObject> pair in resourcesDisplay) {
                if (pair.Key == resource.resourceType) {
                    icon = resourcesIcons.Find(x => x.resourceType == resource.resourceType);
                    pair.Value.GetComponent<DisplayUI>().SetResourceUI(resource, icon.resourceIcon);
                }
            }
        }
    }

    public bool CheckIfUnlocked(List<Resources.Upgrade> upgradesRequired)
    {
        bool isUnlocked = true;

        foreach (Resources.Upgrade upgrade in upgradesRequired) {
            isUnlocked = false;
            foreach (Resources.Upgrade ownedUpgrade in ownedUpgrades) {
                if (ownedUpgrade == upgrade) {
                    isUnlocked = true;
                }                
            }
            if (!isUnlocked) {
                return (false);
            }
        }
        return (isUnlocked);
    }

    public void AddUpgrades(List<Resources.Upgrade> upgradesToAdd)
    {
        bool isAlreadyOwned;

        foreach (Resources.Upgrade upgrade in upgradesToAdd) {
            isAlreadyOwned = false;

            foreach (Resources.Upgrade ownedUpgrade in ownedUpgrades) {
                if (upgrade == ownedUpgrade) {
                    isAlreadyOwned = true;
                }
            }

            if (!isAlreadyOwned) {
                ownedUpgrades.Add(upgrade);
            }
        }
    }

    public void RemoveUpgrade()
    {}
}
