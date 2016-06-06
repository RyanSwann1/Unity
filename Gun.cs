using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Projectile m_projectile;
    public Transform m_muzzle;
    public float m_muzzleVelocity;
    public float m_timeBetweenShots;
    public float m_reloadTime;
    public int m_maxMagazineSize;

    private float m_nextShotTime;
    private int m_currentMagazineSize;

    private void Start()
    {
        m_currentMagazineSize = m_maxMagazineSize;
    }

    public void Shoot()
    {
        if(Time.time > m_nextShotTime && m_currentMagazineSize > 0)
        {
            m_nextShotTime = Time.time + m_timeBetweenShots;
            Projectile newProjectile = Instantiate(m_projectile, m_muzzle.position, m_muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(m_muzzleVelocity);
            m_currentMagazineSize--;

            if(m_currentMagazineSize <= 0)
            {
                StartCoroutine(Reload());
            }
        }
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(m_reloadTime);
        m_currentMagazineSize = m_maxMagazineSize;
    }
}
