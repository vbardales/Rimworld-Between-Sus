using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BetweenSus
{
    public class RoleAbility
    {
        public string label;
        public string description;
    }

    /// <summary>
    /// Small role: the post AND the function of a pawn. Belongs to one team (big role).
    /// </summary>
    public class RoleDef : Def
    {
        public Team team;
        public string tip;
        public float weight = 1f;
        public List<RoleAbility> abilities = new List<RoleAbility>();
    }

    [DefOf]
    public static class RoleDefOf
    {
        public static RoleDef BS_Crewmate_Innocent;
        public static RoleDef BS_Impostor_Impostor;

        static RoleDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RoleDefOf));
    }
}
