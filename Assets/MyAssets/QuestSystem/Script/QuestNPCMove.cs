using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class QuestNPCMove : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator ani;
    [SerializeField]
    private Transform destinationTrans;
    [SerializeField]
    private int questID;
    [SerializeField]
    private bool arriveDestinationDestroy;
    private Quaternion initialRotation;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        initialRotation = transform.rotation;
    }
    private void Start()
    {
        EventManager.Instance.AddEventRegister(EventDefinition.eventQuestNPCMove, EventQuestNPCMove);
    }
    private void Update()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && ani.GetBool("isWalk"))
        {
            ani.SetBool("isWalk", false);
            transform.rotation = initialRotation;
            if (arriveDestinationDestroy)
            {
                arriveDestinationDestroy = false;
                Destroy(gameObject);
            }
        }
    }
    private void EventQuestNPCMove(params object[] args)
    {
        if (questID == (int)args[0])
        {
            ani.SetBool("isWalk", true);
            navMeshAgent.SetDestination(destinationTrans.position);
        }
    }
}
