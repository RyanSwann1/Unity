using UnityEngine;
using System.Collections;

public class Enemy : LivingEntity {

    private EnemyAttack m_enemyAttack;
    private EnemyMovement m_enemyMovement;
    private bool m_hasTarget;
    private GameObject m_target;
    private LivingEntity m_targetObject;
    private Transform m_targetPos;

    private void Awake()
    {
        m_enemyAttack = GetComponent<EnemyAttack>();
        m_enemyMovement = GetComponent<EnemyMovement>();
    }

    protected override void Start()
    {
        base.Start();
        m_target = GameObject.FindGameObjectWithTag("Player");
        if(m_target != null)
        {
            m_targetPos = m_target.transform;
            m_hasTarget = true;
            StartCoroutine(m_enemyMovement.FollowTarget(m_targetPos));
            m_targetObject = m_target.GetComponent<LivingEntity>();
            m_targetObject.OnDeath += OnTargetDeath;
        }
        
    }

    private void Update()
    {
        if(m_hasTarget)
        {
            if (m_enemyAttack.TargetWithinAtkRange(m_targetPos.position))
            {
                StartCoroutine(m_enemyAttack.StartAttack(m_targetPos.position));
            }
        }

    }
    
    private void OnTargetDeath()
    {
        m_hasTarget = false;
        m_enemyMovement.ChangeMovementState(EnemyMovement.MovementState.IDLING);
    }

    public bool HasTarget()
    {
        return m_hasTarget;
    }
}