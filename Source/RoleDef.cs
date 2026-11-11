using System.Collections.Generic;
using RimWorld;
using UnityEngine;
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

        private Texture2D icon;
        public Texture2D Icon => icon ?? (icon = ContentFinder<Texture2D>.Get("BetweenSus/Roles/" + defName, false));
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
