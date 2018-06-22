﻿using System;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace RimWorld.BaseGen
{
	// Token: 0x020003CF RID: 975
	public class SymbolResolver_RandomMechanoidGroup : SymbolResolver
	{
		// Token: 0x060010CF RID: 4303 RVA: 0x0008F514 File Offset: 0x0008D914
		public override void Resolve(ResolveParams rp)
		{
			int? mechanoidsCount = rp.mechanoidsCount;
			int num = (mechanoidsCount == null) ? SymbolResolver_RandomMechanoidGroup.DefaultMechanoidCountRange.RandomInRange : mechanoidsCount.Value;
			Lord lord = rp.singlePawnLord;
			if (lord == null && num > 0)
			{
				Map map = BaseGen.globalSettings.map;
				IntVec3 point;
				LordJob lordJob;
				if (Rand.Bool && (from x in rp.rect.Cells
				where !x.Impassable(map)
				select x).TryRandomElement(out point))
				{
					lordJob = new LordJob_DefendPoint(point);
				}
				else
				{
					lordJob = new LordJob_AssaultColony(Faction.OfMechanoids, false, false, false, false, false);
				}
				lord = LordMaker.MakeNewLord(Faction.OfMechanoids, lordJob, map, null);
			}
			for (int i = 0; i < num; i++)
			{
				PawnKindDef pawnKindDef = rp.singlePawnKindDef;
				if (pawnKindDef == null)
				{
					pawnKindDef = (from kind in DefDatabase<PawnKindDef>.AllDefsListForReading
					where kind.RaceProps.IsMechanoid
					select kind).RandomElementByWeight((PawnKindDef kind) => 1f / kind.combatPower);
				}
				ResolveParams resolveParams = rp;
				resolveParams.singlePawnKindDef = pawnKindDef;
				resolveParams.singlePawnLord = lord;
				resolveParams.faction = Faction.OfMechanoids;
				BaseGen.symbolStack.Push("pawn", resolveParams);
			}
		}

		// Token: 0x04000A3D RID: 2621
		private static readonly IntRange DefaultMechanoidCountRange = new IntRange(1, 5);
	}
}
