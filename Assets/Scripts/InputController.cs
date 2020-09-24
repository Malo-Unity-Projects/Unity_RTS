using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    /* Units Selection */
    public List<GameObject> selectedUnits;
    public int selectedUnitsIndex = 0;

    /* Units Selection UI */
    public RectTransform selectionBox;
    bool isSelecting;
    Vector2 mouseStartPos;

    float clicked = 0f;
    float clickTime = 0;
    float clickDelay = 0.5f;

    public GameObject selectedUnitUI;

    /* Building Unit UI */
    GameObject buildingUnitDisplay;
    public GameObject actionButtonTemplate;
    public Text nameText;
    public Image unitImage;

    /* Selected Unit UI */
    public GameObject selectedUnitsContent;

    public Image selectedUnitImage;
    public Text selectedUnitName;
    public Text selectedUnitHealth;

    public bool isBuilding;

    PlayerInventory playerInventory;
    UnitsController unitsController;

    public LayerMask ignoreLayer;

    void Awake()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        unitsController = GameObject.Find("GameManager").GetComponent<UnitsController>();
        buildingUnitDisplay = playerInventory.buildingUnitDisplay;
        selectedUnits = new List<GameObject>();
        selectedUnitImage.transform.parent.GetComponent<DisplaySelectedUnit>().buttonPanel = playerInventory.buttonPanel;
        selectedUnitImage.transform.parent.GetComponent<DisplaySelectedUnit>().playerInventory = playerInventory;
        //canBuild = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            float scaleFactor = playerInventory.canvas.scaleFactor;
            mouseStartPos = new Vector2(Input.mousePosition.x/scaleFactor, Input.mousePosition.y/scaleFactor);
        }

        if (Time.time - clickTime >= clickDelay) {
            clicked = 0;
        }

        if (Input.GetMouseButtonDown(1)) {
            ManageUnitsMovement();
        }

        if (Input.GetMouseButtonUp(0)) {
            ReleaseSelectionBox();
        }

        if (Input.GetMouseButton(0)) {
            UpdateSelectionBox(Input.mousePosition);
        }
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ChangeSelectedUnit();
        }
    }

    void ManageUnitsMovement()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ~ignoreLayer)) {
            List<Vector3> targetPositionList = GetPositionsList(hit.point, selectedUnits);
            //List<Vector3> targetPositionList = GetPositionListAround(hit.point, 10f, 5);
            int targetPositionIndex = 0;

            Debug.Log("got list");

            foreach (GameObject selectedUnit in selectedUnits) {
                if (selectedUnit && selectedUnit.GetComponent<Entity>() && selectedUnit.GetComponent<Entity>().isOwnUnit) {
                    selectedUnit.GetComponent<Entity>().RightClick(hit.point, hit);
                    //selectedUnit.GetComponent<Entity>().RightClick(targetPositionList[targetPositionIndex], hit);
                    Debug.Log("position used: " + targetPositionList[targetPositionIndex].x + ", " + targetPositionList[targetPositionIndex].z);
                    targetPositionIndex++;
                    //targetPositionIndex = (targetPositionIndex + 1) % targetPositionList.Count;
                }
            }
        }
    }

    List<Vector3> GetPositionsList(Vector3 startPos, List<GameObject> selectedUnits)
    {
        List<Vector3> positionList = new List<Vector3>();
        int unitsPerLine = 0;
        float xPos = startPos.x;
        float zPos = startPos.z;

        if ((float)selectedUnits.Count/3 > 0.5f) {
            unitsPerLine = (selectedUnits.Count/3)+1;
        } else {
            unitsPerLine = (selectedUnits.Count/3)-1;
        }
        for (int i = 0; i != selectedUnits.Count; i++) {
            positionList.Add(new Vector3(xPos, startPos.y, zPos));
            if (i < selectedUnits.Count-1) {
                xPos += selectedUnits[i].GetComponent<BoxCollider>().size.x/2 + selectedUnits[i+1].GetComponent<BoxCollider>().size.x/2 + 2;
            } else {
                    Debug.Log("end hor");
                xPos += selectedUnits[i].GetComponent<BoxCollider>().size.x/2 + 2;
            }
            if (i % unitsPerLine == 0) {
                Debug.Log("new line, " + i);
                xPos = startPos.x;
                if (i < selectedUnits.Count-1) {
                    zPos += selectedUnits[i].GetComponent<BoxCollider>().size.z/2 + selectedUnits[i+1].GetComponent<BoxCollider>().size.z/2 + 2;
                } else {
                    Debug.Log("end ver");
                    zPos += selectedUnits[i].GetComponent<BoxCollider>().size.z/2 + 2;
                }
            }
            /*
            linePos++;
            if (i % unitsPerLine == 0) {
                linePos = 0;
                line++;
            }
            */
        }
        return (positionList);
    }

    List<Vector3> GetPositionListAround(Vector3 startPos, float distance, int positionCount)
    {
        List<Vector3> positionList = new List<Vector3>();

        for (int i = 0; i < positionCount; i++) {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPos + dir * distance;
            positionList.Add(position);
        }
        return (positionList);
    }

    Vector3 ApplyRotationToVector(Vector3 vector, float angle)
    {
        return (Quaternion.Euler(0, 0, angle) * vector);
    }

    void ChangeSelectedUnit()
    {
        if (selectedUnits.Count == 0) {
            return ;
        }
        for (int i = selectedUnitsIndex+1; i != selectedUnits.Count; i++) {
            if (selectedUnits[i].GetComponent<Entity>().entityName != selectedUnits[selectedUnitsIndex].GetComponent<Entity>().entityName) {
                selectedUnitsIndex = i;
                ResetUnitUI();
                SetUnitUI();
                return ;
           }
        }
        if (selectedUnits[0].GetComponent<Entity>().entityName != selectedUnits[selectedUnitsIndex].GetComponent<Entity>().entityName) {
            selectedUnitsIndex = 0;
            ResetUnitUI();
            SetUnitUI();    
        }
    }

    void UpdateSelectionBox(Vector2 mousePos)
    {
        float scaleFactor = playerInventory.canvas.scaleFactor;

        if (!selectionBox.gameObject.activeInHierarchy) {
            selectionBox.gameObject.SetActive(true);
        }

        float width = (mousePos.x/scaleFactor) - mouseStartPos.x;
        float height = (mousePos.y/scaleFactor) - mouseStartPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = mouseStartPos + new Vector2(width/2, height/2);
    }

    void ReleaseSelectionBox()
    {
        List<GameObject> unitsInSelection = new List<GameObject>();

        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        //foreach (GameObject unit in playerInventory.ownedUnits) {
        foreach (Unit unit in unitsController.playerUnits) {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y) {
                unitsInSelection.Add(unit.gameObject);
            }
        }
        if (unitsInSelection.Count != 0) {
            DeselectUnits();
            foreach (GameObject unit in unitsInSelection) {
                unit.GetComponent<Unit>().isSelected = true;
                unit.transform.Find("SelectionCylinder").gameObject.SetActive(true);
                selectedUnits.Add(unit);
            }
        }
        selectedUnits = selectedUnits.OrderBy(go=>go.GetComponent<Entity>().entityName).ToList();
        
        if (selectedUnits.Count != 0) {
            SetUnitUI();
        }
        if (min == max) {
            if (!isBuilding) {
                LeftClick();
            }
        }
    }

    public void EnableLeftClick()
    {
        StartCoroutine(WaitForLayoutPlacement());
    }

    IEnumerator WaitForLayoutPlacement()
    {
        yield return new WaitForSeconds(0.2f);
        isBuilding = false;
    }

    void LeftClick()
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

        if (Physics.Raycast(ray, out hit) && results.Count == 0) {

            clicked++;

            buildingUnitDisplay.gameObject.SetActive(false);
            //Debug.Log("tag: " + hit.collider.tag);

            if (clicked == 1) {
                /* simple clic */
                clickTime = Time.time;
                CheckHitTag(hit);
                Debug.Log("clicked 1");
            }
            if (clicked > 1 && Time.time - clickTime < clickDelay) {
                clicked = 0;
                clickTime = 0;

                if (hit.transform.tag == "Ground" || hit.transform.tag == "Obstacle") {
                    DeselectUnits();
                } else {

                    if (selectedUnits.Count != 0) {
                    
                        Collider[] hitColliders = Physics.OverlapBox(hit.point, new Vector3(10, 10, 10));

                        Entity entityRef = selectedUnits[selectedUnitsIndex].GetComponent<Entity>();

                        DeselectUnits();
                        for (int i = 0; i != hitColliders.Length; i++) {
                            if (hitColliders[i].transform.GetComponent<Entity>() && hitColliders[i].transform.GetComponent<Entity>().entityName == entityRef.entityName) {
                                hitColliders[i].transform.GetComponent<Entity>().isSelected = true;
                                hitColliders[i].transform.Find("SelectionCylinder").gameObject.SetActive(true);
                                selectedUnits.Add(hitColliders[i].gameObject);
                            }
                        }
                        ResetUnitUI();
                        SetUnitUI();
                    } else {
                        CheckHitTag(hit);
                    }
                }
                Debug.Log("double clicked");

            } else if (clicked > 2 || Time.time - clickTime > 1) {
                clicked = 0;
                CheckHitTag(hit);
                Debug.Log("clicked 2");
                return ;
            } else {
                Debug.Log("something else");
                CheckHitTag(hit);
            }

        }
    }

    void CheckHitTag(RaycastHit hit)
    {
        switch (hit.collider.tag) {
            case ("Ground"):
                DeselectUnits();
                break;
            case ("Unit"):
            case ("Building"):

                foreach (GameObject selectedUnit in selectedUnits) {
                    selectedUnit.GetComponent<Entity>().isSelected = false;
                    selectedUnit.transform.Find("SelectionCylinder").gameObject.SetActive(false);
                    if (selectedUnit.GetComponent<Building>()) {
                        selectedUnit.GetComponent<Building>().spawnWayPoint.gameObject.SetActive(false);
                        buildingUnitDisplay.SetActive(false);
                    }
                }
                selectedUnits.Clear();
                GameObject newUnit = hit.collider.gameObject;
                if (newUnit.GetComponent<Entity>().isOwnUnit) {
                    newUnit.GetComponent<Entity>().isSelected = true;
                    if (newUnit.GetComponent<Building>()) {
                        if (newUnit.GetComponent<Building>().isCompleted) {
                            newUnit.GetComponent<Building>().spawnWayPoint.gameObject.SetActive(true);
                        }
                        if (newUnit.GetComponent<Building>().isBusy) {
                            buildingUnitDisplay.SetActive(true);
                            newUnit.GetComponent<Building>().SetUnitBuildingDisplay();
                        } else if (!newUnit.GetComponent<Building>().isCompleted) {
                            buildingUnitDisplay.SetActive(true);
                            newUnit.GetComponent<Building>().SetBuildingBuildingDisplay();
                        }
                    }
                }
                newUnit.transform.Find("SelectionCylinder").gameObject.SetActive(true);
                selectedUnits.Add(newUnit);
                if (newUnit.GetComponent<Entity>().isOwnUnit) {
                    ResetUnitUI();
                    SetUnitUI();
                } else {
                    ResetUnitUI();
                }
                break;
        }
    }

    public void SetUnitUI()
    {
        if (actionButtonTemplate.transform.parent.childCount != 1)
            return ;

        nameText.text = selectedUnits[selectedUnitsIndex].GetComponent<Entity>().entityName;

        if (selectedUnits[selectedUnitsIndex].GetComponent<Building>() && !selectedUnits[selectedUnitsIndex].GetComponent<Building>().isCompleted) {
            GameObject cancelButton = Instantiate(playerInventory.cancelButtonTemplate);
            cancelButton.transform.SetParent(actionButtonTemplate.transform.parent);
            cancelButton.GetComponent<CancelBuilding>().resources = selectedUnits[selectedUnitsIndex].GetComponent<Building>().resourcesCost;
            cancelButton.GetComponent<CancelBuilding>().refBuilding = selectedUnits[selectedUnitsIndex].GetComponent<Building>();
            return ;
        }

        foreach (Action action in selectedUnits[selectedUnitsIndex].GetComponent<Entity>().actions) {
            GameObject actionButton = Instantiate(actionButtonTemplate);
            actionButton.transform.SetParent(actionButtonTemplate.transform.parent);
            actionButton.GetComponent<Button>().onClick.AddListener(action.OnClick);
            actionButton.GetComponent<ActionButton>().SetButtonAction(action);

            if (action.gameObject.GetComponent<BuildingBuild>()) {
                action.gameObject.GetComponent<BuildingBuild>().gatherer = selectedUnits[selectedUnitsIndex].GetComponent<Gatherer>();
            }

            if (!playerInventory.CheckIfUnlocked(action.requiredUpgrades)) {
                actionButton.transform.Find("LockedImage").gameObject.SetActive(true);
            } else {
                actionButton.transform.Find("LockedImage").gameObject.SetActive(false);
            }
            actionButton.gameObject.SetActive(true);
        }

        if (selectedUnits.Count == 1) {

            selectedUnitName.transform.parent.gameObject.SetActive(true);
            selectedUnitName.text = selectedUnits[0].GetComponent<Entity>().entityName;
            selectedUnitHealth.text = selectedUnits[0].GetComponent<Entity>().currenHp + " / " + selectedUnits[0].GetComponent<Entity>().maxHp;
            selectedUnitImage.sprite = selectedUnits[0].GetComponent<Entity>().entityIcon;
            selectedUnitImage.transform.parent.GetComponent<DisplaySelectedUnit>().entity = selectedUnits[0];
            selectedUnitImage.transform.parent.GetComponent<DisplaySelectedUnit>().description = selectedUnits[0].GetComponent<Entity>().description;
            
        } else {

            selectedUnitsContent.gameObject.SetActive(true);

            foreach (GameObject entity in selectedUnits) {
                Debug.Log("unit before: " + entity.GetComponent<Entity>().entityName + " " + entity.name);
            }

            //selectedUnits.Sort();
            
            foreach (GameObject entity in selectedUnits) {
                Debug.Log("unit after: " + entity.GetComponent<Entity>().entityName + " " + entity.name);
            }
            
            foreach (GameObject unit in selectedUnits) {
                GameObject newSelectedUnitDisplay = Instantiate(selectedUnitsContent.transform.GetChild(0).gameObject);

                newSelectedUnitDisplay.GetComponent<DisplaySelectedUnit>().SetDisplayInfos(playerInventory.buttonPanel, unit, unit.GetComponent<Entity>().description);
                newSelectedUnitDisplay.transform.SetParent(selectedUnitsContent.transform);
                newSelectedUnitDisplay.SetActive(true);
            }
        }
    }

    public void UpdateUnitUI()
    {
        int actionIndex = -1;

        for (int i = 0; i != actionButtonTemplate.transform.parent.childCount; i++) {
            if (actionButtonTemplate.transform.parent.GetChild(i).name.EndsWith("(Clone)")) {
                actionIndex++;
            }
            if (actionButtonTemplate.transform.parent.GetChild(i).name.EndsWith("(Clone)") && playerInventory.CheckIfUnlocked(selectedUnits[selectedUnitsIndex].GetComponent<Entity>().actions[actionIndex].requiredUpgrades)) {
                actionButtonTemplate.transform.parent.GetChild(i).transform.Find("LockedImage").gameObject.SetActive(false);
            }
        }
    }

    public void ResetUnitUI()
    {
        nameText.text = "";

        while (actionButtonTemplate.transform.parent.childCount != 1) {
            GameObject save = actionButtonTemplate.transform.parent.GetChild(1).gameObject;
            save.transform.SetParent(null);
            Destroy(save);
        }
        
        while (selectedUnitsContent.transform.childCount != 1) {
            GameObject save = selectedUnitsContent.transform.GetChild(1).gameObject;
            save.transform.SetParent(null);
            Destroy(save);
        }
        selectedUnitsContent.gameObject.SetActive(false);
        selectedUnitName.transform.parent.gameObject.SetActive(false);
    }

    public void DeselectUnits()
    {
        foreach (GameObject unit in selectedUnits) {
            if (unit.GetComponent<Building>()) {
                unit.GetComponent<Building>().spawnWayPoint.gameObject.SetActive(false);
            }
            unit.transform.Find("SelectionCylinder").gameObject.SetActive(false);
            unit.GetComponent<Entity>().isSelected = false;
        }
        ResetUnitUI();
        buildingUnitDisplay.SetActive(false);
        selectedUnits.Clear();
    }
}