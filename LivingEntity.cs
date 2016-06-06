using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public int m_health;
    protected int m_currentHealth;
    protected bool m_isDead;

    public event System.Action OnDeath;

    protected virtual void Start()
    {
        m_currentHealth = m_health;
        m_isDead = false;
    }

    public void TakeHit(RaycastHit hit, int damage)
    {
        //Handle hit
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        m_currentHealth -= damage;
        if(m_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        m_isDead = true;
        if(OnDeath != null)
        {
            OnDeath();
        }
        
        Destroy(gameObject);
    }

}
