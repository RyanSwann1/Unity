using UnityEngine;
using System.Collections;

public interface IDamageable
{
    void TakeHit(RaycastHit hit, int damage); //Take position of hit & damage

    void TakeDamage(int damage); //Take damage
}
