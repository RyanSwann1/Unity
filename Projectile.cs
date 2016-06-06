using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{

    public int m_damage;

    private float m_movementSpeed;
    private float m_destroyTime;
    private int m_shootableMask;

    private void Awake()
    {
        m_shootableMask = LayerMask.GetMask("Shootable");
    }

    private void Start()
    {
        m_destroyTime = 3.5f;
        Destroy(gameObject, m_destroyTime);
        //If initial collisons on spawn detected
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f);
        if (initialCollisions.Length > 0)
        {
            CheckInitialCollisions(initialCollisions[0]);
        }
    }

    private void Update()
    {
        float moveDistance = m_movementSpeed * Time.deltaTime;
        CheckCollisions(moveDistance);
        Move(moveDistance);
    }


    private void Move(float moveDistance)
    {
        transform.Translate(Vector3.forward * moveDistance);
    }

    private void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, Vector3.forward);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, moveDistance + 0.1f, m_shootableMask))
        {
            OnHit(rayHit);
        }
    }

    private void CheckInitialCollisions(Collider col)
    {
        IDamageable damageableGameObject = col.gameObject.GetComponent<IDamageable>();
        if(damageableGameObject != null)
        {
            damageableGameObject.TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }

    private void OnHit(RaycastHit rayHit)
    {
        IDamageable damageableGameObject = rayHit.collider.gameObject.GetComponent<IDamageable>();
        if(damageableGameObject != null)
        {
            damageableGameObject.TakeDamage(m_damage);
        }
        Destroy(gameObject);
    }

    public void SetSpeed(float newSpeed)
    {
        m_movementSpeed = newSpeed;
    }
}
