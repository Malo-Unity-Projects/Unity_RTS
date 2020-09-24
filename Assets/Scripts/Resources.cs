using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public enum ResourceType { METAL, MORPHIC, ENERGY };

    [System.Serializable]
    public struct ResourceStruct {
        public Resources.ResourceType resourceType;
        public int resourceQuantity;
        public int maxResource;
    }

    [System.Serializable]
    public struct ResourceCost {
        public Resources.ResourceType resourceType;
        public int resourceCost;
    }

    public enum Upgrade { BUILDHQ, BUILDBARRACKS, UPGRADESOLDIER, BUILDFACTORY, UPGRADETANK }
}
