using System;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class JobGiver_TakeDrugsForDrugPolicy : ThinkNode_JobGiver
	{
		public override float GetPriority(Pawn pawn)
		{
			DrugPolicy currentPolicy = pawn.drugs.CurrentPolicy;
			for (int i = 0; i < currentPolicy.Count; i++)
			{
				if (pawn.drugs.ShouldTryToTakeScheduledNow(currentPolicy[i].drug))
				{
					return 7.5f;
				}
			}
			return 0f;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Profiler.BeginSample("DrugPolicy");
			DrugPolicy currentPolicy = pawn.drugs.CurrentPolicy;
			for (int i = 0; i < currentPolicy.Count; i++)
			{
				if (pawn.drugs.ShouldTryToTakeScheduledNow(currentPolicy[i].drug))
				{
					Thing thing = this.FindDrugFor(pawn, currentPolicy[i].drug);
					if (thing != null)
					{
						Profiler.EndSample();
						return DrugAIUtility.IngestAndTakeToInventoryJob(thing, pawn, 1);
					}
				}
			}
			Profiler.EndSample();
			return null;
		}

		private Thing FindDrugFor(Pawn pawn, ThingDef drugDef)
		{
			ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
			for (int i = 0; i < innerContainer.Count; i++)
			{
				if (innerContainer[i].def == drugDef && this.DrugValidator(pawn, innerContainer[i]))
				{
					return innerContainer[i];
				}
			}
			Predicate<Thing> validator = (Thing x) => this.DrugValidator(pawn, x);
			return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(drugDef), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, -1, false, RegionType.Set_Passable);
		}

		private bool DrugValidator(Pawn pawn, Thing drug)
		{
			if (!drug.def.IsDrug)
			{
				return false;
			}
			if (drug.Spawned)
			{
				if (drug.IsForbidden(pawn))
				{
					return false;
				}
				if (!pawn.CanReserve(drug, 1, -1, ReservationLayer.Default, false))
				{
					return false;
				}
				if (!drug.IsSociallyProper(pawn))
				{
					return false;
				}
			}
			return true;
		}
	}
}