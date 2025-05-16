using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "ScriptableObjects/SpellData")]
public class SpellData : ScriptableObject
{
    // Todo: make these private and serializable
    public float speed;
    public float range;
    public float damage;
    public float push;
    public float duration;

    // Also have triggers and events

    public SpellData()
    {
        speed = 5;
        range = -1;
        damage = 0;
        push = 0;
        duration = -1;
    }
}
