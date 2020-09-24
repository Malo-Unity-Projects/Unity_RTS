using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUI : MonoBehaviour
{
    public Image resourceImage;
    public Text resourceText;

    public void SetResourceUI(Resources.ResourceStruct resource, Sprite resourceIcon)
    {
        resourceImage.sprite = resourceIcon;
        resourceText.text = resource.resourceQuantity + " / " + resource.maxResource;
    }
}
