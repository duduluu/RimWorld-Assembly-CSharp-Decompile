﻿using System;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	public class LordToil_HuntEnemies : LordToil
	{
		public LordToil_HuntEnemies(IntVec3 fallbackLocation)
		{
			this.data = new LordToilData_HuntEnemies();
			this.Data.fallbackLocation = fallbackLocation;
		}

		private LordToilData_HuntEnemies Data
		{
			get
			{
				return (LordToilData_HuntEnemies)this.data;
			}
		}

		public override bool ForceHighStoryDanger
		{
			get
			{
				return true;
			}
		}

		public override void UpdateAllDuties()
		{
			LordToilData_HuntEnemies data = this.Data;
			for (int i = 0; i < this.lord.ownedPawns.Count; i++)
			{
				Pawn pawn = this.lord.ownedPawns[i];
				if (!data.fallbackLocation.IsValid)
				{
					RCellFinder.TryFindRandomSpotJustOutsideColony(this.lord.ownedPawns[0], out data.fallbackLocation);
				}
				pawn.mindState.duty = new PawnDuty(DutyDefOf.HuntEnemiesIndividual);
				pawn.mindState.duty.focusSecond = data.fallbackLocation;
			}
		}
	}
}
