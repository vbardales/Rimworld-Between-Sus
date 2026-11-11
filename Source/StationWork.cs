using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace BetweenSus
{
    [DefOf]
    public static class StationJobDefOf
    {
        public static JobDef BS_DoStation;

        static StationJobDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(StationJobDefOf));
    }

    public class WorkGiver_ShipStation : WorkGiver_Scanner
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false) =>
            !(GameComponent_Roles.Instance?.Active ?? false);

        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn) =>
            pawn.Map.listerBuildings.allBuildingsColonist.OfType<Building_Station>();

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false) =>
            t is Building_Station station && station.Available(pawn) && pawn.CanReserve(t, 1, -1, null, forced);

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false) =>
            HasJobOnThing(pawn, t, forced) ? JobMaker.MakeJob(StationJobDefOf.BS_DoStation, t) : null;
    }

    public class JobDriver_DoStation : JobDriver
    {
        private Building_Station Station => (Building_Station)job.targetA.Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed) =>
            pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            this.FailOn(() => !Station.Available(pawn));
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

            int ticks = Station.IsFix ? 450 : 300;
            if (GameComponent_Roles.Instance?.Is(pawn, "BS_Crewmate_Engineer") == true) ticks /= 2;
            yield return Toils_General.Wait(ticks)
                .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch)
                .WithProgressBarToilDelay(TargetIndex.A);

            yield return new Toil
            {
                initAction = () => Station.Complete(pawn),
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
