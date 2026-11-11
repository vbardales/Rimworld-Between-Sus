using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// Petit role : poste ET fonction d'un pion. Appartient a une equipe (gros role).
    /// </summary>
    public class RoleDef : Def
    {
        public Team team;
        public string iconPath;
        public string tip;
    }

    [DefOf]
    public static class RoleDefOf
    {
        public static RoleDef BS_Crewmate_Innocent;
        public static RoleDef BS_Impostor_Impostor;

        static RoleDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(RoleDefOf));
    }
}
