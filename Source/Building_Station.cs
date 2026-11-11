using RimWorld;
using Verse;

namespace BetweenSus
{
    /// <summary>
    /// A ship station. Task stations are done once for good by a crewmate (an impostor only fakes it).
    /// Fix stations are active only while the matching sabotage runs.
    /// </summary>
    public class Building_Station : Building
    {
        private bool done;
        private int fakeUntil;

        public bool Done => done;
        public bool IsFix => def.defName.StartsWith("BS_Fix");
        public SabotageKind FixKind => def.defName == "BS_FixReactorStation" ? SabotageKind.Reactor : SabotageKind.O2;

        public bool Available(Pawn pawn)
        {
            var ship = GameComponent_Ship.Instance;
            var roles = GameComponent_Roles.Instance;
            if (ship == null || roles == null || !roles.Active || ship.Ended) return false;

            if (IsFix)
                return ship.ActiveSabotage == FixKind && !ship.IsFixed(this);

            if (done) return false;
            // An impostor may fake a task, then leaves it available for the crew.
            if (roles.TeamOf(pawn) == Team.Impostor)
                return Find.TickManager.TicksGame >= fakeUntil;
            return true;
        }

        public void Complete(Pawn pawn)
        {
            var ship = GameComponent_Ship.Instance;
            var roles = GameComponent_Roles.Instance;
            if (ship == null || roles == null) return;

            if (IsFix)
            {
                ship.MarkFixed(this);
                return;
            }
            if (roles.TeamOf(pawn) == Team.Impostor)
            {
                fakeUntil = Find.TickManager.TicksGame + 2500;
                return;
            }
            done = true;
            ship.OnTaskDone();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref done, "done");
            Scribe_Values.Look(ref fakeUntil, "fakeUntil");
        }
    }
}
