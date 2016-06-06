using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {


    public float m_attackRange;
    public float m_attackSpeed;
    public float m_timeBetweenAttacks;
    public int m_damage;

    private float m_nextAttackTime;
    private bool m_attackReady;
    private int m_playerMask;

    private EnemyMovement m_enemyMovement;

    private void Awake()
    {
        m_enemyMovement = GetComponent<EnemyMovement>();
        m_playerMask = LayerMask.GetMask("Player");
    }

    private void Start()
    {
        m_attackReady = true;
    }


    private IEnumerator AttackMove(Vector3 startPos, Vector3 endPos)
    {
        float timer = 0.0f;
        float rate = 1.0f / m_attackSpeed;
        while (timer < 1.0f)
        {
            if(m_attackReady)
            {
                Attack();
            }
            timer += Time.deltaTime * rate;
            transform.position = Vector3.Lerp(startPos, endPos, timer);
            yield return null;
        }
    }


    public bool TargetWithinAtkRange(Vector3 targetPos)
    {
        float sqrLengthToTarget = (targetPos - transform.position).sqrMagnitude;
        if (Time.time > m_nextAttackTime)
        {
            if (sqrLengthToTarget < m_attackRange * m_attackRange)
            {
                m_nextAttackTime = Time.time + m_timeBetweenAttacks;
                return true;
                
            }
        }
        return false;
    }

    public IEnumerator StartAttack(Vector3 targetPos)
    {
        m_attackReady = true;
        m_enemyMovement.ChangeMovementState(EnemyMovement.MovementState.IDLING);
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPos;
        yield return StartCoroutine(AttackMove(startPos, endPos));
        
        yield return StartCoroutine(AttackMove(endPos, startPos));

        //End Attack
        m_enemyMovement.ChangeMovementState(EnemyMovement.MovementState.CHASING);
    }

    private void Attack()
    {
        float attackRange = 1f;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit rayHit;
        if(Physics.Raycast(ray, out rayHit, attackRange, m_playerMask))
        {
            IDamageable damageableObject = rayHit.collider.gameObject.GetComponent<IDamageable>();
            if(damageableObject != null)
            {
                damageableObject.TakeDamage(m_damage);
                m_attackReady = false;
            }
        }
    }
}
