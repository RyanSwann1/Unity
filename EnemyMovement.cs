using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    public enum MovementState
    {
        CHASING,
        IDLING,
    }

    public float m_movementRefreshRate;

    private Enemy m_enemy;
    private NavMeshAgent m_navMeshAgent;
    private MovementState m_currentMovementState;

    private void Awake()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_enemy = GetComponent<Enemy>();
    }

    public IEnumerator FollowTarget(Transform targetPos)
    {
        while(m_enemy.HasTarget())
        {
            if(m_currentMovementState == MovementState.CHASING)
            {
                m_navMeshAgent.SetDestination(targetPos.position);
                
            }
            yield return new WaitForSeconds(m_movementRefreshRate);
        }
    }

    public void ChangeMovementState(MovementState newState)
    {
        m_currentMovementState = newState;
    }
}
