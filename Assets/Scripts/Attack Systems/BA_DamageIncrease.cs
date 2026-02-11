using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "BA_DamageIncrease", menuName = "Bullets/BA_DamageIncrease")]
public class BA_DamageIncrease : BulletAbility
{
    public bool isActive;
    public int damageIncrease = 2;

}
