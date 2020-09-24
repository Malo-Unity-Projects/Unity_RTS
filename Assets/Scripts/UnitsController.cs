using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsController : MonoBehaviour
{
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] unitsOnScene = GameObject.FindGameObjectsWithTag("Unit");

        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();

        for (int i = 0; i != unitsOnScene.Length; i++) {
            if (unitsOnScene[i].GetComponent<Unit>().isOwnUnit) {
                playerUnits.Add(unitsOnScene[i].GetComponent<Unit>());
            } else {
                enemyUnits.Add(unitsOnScene[i].GetComponent<Unit>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Unit unit in playerUnits) {
            if (unit.currentAction != Unit.Action.IDLE || unit.GetComponent<Gatherer>()) {
                continue ;
            }
            CheckForTarget(unit, enemyUnits);
        }
        foreach (Unit unit in enemyUnits) {
            if (unit.currentAction != Unit.Action.IDLE || unit.GetComponent<Gatherer>()) {
                continue ;
            }
            CheckForTarget(unit, playerUnits);
        }
    }

    void CheckForTarget(Unit unit, List<Unit> targets)
    {
        foreach (Unit target in targets) {
            if ((unit.transform.position - target.transform.position).magnitude < unit.attackRange) {
                unit.target = target.gameObject;
                unit.currentAction = Unit.Action.ATTACKING;
                Debug.Log("target found");
                return ;
            }
        }
        return ;
    }
}
