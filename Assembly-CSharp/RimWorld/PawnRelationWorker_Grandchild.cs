﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020004C5 RID: 1221
	public class PawnRelationWorker_Grandchild : PawnRelationWorker
	{
		// Token: 0x060015DC RID: 5596 RVA: 0x000C2824 File Offset: 0x000C0C24
		public override bool InRelation(Pawn me, Pawn other)
		{
			bool result;
			if (me == other)
			{
				result = false;
			}
			else
			{
				PawnRelationWorker worker = PawnRelationDefOf.Child.Worker;
				result = ((other.GetMother() != null && worker.InRelation(me, other.GetMother())) || (other.GetFather() != null && worker.InRelation(me, other.GetFather())));
			}
			return result;
		}
	}
}
