using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : Entity
{
    public int maxResources;
    public int currentResources;

    public Resources.ResourceType resourceType;
    public int gatherersNb;

    void Start()
    {
        currentResources = maxResources;
    }

    public override void RightClick(Vector3 positionVector, RaycastHit hit)
    {
    }
}
