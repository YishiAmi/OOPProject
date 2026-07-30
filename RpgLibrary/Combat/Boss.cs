using System;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;

namespace RpgLibrary.Combat
{
    // Boss = a special Enemy with an UltimateSkill and an enrage phase.
    // Shows both inheritance (Boss : Enemy) and composition (has-a Ultimate).
    //
    // Enrage threshold percent and the attack bonus applied on enrage
    // are ctor params — a designer can tune each boss without editing
    // this file.
    public class Boss : Enemy
    {
        public UltimateSkill Ultimate { get; set; }
        public bool IsEnraged { get; set; }

        public int EnrageThresholdPercent { get; }
        public int EnrageAttackBonus { get; }

        public Boss(string name,
                    int maxHealth,
                    int attack,
                    int defense,
                    UltimateSkill ultimate,
                    int speed = 7,
                    int enrageThresholdPercent = 30,
                    int enrageAttackBonus = 8)
            : base(name, maxHealth, attack, defense, speed)
        {
            if (ultimate == null)
                throw new ArgumentNullException(nameof(ultimate));
            if (enrageThresholdPercent < 0 || enrageThresholdPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(enrageThresholdPercent),
                    "Must be between 0 and 100.");
            if (enrageAttackBonus < 0)
                throw new ArgumentOutOfRangeException(nameof(enrageAttackBonus));

            Ultimate = ultimate;
            IsEnraged = false;
            EnrageThresholdPercent = enrageThresholdPercent;
            EnrageAttackBonus = enrageAttackBonus;
        }

        // Takes damage, charges the ultimate, and enters enrage
        // mode below the configured HP threshold.
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            Ultimate.Charge();

            CombatLog.Write($"(Ultimate charge: {Ultimate.CurrentCharge}/{Ultimate.MaxCharge})");

            int threshold = MaxHealth * EnrageThresholdPercent / 100;
            if (!IsEnraged && IsAlive() && Health <= threshold)
            {
                IsEnraged = true;
                Attack += EnrageAttackBonus;
                CombatLog.Write($" *** {Name} ENRAGES! Attack rises by {EnrageAttackBonus}. ***");
            }
        }

        public override void TakeTurn(ICombatant target)
        {
            if (!IsAlive() || target == null) return;

            string phase = IsEnraged ? "PHASE 2 (Enraged)" : "PHASE 1";
            CombatLog.Write($"[BOSS {Name} - {phase}]");

            if (Ultimate.IsCharged)
            {
                try
                {
                    Ultimate.Use(this, target);
                }
                catch (UltimateNotChargedException ex)
                {
                    CombatLog.Write($"[Boss] {ex.Message} - attacking normally instead.");
                    base.TakeTurn(target);
                }
            }
            else
            {
                base.TakeTurn(target);
            }
        }
    }
}
