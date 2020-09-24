using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : Entity
{
    public enum BuildingType { HQ, BARRACKS, FACTORY };
    public BuildingType buildingType;

    public bool isBusy;
    public bool isCompleted;
    public bool isDrop;

    PlayerInventory playerInventory;

    GameObject spawnTarget;
    enum TargetType { UNIT, BUILDING, RESOURCE };
    TargetType targetType;
    public Transform spawnPoint;
    public GameObject spawnWayPoint;
    public float heightSpawn;

    /* building display */
    GameObject buildingUnitDisplay;
    GameObject buildingUnitIconTemplate;
    Slider progressSlider;
    Text unitNameText;
    string unitToBuildName;
    Image unitImageDisplay;
    CancelAction cancelButtonSave;
    Sprite unitSprite;

    /* building variables */
    float timePassed;
    int timeToBuild;
    public bool isCancelled;
    public struct UnitQueue {
        public Unit unitToBuild;
        public int timeToBuild;
        public CancelAction cancelSave;
        public Sprite unitSprite;
    }
    public List<UnitQueue> unitsQueue;
    public int queueMaxLength;

    public Gatherer buildingGatherer;
    public float buildingTimeMax;
    public float buildingTimePassed;
    public List<Resources.ResourceCost> resourcesCost;

    void Start()
    {
        InitDisplay();
        unitsQueue = new List<UnitQueue>();

        isCancelled = false;

        foreach (Action action in actions) {
            if (action.gameObject.GetComponent<UnitBuild>()) {
                action.gameObject.GetComponent<UnitBuild>().building = this;
            }
        }
    }

    void Update()
    {
        KeepCanvasAligned();

        if (unitsQueue.Count != 0 && !isBusy && isCompleted) {
            int time = unitsQueue[0].timeToBuild;
            Unit unit = unitsQueue[0].unitToBuild;
            CancelAction cancelSave = unitsQueue[0].cancelSave;
            Sprite unitSprite = unitsQueue[0].unitSprite;
            unitsQueue.RemoveAt(0);
            if (buildingUnitIconTemplate.transform.parent.childCount > 1) {
                Destroy(buildingUnitIconTemplate.transform.parent.GetChild(1).gameObject);
            }
            StartBuildingUnit(unit, time, cancelSave, unitSprite);
        }
    }

    public void InitBuilding(Vector3 spawnPosition, Gatherer buildingGatherer, List<Resources.ResourceCost> resourcesCosts, int timeMax)
    {
        isOwnUnit = true;
        isCompleted = false;
        transform.position = spawnPosition;
        buildingTimeMax = timeMax;
        this.buildingGatherer = buildingGatherer;
        resourcesCost = resourcesCosts;
        InitDisplay();
        StartCoroutine(BuildBuilding());
    }

    public void InitDisplay()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        buildingUnitDisplay = playerInventory.buildingUnitDisplay;
        progressSlider = playerInventory.progressSlider;
        unitNameText = playerInventory.unitNameText;
        unitImageDisplay = playerInventory.unitImageDisplay;
        buildingUnitIconTemplate = playerInventory.buildingUnitIconTemplate;
    }

    public void AddUnitToQueue(Unit unitToBuild, int timeToBuild, List<Resources.ResourceCost> resourcesCosts, Sprite unitSprite)
    {
        UnitQueue unitQueue;
        unitQueue.unitToBuild = unitToBuild;
        unitQueue.timeToBuild = timeToBuild;
        unitQueue.unitSprite = unitSprite;
        
        GameObject newDisplay = Instantiate(buildingUnitIconTemplate);
        newDisplay.transform.SetParent(buildingUnitIconTemplate.transform.parent);
        //newDisplay.GetComponent<Image>().sprite = unitSprite;
        newDisplay.transform.Find("UnitQueueDisplay").GetComponent<Image>().sprite = unitSprite;
        newDisplay.GetComponent<CancelAction>().refBuilding = this;
        newDisplay.GetComponent<CancelAction>().resources = resourcesCosts;

        newDisplay.gameObject.SetActive(true);
        unitQueue.cancelSave = newDisplay.GetComponent<CancelAction>();

        unitsQueue.Add(unitQueue);
    }

    public void StartBuildingUnit(Unit unitToBuild, int timeToBuild, CancelAction cancelSave, Sprite refUnitSprite)
    {
        isBusy = true;
        timePassed = 0;
        this.timeToBuild = timeToBuild;
        unitToBuildName = unitToBuild.entityName;
        cancelButtonSave = cancelSave;
        unitSprite = refUnitSprite;
        StartCoroutine(BuildUnit(unitToBuild, true));
    }

    IEnumerator BuildUnit(Unit unitToBuild, bool isPlayer)
    {
        SetUnitBuildingDisplay();

        while (timePassed < (float)timeToBuild) {
            yield return new WaitForSeconds(0.2f);
            timePassed += 0.2f;
            if (isSelected) {
                progressSlider.value = timePassed;
            }
            if (isCancelled) {
                isCancelled = false;
                isBusy = false;
                if (unitsQueue.Count == 0) {
                    buildingUnitDisplay.gameObject.SetActive(false);
                }
                yield break;
            }
        }
        if (isCancelled) {
            yield break;
        }
        Debug.Log("name: " + unitToBuild.entityName);
        SpawnUnit(unitToBuild, isPlayer);
    }

    public IEnumerator BuildBuilding()
    {
        while (buildingTimePassed < buildingTimeMax) {

            yield return new WaitForSeconds(0.2f);

            if (!this) {
                yield break;
            }
            if (buildingGatherer && buildingGatherer.isBuilding && Vector3.Distance(transform.position, buildingGatherer.transform.position) < buildingGatherer.buildingDistance) {
                buildingTimePassed += 0.2f;
            }

            if (isSelected) {
                progressSlider.value = buildingTimePassed;
            }
        }
        isCompleted = true;
        buildingGatherer.isBuilding = false;
        buildingGatherer = null;
        buildingUnitDisplay.gameObject.SetActive(false);
        if (isSelected) {
            GameObject cancelSave = GameObject.Find("GameManager").GetComponent<InputController>().actionButtonTemplate.transform.parent.GetChild(1).gameObject;
            cancelSave.transform.SetParent(null);
            Destroy(cancelSave);
        }
        GameObject.Find("GameManager").GetComponent<InputController>().SetUnitUI();
    }

    public void SpawnUnit(Unit unitToBuild, bool isPlayer)
    {
        Unit newUnit = Instantiate(unitToBuild);
        newUnit.transform.position = spawnPoint.transform.position;
        isBusy = false;

        newUnit.isOwnUnit = isPlayer;
        if (newUnit.isOwnUnit) {
            GameObject.Find("GameManager").GetComponent<UnitsController>().playerUnits.Add(newUnit);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().nbUnits++;
            /*
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().ownedUnits.Add(newUnit.gameObject);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().nbUnits++;
            */
        } else {
            GameObject.Find("GameManager").GetComponent<UnitsController>().enemyUnits.Add(newUnit);
        }
        if (spawnTarget) {
            newUnit.target = spawnTarget;
            if (targetType == TargetType.RESOURCE && newUnit.GetComponent<Gatherer>()) {
                newUnit.currentAction = Unit.Action.GATHERING;
                newUnit.GetComponent<Gatherer>().resourceNode = newUnit.target.gameObject.GetComponent<ResourceNode>();
                newUnit.GetComponent<Gatherer>().heldResource.resourceType = newUnit.GetComponent<Gatherer>().resourceNode.resourceType;
            } else {
                if (spawnTarget.GetComponent<Entity>().isOwnUnit) {
                    newUnit.currentAction = Unit.Action.MOVING;
                } else {
                    newUnit.currentAction = Unit.Action.ATTACKING;
                    newUnit.agent.stoppingDistance = newUnit.attackRange-1;
                }
            }
            newUnit.Move(newUnit.agent, newUnit.target.transform.position);
        } else {
            newUnit.Move(newUnit.agent, spawnWayPoint.transform.position);
        }
        buildingUnitDisplay.gameObject.SetActive(false);
    }

    public void SetBuildingBuildingDisplay()
    {
        buildingUnitDisplay.gameObject.SetActive(true);
        unitNameText.text = entityName;
        progressSlider.value = buildingTimePassed;
        progressSlider.maxValue = buildingTimeMax;
    }

    public override void SetUnitBuildingDisplay()
    {
        buildingUnitDisplay.gameObject.SetActive(true);
        unitNameText.text = unitToBuildName;
        progressSlider.value = timePassed;
        progressSlider.maxValue = timeToBuild;
        unitImageDisplay.transform.parent.gameObject.GetComponent<CancelAction>().resources = cancelButtonSave.resources;
        unitImageDisplay.transform.parent.gameObject.GetComponent<CancelAction>().refBuilding = cancelButtonSave.refBuilding;
        unitImageDisplay.sprite = unitSprite;
    }

    public override void RightClick(Vector3 positionVector, RaycastHit hit)
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, ~ignoreLayer)) {
          */
            switch (hit.collider.tag) {
                case ("Ground"):
                    if (!spawnWayPoint.activeInHierarchy && isCompleted) {
                        spawnWayPoint.gameObject.SetActive(true);
                    }
                    spawnWayPoint.transform.position = new Vector3(hit.point.x, hit.point.y+0.1f, hit.point.z);
                    spawnWayPoint.transform.localPosition = new Vector3(spawnWayPoint.transform.localPosition.x, spawnWayPoint.transform.localPosition.y, spawnWayPoint.transform.localPosition.z);
                    if (spawnTarget)
                        spawnTarget = null;
                    break;
                case ("Resource"):
                    if (spawnWayPoint.activeInHierarchy) {
                        spawnWayPoint.gameObject.SetActive(false);
                    }
                    spawnTarget = hit.collider.gameObject;
                    targetType = TargetType.RESOURCE;
                    break;
                case ("Building"):
                    if (spawnWayPoint.activeInHierarchy) {
                        spawnWayPoint.gameObject.SetActive(false);
                    }
                    spawnTarget = hit.collider.gameObject;
                    targetType = TargetType.BUILDING;
                    break;
                case ("Unit"):
                    if (spawnWayPoint.activeInHierarchy) {
                        spawnWayPoint.gameObject.SetActive(false);
                    }
                    spawnTarget = hit.collider.gameObject;
                    targetType = TargetType.UNIT;
                    break;
            }
        }
    //}
}
