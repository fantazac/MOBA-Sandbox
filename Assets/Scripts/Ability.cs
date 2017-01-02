using UnityEngine;
using System.Collections;
using System.Threading;

public enum AbilityType
{
    AREA,
    TARGETABLE
}

public class Ability
{
    public string name { get; set; }
    public string description { get; set; }
    public float manaCost { get; set; }
    public float coolDown { get; set; }
    public int level { get; set; }
    public int maxLevel { get; set; }
    public KeyCode assaignedKey { get; set; }
    public string iconPath { get; set; }

    public Animation skillAnimation { get; set; }
    public AbilityType type { get; set; }

}
