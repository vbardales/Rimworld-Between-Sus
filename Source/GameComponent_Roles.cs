using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// Source de verite des roles. Un pion = un petit role, l'equipe en decoule.
    /// </summary>
    public class GameComponent_Roles : GameComponent
    {
        private Dictionary<Pawn, RoleDef> roles = new Dictionary<Pawn, RoleDef>();
        private List<Pawn> keysWorkingList;
        private List<RoleDef> valuesWorkingList;

        public GameComponent_Roles(Game game) { }

        public RoleDef RoleOf(Pawn pawn) =>
            pawn != null && roles.TryGetValue(pawn, out var role) ? role : null;

        public Team? TeamOf(Pawn pawn) => RoleOf(pawn)?.team;

        public IEnumerable<Pawn> Teammates(Pawn pawn)
        {
            var team = TeamOf(pawn);
            if (team == null) yield break;
            foreach (var kv in roles)
                if (kv.Key != pawn && kv.Value.team == team.Value)
                    yield return kv.Key;
        }

        /// <summary>Tire impostorCount imposteurs au hasard, le reste est coequipier.</summary>
        public void Assign(IList<Pawn> pawns, int impostorCount)
        {
            roles.Clear();
            var shuffled = pawns.InRandomOrder().ToList();
            for (int i = 0; i < shuffled.Count; i++)
            {
                roles[shuffled[i]] = i < impostorCount
                    ? RoleDefOf.BS_Impostor_Impostor
                    : RoleDefOf.BS_Crewmate_Innocent;
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref roles, "roles", LookMode.Reference, LookMode.Def,
                ref keysWorkingList, ref valuesWorkingList);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                roles = (roles ?? new Dictionary<Pawn, RoleDef>())
                    .Where(kv => kv.Key != null && kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
