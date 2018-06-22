﻿using System;
using RimWorld.Planet;
using Verse;

namespace RimWorld
{
	// Token: 0x0200055C RID: 1372
	public class FactionRelation : IExposable
	{
		// Token: 0x060019EA RID: 6634 RVA: 0x000E153C File Offset: 0x000DF93C
		public void CheckKindThresholds(Faction faction, bool canSendLetter, string reason, GlobalTargetInfo lookTarget, out bool sentLetter)
		{
			FactionRelationKind previousKind = this.kind;
			sentLetter = false;
			if (this.kind != FactionRelationKind.Hostile && this.goodwill <= -75)
			{
				this.kind = FactionRelationKind.Hostile;
				faction.Notify_RelationKindChanged(this.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
			}
			if (this.kind != FactionRelationKind.Ally && this.goodwill >= 75)
			{
				this.kind = FactionRelationKind.Ally;
				faction.Notify_RelationKindChanged(this.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
			}
			if (this.kind == FactionRelationKind.Hostile && this.goodwill >= 0)
			{
				this.kind = FactionRelationKind.Neutral;
				faction.Notify_RelationKindChanged(this.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
			}
			if (this.kind == FactionRelationKind.Ally && this.goodwill <= 0)
			{
				this.kind = FactionRelationKind.Neutral;
				faction.Notify_RelationKindChanged(this.other, previousKind, canSendLetter, reason, lookTarget, out sentLetter);
			}
		}

		// Token: 0x060019EB RID: 6635 RVA: 0x000E1628 File Offset: 0x000DFA28
		public void ExposeData()
		{
			Scribe_References.Look<Faction>(ref this.other, "other", false);
			Scribe_Values.Look<int>(ref this.goodwill, "goodwill", 0, false);
			Scribe_Values.Look<FactionRelationKind>(ref this.kind, "kind", FactionRelationKind.Neutral, false);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				BackCompatibility.FactionRelationLoadingVars(this);
			}
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x000E167C File Offset: 0x000DFA7C
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"(",
				this.other,
				", goodwill=",
				this.goodwill.ToString("F1"),
				", kind=",
				this.kind,
				")"
			});
		}

		// Token: 0x04000F33 RID: 3891
		public Faction other = null;

		// Token: 0x04000F34 RID: 3892
		public int goodwill = 100;

		// Token: 0x04000F35 RID: 3893
		public FactionRelationKind kind = FactionRelationKind.Neutral;
	}
}
