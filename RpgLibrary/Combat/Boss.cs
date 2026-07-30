using System;
using RpgLibrary.Contracts;
using RpgLibrary.Exceptions;

namespace RpgLibrary.Combat
{
 
    // Boss, a special Enemy with an UltimateSkill and enrage mode. whuaaaaaa
    // Shows: inheritance (Boss : Enemy) + composition (has-a Ultimate).

    public class Boss : Enemy
    {
        public UltimateSkill Ultimate {get;set;}
        public bool IsEnraged {get;set; }

        public Boss(string name, int maxHealth, int attack, int defense, UltimateSkill ultimate): base(name, maxHealth, attack, defense)
        {
            Ultimate = ultimate;
            IsEnraged = false;
        }
        // and enters enrage mode below 30% HP.
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            Ultimate.Charge();

            Console.WriteLine($"(Ultimate charge:{Ultimate.CurrentCharge}/{Ultimate.MaxCharge})");

            if (!IsEnraged && IsAlive() && Health <= MaxHealth * 3 / 10)
            {
                IsEnraged = true;
                Attack += 8;
                Console.WriteLine($" *** {Name} ENRAGES! Attack rises by 8. ***");
            }
        }

        public override void TakeTurn(ICombat target)
        {
            if (!IsAlive()) return;

            string phase = IsEnraged ? "PHASE 2 (Enraged)" : "PHASE 1";
            Console.WriteLine($"[BOSS {Name} - {phase}]");

            if (Ultimate.IsCharged)
            {
            try
            {
                Ultimate.Use(this, target);
            }
            catch (UltimateNotChargedException ex)
            {
                Console.WriteLine($"[Boss] {ex.Message} - attacking normally instead.");
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

