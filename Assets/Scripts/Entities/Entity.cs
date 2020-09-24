using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public string entityName;
    public int currenHp;
    public int maxHp;
    public Canvas healthBarCanvas;
    public Image healthBar;

    public string description;

    public bool isSelected;
    public bool isPrimary;
    public bool isOwnUnit;
    public bool isAllyUnit;

    public int damage;
    public int nbKills;

    public List<Action> actions;

    public LayerMask ignoreLayer;

    public Sprite entityIcon;

    void Awake()
    {
        currenHp = maxHp;
    }

    void Update()
    {
        KeepCanvasAligned();
    }

    public virtual void RightClick(Vector3 positionVector, RaycastHit hit)
    {
    }

    public void TakeDamage(int damage)
    {
        currenHp -= damage;
        healthBar.fillAmount = (float)currenHp / (float)maxHp;

        if (currenHp < 0)
            currenHp = 0;
        if (currenHp <= 0) {
            if (isOwnUnit) {
                GameObject.Find("GameManager").GetComponent<UnitsController>().playerUnits.Remove(gameObject.GetComponent<Unit>());
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().nbUnits--;
            } else {
                GameObject.Find("GameManager").GetComponent<UnitsController>().enemyUnits.Remove(gameObject.GetComponent<Unit>());
            }
            Debug.Log("ded");
            Destroy(gameObject);
        }
    }

    public virtual void SetUnitBuildingDisplay()
    {
    }

    public void KeepCanvasAligned()
    {
        healthBarCanvas.transform.eulerAngles = new Vector3(
            Camera.main.transform.eulerAngles.x,
            Camera.main.transform.eulerAngles.y,
            healthBarCanvas.transform.eulerAngles.z
        );
    }
}
