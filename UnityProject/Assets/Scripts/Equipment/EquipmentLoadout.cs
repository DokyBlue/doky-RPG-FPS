using System.Collections.Generic;
using RPGFPS.Combat;
using UnityEngine;

namespace RPGFPS.Equipment
{
    public class EquipmentLoadout : MonoBehaviour
    {
        [SerializeField] private HealthComponent ownerHealth;
        [SerializeField] private StatBlock basePlayerStats = new(150f, 5f, 25f, 4f);
        [SerializeField] private List<EquipmentDefinition> equippedItems = new();

        public void Configure(HealthComponent healthComponent)
        {
            ownerHealth = healthComponent;
        }

        public void RecalculateStats()
        {
            if (ownerHealth == null) return;
            var finalStats = basePlayerStats;

            foreach (var item in equippedItems)
            {
                if (item == null) continue;
                finalStats += item.GetScaledBonus();
            }

            ownerHealth.OverrideStats(finalStats);
            ownerHealth.HealToFull();
        }

        public void Equip(EquipmentDefinition definition)
        {
            if (definition == null) return;
            equippedItems.Add(definition);
            RecalculateStats();
        }
    }
}
