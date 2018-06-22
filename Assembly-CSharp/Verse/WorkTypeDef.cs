﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;

namespace Verse
{
	// Token: 0x02000BB4 RID: 2996
	public class WorkTypeDef : Def
	{
		// Token: 0x060040F6 RID: 16630 RVA: 0x0022501C File Offset: 0x0022341C
		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string e in this.<ConfigErrors>__BaseCallProxy0())
			{
				yield return e;
			}
			if (this.naturalPriority < 0 || this.naturalPriority > 10000)
			{
				yield return "naturalPriority is " + this.naturalPriority + ", but it must be between 0 and 10000";
			}
			yield break;
		}

		// Token: 0x060040F7 RID: 16631 RVA: 0x00225048 File Offset: 0x00223448
		public override void ResolveReferences()
		{
			foreach (WorkGiverDef item in from d in DefDatabase<WorkGiverDef>.AllDefs
			where d.workType == this
			orderby d.priorityInType descending
			select d)
			{
				this.workGiversByPriority.Add(item);
			}
		}

		// Token: 0x060040F8 RID: 16632 RVA: 0x002250DC File Offset: 0x002234DC
		public override int GetHashCode()
		{
			return Gen.HashCombine<string>(this.defName.GetHashCode(), this.gerundLabel);
		}

		// Token: 0x04002C65 RID: 11365
		public WorkTags workTags;

		// Token: 0x04002C66 RID: 11366
		[MustTranslate]
		public string labelShort;

		// Token: 0x04002C67 RID: 11367
		[MustTranslate]
		public string pawnLabel;

		// Token: 0x04002C68 RID: 11368
		[MustTranslate]
		public string gerundLabel;

		// Token: 0x04002C69 RID: 11369
		[MustTranslate]
		public string verb;

		// Token: 0x04002C6A RID: 11370
		public bool visible = true;

		// Token: 0x04002C6B RID: 11371
		public int naturalPriority = 0;

		// Token: 0x04002C6C RID: 11372
		public bool alwaysStartActive = false;

		// Token: 0x04002C6D RID: 11373
		public bool requireCapableColonist = false;

		// Token: 0x04002C6E RID: 11374
		public List<SkillDef> relevantSkills = new List<SkillDef>();

		// Token: 0x04002C6F RID: 11375
		[Unsaved]
		public List<WorkGiverDef> workGiversByPriority = new List<WorkGiverDef>();
	}
}
