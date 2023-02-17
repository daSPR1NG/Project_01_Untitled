using NaughtyAttributes;
using UnityEngine;

namespace dnSR_Coding
{
    /// <summary>
    /// This class holds all the enums used in the project.
    /// </summary>
    public static class Enums 
    {        
        public enum StatType 
        {
            Unassigned,
            Strength_STR, Endurance_END, Dexterity_DEX,
            Initiative_INI, Health_HP, Damage_DMG,
        }

        public enum TurnInfluence 
        {
            Unassigned,
            Player, IA 
        }

        public enum Team 
        {
            Unassigned,
            Ally, Enemy 
        }

        public enum Operand 
        {
            PLUS, MINUS 
        }

        public enum CombatPosition
        {
            Left, Center, Right
        }

        public enum ItemSlot
        {
            Unassigned,
            Helmet, BodyArmour, Gloves, Boots, Weapon_Left, Weapon_Right, Ring_Left, Ring_Right, Amulet
        }
    }    
}