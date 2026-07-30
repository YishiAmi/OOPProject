namespace RpgLibrary.Combat
{
    // How a skill picks its target.
    // BattleSystem asks the skill for this instead of checking `is HealSkill`
    //  that means a library user can add a new Skill subclass tomorrow and
    // the battle loop will target it correctly with zero framework changes.
    public enum TargetType
    {
        Self,           // caster only (e.g. self-heal, buff)
        SingleAlly,     // one chosen ally (excluding self)
        SingleEnemy,    // one chosen enemy
        AllAllies,      // every living ally
        AllEnemies      // every living enemy
    }
}
