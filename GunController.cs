using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public Transform m_weaponHold;
    public Gun m_startingGun;

    private Gun m_equippedGun;
    private bool m_isGunEquipped;

    private void Start()
    {
        m_isGunEquipped = false;
        EquipGun(m_startingGun);

    }

    public void EquipGun(Gun newGun)
    {
        //Destroy existing gun
        if(m_isGunEquipped)
        {
            Destroy(m_equippedGun);
        }

        //Equip the new gun
        m_equippedGun = Instantiate(newGun, m_weaponHold.position, m_weaponHold.rotation) as Gun;
        m_equippedGun.transform.parent = m_weaponHold;
        m_isGunEquipped = true;
    }

    public void Shoot()
    {
        //Shoot gun if equipped
        if(m_isGunEquipped)
        {
            m_equippedGun.Shoot();
        }
    }
}
