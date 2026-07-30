using System;
using System.Collections.Generic;
using RpgLibrary.Contracts;

namespace RpgLibrary.Combat
{
    // A hero-side combatant plus the skills / ultimate the player can
    // pick during their turn. Enemies keep their own skill rotation
    // inside the Enemy class, so only the hero side needs this wrapper.
    //
    // Skills is exposed as IReadOnlyList so external code can't
    // mutate the skill loadout mid-battle. Internally we hold the
    // mutable List so this class can be extended with AddSkill later
    // if needed.
    public class PartyMember
    {
        private readonly List<Skill> _skills;

        public ICombatant Combatant { get; }
        public IReadOnlyList<Skill> Skills => _skills;
        public UltimateSkill? Ultimate { get; }

        public PartyMember(ICombatant combatant,
                           IEnumerable<Skill>? skills = null,
                           UltimateSkill? ultimate = null)
        {
            if (combatant == null)
                throw new ArgumentNullException(nameof(combatant));

            Combatant = combatant;
            _skills = skills != null ? new List<Skill>(skills) : new List<Skill>();
            Ultimate = ultimate;
        }
    }
}
