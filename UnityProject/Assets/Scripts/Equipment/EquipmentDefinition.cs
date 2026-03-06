using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Equipment
{
    public enum EquipmentQuality
    {
        White,
        Blue,
        Purple,
        Gold
    }

    [CreateAssetMenu(fileName = "EquipmentDefinition", menuName = "RPGFPS/Equipment Definition")]
    public class EquipmentDefinition : ScriptableObject
    {
        public string itemName;
        public EquipmentQuality quality;
        public StatBlock bonus;

        public float QualityMultiplier => quality switch
        {
            EquipmentQuality.White => 1.00f,
            EquipmentQuality.Blue => 1.20f,
            EquipmentQuality.Purple => 1.45f,
            EquipmentQuality.Gold => 1.75f,
            _ => 1f
        };

        public StatBlock GetScaledBonus()
        {
            return bonus * QualityMultiplier;
        }
    }
}
