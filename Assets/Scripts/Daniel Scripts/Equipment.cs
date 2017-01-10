using UnityEngine;
using System.Collections;
using System.Threading;

namespace MOBA.ExtClasses
{
    public enum EquippableType
    {
        HEAD_PIECE,
        CHEST_PIECE,
        LEG_PIECE,
        ARM_PIECE,
        WEAPON,
        SHIELD
    }

    public class Equippable
    {
        public string name { get; set; }
        public string description { get; set; }
        public int cost { get; set; }
        public Ability ability { get; set; }
        public string iconPath { get; set; }

        public Animation skillAnimation { get; set; }
        public EquippableType type { get; set; }

    }
}
