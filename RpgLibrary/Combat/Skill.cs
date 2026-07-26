using System;

namespace rpg
{

    public abstract class Skill
    {
        public string Name {get;set;}
        public int Power {get;set;}

        protected Skill(string name, int power)
        {
            Name = name;
            Power = power;
        }
        public abstract void Use(ICombat source, ICombat target); //source is the the on who is using the skill while target is the one who is receiving it
    }


    public class AttackSkill : Skill
    {
    
        public AttackSkill(string name, int power) : base(name, power)
        {
            
        }

        public override void Use(ICombat source, ICombat target)
        {
            Console.WriteLine($"{source.Name} uses {Name} on {target.Name}!");
            target.TakeDamage(Power);
        }
    }

    public class HealSkill : Skill
    {
        public HealSkill(string name, int healAmount) : base(name, healAmount)
        {
            
        }

        public override void Use(ICombat source, ICombat target)
        {
            Console.WriteLine($"{source.Name} heals with {Name} {Power} HP");
            source.Heal(Power);   // heal SELF, ignore target
        }
    }
}
