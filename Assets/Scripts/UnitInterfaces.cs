using UnityEngine;
using System.Collections.Generic;
using MOBA.ExtClasses;

public interface IUnit
{
    float health { get; set; }
    float attack { get; set; }
    float defense { get; set; }
    float agility { get; set; }
    float movementSpeed { get; set; }
    float attackSpeed { get; set; }

    Animation attackAnimation { get; set; }
    Animation walkAnimation { get; set; }
    Animation idleAnimation { get; set; }
    Animation deathAnimation { get; set; }
    Animation lifeAnimation { get; set; }

    void Move(Vector3 movement);
    void DecreaseHealth(float attackedAmmount);
    void HealHealth(float healAmmount);
    void IncreaseHealth(float increaseAmmount);
    void Attack(GameObject target);
}

public interface IMob : IUnit
{

}

public interface ICharacter : IUnit
{
    float mana { get; set; }
    float abilityPower { get; set; }
    float physicalPenetration { get; set; }
    float magicalPenetration { get; set; }
    List<Ability> abilities { get; set; }
    List<Equippable> equipment { get; set; }
    int money { get; set; }

    void Equip(GameObject equipable);
    void CastTargetSelectAbility(GameObject target);
    void CastAreaSelectAbility(Vector3 location);
}
