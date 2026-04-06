using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbilityManager : MonoBehaviour
{
    public static BulletAbilityManager Instance { get; private set; }
    //public List<BulletAbility> bulletAbilities;
    //public List<BulletAbility> activeAbilities;
    
    public int BA_DamageIncrease = 0;
    public float BA_DamageMultiplier = 1f;
    public bool BA_Stun = false;
    public bool BA_AOE = false;
    public bool BA_Slow = false;
    public bool BA_Opportunist = false;
    public bool BA_GoldenGun = false;
    public GameObject goldenGunVFX;

    private int bulletsToFire = 1;
    private float bulletDamage;
    private float bulletSpeed;

    public void BulletBuilder(BulletBehavior bullet)
    {
        bulletDamage = 1f;
        bulletSpeed = 10f;
        if (BA_DamageIncrease > 0)
        {
            bulletDamage += BA_DamageIncrease;
        }
        if (BA_DamageMultiplier > 1f)
        {
            bulletDamage *= BA_DamageMultiplier;
        }
        if (BA_Stun == true)
        {
            bullet.isStunBullet = true;
        }
        if (BA_AOE == true)
        {
            bullet.isAOEBullet = true;
            bullet.AOERadius = 2f;
        }
        if (BA_Slow == true)
        {
            bullet.isSlowBullet = true;
        }
        if (BA_Opportunist == true)
        {
            bullet.isOpportunistBullet = true;
        }
        if (BA_GoldenGun == true)
        {
            bulletsToFire = 3;
            BA_GoldenGun = false;
        }

        bulletsToFire -= 1;

        if (bulletsToFire <= 0)
        {
            bulletsToFire = 1;
            BA_DamageIncrease = 0;
            BA_DamageMultiplier = 1f;
            BA_Stun = false;
            BA_AOE = false;
            BA_Slow = false;
            BA_Opportunist = false;
            BA_GoldenGun = false;
            if (goldenGunVFX != null)
            {
                goldenGunVFX.SetActive(false);
            }
        }
    }
}
