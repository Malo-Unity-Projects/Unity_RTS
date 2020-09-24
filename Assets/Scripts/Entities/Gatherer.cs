using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : Unit
{
    public bool isGathering;
    public float gatheringTime;
    public float gatheringDistance;

    public Resources.ResourceStruct heldResource;
    public ResourceNode resourceNode;
    GameObject closestDropOff;

    PlayerInventory playerInventory;

    public bool isBuilding;
    //float buildingTimePassed;
    //public float buildingTimeMax;
    public float buildingDistance;
    public Building buildingBeingBuild;

    void Awake()
    {
        currenHp = maxHp;
    }

    void Start()
    {
        playerInventory = GameObject.Find("Player").GetComponent<PlayerInventory>();
        StartCoroutine(ResourceTick());
    }

    void Update()
    {
        KeepCanvasAligned();

        switch (currentAction) {
            case (Action.DELIVERING):
                DeliverResources();
                break;
            case (Action.BUILDING):
                
                if (buildingBeingBuild && Vector3.Distance(transform.position, buildingBeingBuild.transform.position) < buildingDistance && !isBuilding) {
                    isBuilding = true;
                    //StartCoroutine(BuildBuilding());
                }
                break;
        }
        /*
        if (currenHp <= 0) {
            if (isOwnUnit) {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().nbUnits--;
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().ownedUnits.Remove(gameObject);
            }
            Destroy(gameObject);
        }
        */
        if (target) {
            ManageAttack();
        }
    }

    public override void RightClick(Vector3 positionVector, RaycastHit hit)
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)) {
            */
            target = null;
            resourceNode = null;
            isGathering = false;

            if (isBuilding && buildingBeingBuild != hit.collider.GetComponent<Building>()) {
                isBuilding = false;
                buildingBeingBuild.buildingGatherer = null;
                buildingBeingBuild = null;
            }
            switch (hit.collider.tag) {
                case ("Ground"):
                    Move(agent, hit.point);
                    currentAction = Action.MOVING;
                    break ;
                case ("Unit"):
                    Move(agent, hit.point);
                    agent.stoppingDistance = attackRange-1;
                    if (!hit.transform.GetComponent<Entity>().isOwnUnit) {
                        currentAction = Action.ATTACKING;
                        target = hit.collider.gameObject;
                    }
                    break ;
                case ("Building"):
                    Move(agent, hit.point);
                    if (hit.transform.GetComponent<Building>().isDrop) {
                        currentAction = Action.DELIVERING;
                        closestDropOff = hit.transform.gameObject;
                    } else if (!hit.transform.GetComponent<Building>().isCompleted) {
                        buildingBeingBuild = hit.transform.GetComponent<Building>();
                        hit.transform.GetComponent<Building>().buildingGatherer = this;
                        currentAction = Action.BUILDING;
                    } else {
                        currentAction = Action.MOVING;
                    }
                    break ;
                case ("Resource"):
                    currentAction = Action.GATHERING;
                    agent.stoppingDistance = gatheringDistance-1;
                    target = hit.collider.gameObject;
                    resourceNode = target.gameObject.GetComponent<ResourceNode>();
                    heldResource.resourceType = resourceNode.resourceType;
                    Move(agent, target.transform.position);
                    break ;
            }
        //}
    }

    IEnumerator ResourceTick()
    {
        while (true) {
            yield return new WaitForSeconds(gatheringTime);

            if (target)
                distToTarget = Vector3.Distance(transform.position, target.transform.position);

            if (distToTarget <= 2.5f && !isGathering && currentAction == Action.GATHERING) {
                isGathering = true;
            }

            if (isGathering && distToTarget <= 2.5f) {
                heldResource.resourceQuantity++;
                resourceNode.currentResources -= 1;
                if (resourceNode.currentResources <= 0) {
                    Destroy(resourceNode.gameObject);
                    isGathering = false;
                    currentAction = Action.DELIVERING;
                    TargetDrop();
                }
            }
            if (heldResource.resourceQuantity == heldResource.maxResource && currentAction == Action.GATHERING) {
                TargetDrop();
                isGathering = false;
            }
        }
    }

    public override void SetUnitBuildingDisplay()
    {
        playerInventory.buildingUnitDisplay.gameObject.SetActive(true);
        playerInventory.unitNameText.text = buildingBeingBuild.entityName;
        playerInventory.progressSlider.value = buildingBeingBuild.buildingTimePassed;
        playerInventory.progressSlider.maxValue = buildingBeingBuild.buildingTimeMax;
    }

    void TargetDrop()
    {
        closestDropOff = GetClosestDropOff(GameObject.Find("Player").GetComponent<PlayerInventory>().drops);
        Move(agent, closestDropOff.transform.position);
        currentAction = Action.DELIVERING;
    }

    void DeliverResources()
    {
        float distToDrop = Vector3.Distance(transform.position, closestDropOff.transform.position);

        if (distToDrop <= 3.5f) {
            currentAction = Action.IDLE;
            Move(agent, Vector3.zero);
            
            Resources.ResourceStruct res = playerInventory.ownedResources.Find(x => x.resourceType == heldResource.resourceType);
            int maxQuantity = res.maxResource - res.resourceQuantity;

            if (heldResource.resourceQuantity <= maxQuantity) {
                res.resourceQuantity += heldResource.resourceQuantity;
                heldResource.resourceQuantity = 0;
            } else {
                res.resourceQuantity = res.maxResource;
                heldResource.resourceQuantity -= maxQuantity;
            }
            for (int i = 0; i != playerInventory.ownedResources.Count; i++) {
                if (playerInventory.ownedResources[i].resourceType == res.resourceType) {
                    playerInventory.ownedResources[i] = res;
                }
            }
            if (target) {
                Move(agent, target.transform.position);
                agent.stoppingDistance = gatheringDistance-1;
                currentAction = Action.GATHERING;
            }
        }
    }

    GameObject GetClosestDropOff(List<GameObject> dropOffs)
    {
        GameObject closestDrop = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject targetDrop in dropOffs) {
            Vector3 direction = targetDrop.transform.position - position;
            float distance = direction.sqrMagnitude;
            if (distance < closestDistance) {
                closestDistance = distance;
                closestDrop = targetDrop;
            }
        }
        return (closestDrop);
    }
}
