﻿using System;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace RimWorld
{
	// Token: 0x0200065B RID: 1627
	public class TaleData_Def : TaleData
	{
		// Token: 0x04001349 RID: 4937
		public Def def;

		// Token: 0x0400134A RID: 4938
		private string tmpDefName;

		// Token: 0x0400134B RID: 4939
		private Type tmpDefType;

		// Token: 0x060021FA RID: 8698 RVA: 0x00120620 File Offset: 0x0011EA20
		public override void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving)
			{
				this.tmpDefName = ((this.def == null) ? null : this.def.defName);
				this.tmpDefType = ((this.def == null) ? null : this.def.GetType());
			}
			Scribe_Values.Look<string>(ref this.tmpDefName, "defName", null, false);
			Scribe_Values.Look<Type>(ref this.tmpDefType, "defType", null, false);
			if (Scribe.mode == LoadSaveMode.LoadingVars && this.tmpDefName != null)
			{
				this.def = GenDefDatabase.GetDef(this.tmpDefType, this.tmpDefName, true);
			}
		}

		// Token: 0x060021FB RID: 8699 RVA: 0x001206D4 File Offset: 0x0011EAD4
		public override IEnumerable<Rule> GetRules(string prefix)
		{
			if (this.def != null)
			{
				yield return new Rule_String(prefix + "_label", this.def.label);
				yield return new Rule_String(prefix + "_definite", Find.ActiveLanguageWorker.WithDefiniteArticle(this.def.label));
				yield return new Rule_String(prefix + "_indefinite", Find.ActiveLanguageWorker.WithIndefiniteArticle(this.def.label));
			}
			yield break;
		}

		// Token: 0x060021FC RID: 8700 RVA: 0x00120708 File Offset: 0x0011EB08
		public static TaleData_Def GenerateFrom(Def def)
		{
			return new TaleData_Def
			{
				def = def
			};
		}
	}
}
