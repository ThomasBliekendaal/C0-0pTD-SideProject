using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassInfo", menuName = "ClassInfo")]
public class ClassScriptableObjectBaseStats : ScriptableObject
{
    public string className;
    public ClassStats baseStats;
    public string classInformation;
    public string ability;
    public DamageStyle damageStyle;
    public CrowdControl crowdControl;
    public Sprite image;

    public enum DamageStyle { Melee,Ranged,Combined}
    public enum CrowdControl { Bad,Decent,Good}
}

[System.Serializable]
public class ClassStats
{
    public float damage;
    public float health;
    public float mana;
    public float movementSpeed;
    public float jumpHeight;
}
