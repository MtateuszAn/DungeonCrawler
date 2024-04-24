using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemieScript : MonoBehaviour
{
    Transform player;
    NavMeshAgent agent;

    [SerializeField] float patrolRange;

    [SerializeField] public LayerMask targetMask;
    [SerializeField] public LayerMask obsticleMask;
    [SerializeField] public float radius360vision;
    [SerializeField] public float fovRadius;
    [SerializeField][Range(0, 360)] public float angle;

    [SerializeField] Renderer model;
    [SerializeField] Material materialIdle;
    [SerializeField] Material materialPatrol;
    [SerializeField] Material materialHunt;

    [SerializeField] float memorySec = 3;
    [SerializeField] float maxHealth = 100;
    float lastSeenPlayer= -99;
    float health;
    void Start()
    {
        health = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("/PlayerPrefab/Player").transform;
        StartCoroutine(EnemyRutine());
        StartCoroutine(FOVRutine());
    }
    private void OnDisable()
    {
        StopCoroutine(EnemyRutine());
        StopCoroutine(FOVRutine());
    }

    public void takeDamage(float damage)
    {
        lastSeenPlayer = Time.time;
        currentState = EnemyState.Hunt;
        health -= damage;
        if (health < 0) { 
            StopAllCoroutines();
            GameObject.Destroy(gameObject);
        }
    }

    public enum EnemyState
    {
        Idle,
        Hunt,
        Patrol,
        Atack
    }

    public EnemyState currentState = EnemyState.Hunt;

    IEnumerator EnemyRutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        // Aktualizuj zachowanie w zale¿noœci od aktualnego stanu
        while (true)
        {
            yield return wait;
            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleBehavior();
                    break;
                case EnemyState.Hunt:
                    UpdateHuntBehavior();
                    break;
                case EnemyState.Patrol:
                    UpdatePatrolBehavior();
                    break;
                case EnemyState.Atack:
                    UpdatePatrolBehavior();
                    break;
            }
        }
        
    }

    void UpdateIdleBehavior()
    {
        model.material = materialIdle;
    }
    float r;
    void UpdateHuntBehavior()
    {
        model.material = materialHunt;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.updateRotation = false;
            Vector3 dir = player.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            var rotation = lookRotation.eulerAngles;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation.y, ref r, 0.01f);
            transform.rotation = Quaternion.Euler(0f, Angle, 0f);
        }
        else
        {
            agent.updateRotation = true;
        }



        if (player != null)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            //Debug.Log("Nie ma gracza");
        }
    }

    void UpdatePatrolBehavior() // sorce https://github.com/JonDevTutorial/RandomNavMeshMovement/blob/main/RandomMovement.cs
    {
        model.material = materialPatrol;
        //Debug.Log("Patrol");
        if (agent.remainingDistance <= agent.stoppingDistance) //done with path
        {
            Vector3 point;
            if (RandomPoint(transform.position, patrolRange, out point)) //pass in our centre point and radius of area
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                agent.SetDestination(point);
            }
        }

    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)// sorce https://github.com/JonDevTutorial/RandomNavMeshMovement/blob/main/RandomMovement.cs
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        {
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    IEnumerator FOVRutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
        
    }

    void FieldOfViewCheck()
    {

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position+ new Vector3(0,1,0),fovRadius,targetMask);
        if (rangeChecks.Length !=0 )
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (player.position - transform.position).normalized;

            //Debug.Log("gracz w kolko");

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obsticleMask))
                {
                    lastSeenPlayer = Time.time;
                    currentState = EnemyState.Hunt;
                    return;
                }
                else
                {
                    Memory();
                }
                    
            }
            else
            {
                Memory();
            }
                
        }
        else if(currentState == EnemyState.Hunt)
            Memory();

        rangeChecks = Physics.OverlapSphere(transform.position + new Vector3(0, 1, 0), radius360vision, targetMask);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (player.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obsticleMask))
            {
                lastSeenPlayer = Time.time;
                currentState = EnemyState.Hunt;
                return;
            }
        }
    }

    void Memory()
    {
        //Debug.Log("MEM");
        if(lastSeenPlayer < Time.time - memorySec) 
        {
            currentState = EnemyState.Patrol;
        }
    }
}
