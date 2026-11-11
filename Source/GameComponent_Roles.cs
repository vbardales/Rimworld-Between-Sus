using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace BetweenSus
{
    /// <summary>
    /// Source of truth for roles. One pawn = one small role, the team follows from it.
    /// </summary>
    public class GameComponent_Roles : GameComponent
    {
        private Dictionary<Pawn, RoleDef> roles = new Dictionary<Pawn, RoleDef>();
        private Pawn mainPawn;
        private List<Pawn> keysWorkingList;
        private List<RoleDef> valuesWorkingList;

        public GameComponent_Roles(Game game) { }

        public static GameComponent_Roles Instance => Current.Game?.GetComponent<GameComponent_Roles>();

        public Pawn MainPawn => mainPawn;
        public bool Active => mainPawn != null && roles.Count > 0;

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

        public IEnumerable<Pawn> AllPawns() => roles.Keys;

        public int ImpostorCount => roles.Values.Count(r => r.team == Team.Impostor);

        /// <summary>Draws impostorCount impostors at random, the rest are crewmates. The main pawn is drawn at random.</summary>
        public void Assign(IList<Pawn> pawns, int impostorCount)
        {
            roles.Clear();
            var shuffled = pawns.InRandomOrder().ToList();
            for (int i = 0; i < shuffled.Count; i++)
                roles[shuffled[i]] = DrawRole(i < impostorCount ? Team.Impostor : Team.Crewmate);
            mainPawn = pawns.RandomElement();
        }

        private static RoleDef DrawRole(Team team)
        {
            var pool = DefDatabase<RoleDef>.AllDefs.Where(r => r.team == team).ToList();
            return pool.Count == 0 ? null : pool.RandomElementByWeight(r => r.weight);
        }

        /// <summary>True when the pawn has the small role with this defName.</summary>
        public bool Is(Pawn pawn, string roleDefName) => RoleOf(pawn)?.defName == roleDefName;

        /// <summary>Chance that a pawn refuses a player order. The main pawn never refuses.</summary>
        public float RefusalChance(Pawn pawn, Job job)
        {
            if (!Active || pawn == mainPawn) return 0f;
            var role = RoleOf(pawn);
            if (role == null) return 0f;

            if (role.team == Team.Impostor)
            {
                // Cover: obeys almost always, except an order that would expose it.
                if (job?.targetA.Thing is Pawn target && target != pawn && TeamOf(target) == Team.Impostor
                    && (job.def == JobDefOf.AttackMelee || job.def == JobDefOf.AttackStatic))
                    return 0.9f;
                return 0.02f;
            }

            float mood = pawn.needs?.mood?.CurLevelPercentage ?? 0.5f;
            float chance = 0.04f;
            if (mood < 0.35f) chance += 0.2f * (0.35f - mood) / 0.35f;
            return chance;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref mainPawn, "mainPawn");
            Scribe_Collections.Look(ref roles, "roles", LookMode.Reference, LookMode.Def,
                ref keysWorkingList, ref valuesWorkingList);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
                roles = (roles ?? new Dictionary<Pawn, RoleDef>())
                    .Where(kv => kv.Key != null && kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}
