using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : Entity
{
    public enum Action { IDLE, MOVING, GATHERING, DELIVERING, ATTACKING, BUILDING };
    public Action currentAction;

    /* Attack */
    public float attackRange;
    public float attackDelay;
    bool isAttacking;
    public float distToTarget;
    public GameObject target;

    public Transform barrelEnd;
    public GameObject projectile;

    /* Movement */
    public NavMeshAgent agent;

    Vector3 lastPos;

    void Awake()
    {
        currenHp = maxHp;
        isAttacking = false;
    }

    void Update()
    {
        KeepCanvasAligned();

        if (target) {
            ManageAttack();
        }
        if (!target && currentAction == Action.ATTACKING) {
            currentAction = Action.IDLE;
            Move(agent, transform.position);
        }
        if (target && Vector3.Angle(transform.forward, target.transform.position - transform.position) > 10) {
            var q = Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 140 * Time.deltaTime);
            //transform.LookAt(target.transform);
        }
        if (currentAction == Action.MOVING) {
            /*
            if (agent.remainingDistance < 2) {
                Debug.Log("stop");
                Move(agent, transform.position);
                currentAction = Action.IDLE;
            }
            */
            if (Vector3.Distance(lastPos, transform.position) < 0.005 && agent.remainingDistance < 5) {
                Debug.Log("stop");
                Move(agent, transform.position);
                currentAction = Action.IDLE;
            }
        }
        lastPos = transform.position;
    }

    public override void RightClick(Vector3 positionVector, RaycastHit hit)
    {
        /*
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)) {
        */
            Debug.Log("tag: " + hit.transform.tag);

            target = null;

            Move(agent, positionVector);
            switch (hit.collider.tag) {
                case ("Ground"):
                    //Move(agent, hit.point);
                    currentAction = Action.MOVING;
                    agent.stoppingDistance = 0;
                    break ;
                case ("Unit"):
                    //Move(agent, hit.point);
                    if (!hit.transform.GetComponent<Entity>().isOwnUnit) {
                        Debug.Log("target acquired");
                        currentAction = Action.ATTACKING;
                        target = hit.collider.gameObject;
                        agent.stoppingDistance = attackRange-1;
                    }
                    break ;
                case ("Building"):
                case ("Resource"):
                    currentAction = Action.MOVING;
                    target = hit.collider.gameObject;
                    //Move(agent, hit.point);
                    break ;
            }
        //}
    }

    public void Move(NavMeshAgent agent, Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void ManageAttack()
    {
        if (currentAction == Action.GATHERING || currentAction == Action.ATTACKING) {
            distToTarget = Vector3.Distance(target.transform.position, transform.position);
        }
        if (currentAction == Action.ATTACKING && distToTarget <= attackRange && !isAttacking && Vector3.Angle(transform.forward, target.transform.position - transform.position) < 10) {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);
        if (target) {
            /* jouer une animation de folie */
            //target.GetComponent<Entity>().TakeDamage(damage);
            LaunchProjectile();
            if (target && target.GetComponent<Entity>().currenHp <= 0) {
                currentAction = Action.IDLE;
                nbKills++;
            } else if (!target) {
                currentAction = Action.IDLE;
            }
        }
        isAttacking = false;
    }

    public virtual void LaunchProjectile()
    {
        GameObject projectileClone = Instantiate(projectile, barrelEnd.position, barrelEnd.rotation);

        //projectileClone.transform.rotation = new Quaternion(projectileClone.transform.rotation.x, 0, projectileClone.transform.rotation.z, projectileClone.transform.rotation.w);
        //projectileClone.transform.position = barrelEnd.position;
        //projectileClone.GetComponent<Rigidbody>().AddForce(projectileClone.transform.forward * 25);
        projectileClone.GetComponent<Projectile>().damage = damage;
        projectileClone.GetComponent<Projectile>().target = target;
    }
}