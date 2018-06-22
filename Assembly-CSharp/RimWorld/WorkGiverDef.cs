﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	// Token: 0x02000BB1 RID: 2993
	public class WorkGiverDef : Def
	{
		// Token: 0x17000A25 RID: 2597
		// (get) Token: 0x060040F1 RID: 16625 RVA: 0x00224CD4 File Offset: 0x002230D4
		public WorkGiver Worker
		{
			get
			{
				if (this.workerInt == null)
				{
					this.workerInt = (WorkGiver)Activator.CreateInstance(this.giverClass);
					this.workerInt.def = this;
				}
				return this.workerInt;
			}
		}

		// Token: 0x060040F2 RID: 16626 RVA: 0x00224D20 File Offset: 0x00223120
		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string error in this.<ConfigErrors>__BaseCallProxy0())
			{
				yield return error;
			}
			if (this.verb.NullOrEmpty())
			{
				yield return this.defName + " lacks a verb.";
			}
			if (this.gerund.NullOrEmpty())
			{
				yield return this.defName + " lacks a gerund.";
			}
			yield break;
		}

		// Token: 0x04002C36 RID: 11318
		public Type giverClass = null;

		// Token: 0x04002C37 RID: 11319
		public WorkTypeDef workType = null;

		// Token: 0x04002C38 RID: 11320
		public WorkTags workTags = WorkTags.None;

		// Token: 0x04002C39 RID: 11321
		public int priorityInType = 0;

		// Token: 0x04002C3A RID: 11322
		[MustTranslate]
		public string verb;

		// Token: 0x04002C3B RID: 11323
		[MustTranslate]
		public string gerund;

		// Token: 0x04002C3C RID: 11324
		public bool scanThings = true;

		// Token: 0x04002C3D RID: 11325
		public bool scanCells = false;

		// Token: 0x04002C3E RID: 11326
		public bool emergency = false;

		// Token: 0x04002C3F RID: 11327
		public List<PawnCapacityDef> requiredCapacities = new List<PawnCapacityDef>();

		// Token: 0x04002C40 RID: 11328
		public bool directOrderable = true;

		// Token: 0x04002C41 RID: 11329
		public bool prioritizeSustains = false;

		// Token: 0x04002C42 RID: 11330
		public bool canBeDoneByNonColonists = false;

		// Token: 0x04002C43 RID: 11331
		public JobTag tagToGive = JobTag.MiscWork;

		// Token: 0x04002C44 RID: 11332
		public WorkGiverEquivalenceGroupDef equivalenceGroup = null;

		// Token: 0x04002C45 RID: 11333
		public bool canBeDoneWhileDrafted = false;

		// Token: 0x04002C46 RID: 11334
		public int autoTakeablePriorityDrafted = -1;

		// Token: 0x04002C47 RID: 11335
		public ThingDef forceMote = null;

		// Token: 0x04002C48 RID: 11336
		public List<ThingDef> fixedBillGiverDefs = null;

		// Token: 0x04002C49 RID: 11337
		public bool billGiversAllHumanlikes = false;

		// Token: 0x04002C4A RID: 11338
		public bool billGiversAllHumanlikesCorpses = false;

		// Token: 0x04002C4B RID: 11339
		public bool billGiversAllMechanoids = false;

		// Token: 0x04002C4C RID: 11340
		public bool billGiversAllMechanoidsCorpses = false;

		// Token: 0x04002C4D RID: 11341
		public bool billGiversAllAnimals = false;

		// Token: 0x04002C4E RID: 11342
		public bool billGiversAllAnimalsCorpses = false;

		// Token: 0x04002C4F RID: 11343
		public bool tendToHumanlikesOnly = false;

		// Token: 0x04002C50 RID: 11344
		public bool tendToAnimalsOnly = false;

		// Token: 0x04002C51 RID: 11345
		public bool feedHumanlikesOnly = false;

		// Token: 0x04002C52 RID: 11346
		public bool feedAnimalsOnly = false;

		// Token: 0x04002C53 RID: 11347
		[Unsaved]
		private WorkGiver workerInt = null;
	}
}
