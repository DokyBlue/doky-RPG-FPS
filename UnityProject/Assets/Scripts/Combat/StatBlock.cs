using UnityEngine;

namespace RPGFPS.Combat
{
    [System.Serializable]
    public struct StatBlock
    {
        public float maxHp;
        public float armor;
        public float attack;
        public float fireRate;

        public StatBlock(float maxHp, float armor, float attack, float fireRate)
        {
            this.maxHp = maxHp;
            this.armor = armor;
            this.attack = attack;
            this.fireRate = fireRate;
        }

        public static StatBlock operator +(StatBlock a, StatBlock b)
        {
            return new StatBlock(
                a.maxHp + b.maxHp,
                a.armor + b.armor,
                a.attack + b.attack,
                a.fireRate + b.fireRate
            );
        }

        public static StatBlock operator *(StatBlock a, float scale)
        {
            return new StatBlock(
                a.maxHp * scale,
                a.armor * scale,
                a.attack * scale,
                a.fireRate * scale
            );
        }
    }
}
