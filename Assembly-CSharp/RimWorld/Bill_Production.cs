﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020006B1 RID: 1713
	public class Bill_Production : Bill, IExposable
	{
		// Token: 0x04001445 RID: 5189
		public BillRepeatModeDef repeatMode = BillRepeatModeDefOf.RepeatCount;

		// Token: 0x04001446 RID: 5190
		public int repeatCount = 1;

		// Token: 0x04001447 RID: 5191
		private BillStoreModeDef storeMode = BillStoreModeDefOf.BestStockpile;

		// Token: 0x04001448 RID: 5192
		private Zone_Stockpile storeZone = null;

		// Token: 0x04001449 RID: 5193
		public int targetCount = 10;

		// Token: 0x0400144A RID: 5194
		public bool pauseWhenSatisfied = false;

		// Token: 0x0400144B RID: 5195
		public int unpauseWhenYouHave = 5;

		// Token: 0x0400144C RID: 5196
		public bool includeEquipped = false;

		// Token: 0x0400144D RID: 5197
		public bool includeTainted = false;

		// Token: 0x0400144E RID: 5198
		public Zone_Stockpile includeFromZone = null;

		// Token: 0x0400144F RID: 5199
		public FloatRange hpRange = FloatRange.ZeroToOne;

		// Token: 0x04001450 RID: 5200
		public QualityRange qualityRange = QualityRange.All;

		// Token: 0x04001451 RID: 5201
		public bool limitToAllowedStuff = false;

		// Token: 0x04001452 RID: 5202
		public bool paused = false;

		// Token: 0x060024BF RID: 9407 RVA: 0x0013ADE0 File Offset: 0x001391E0
		public Bill_Production()
		{
		}

		// Token: 0x060024C0 RID: 9408 RVA: 0x0013AE68 File Offset: 0x00139268
		public Bill_Production(RecipeDef recipe) : base(recipe)
		{
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060024C1 RID: 9409 RVA: 0x0013AEF0 File Offset: 0x001392F0
		protected override string StatusString
		{
			get
			{
				string result;
				if (this.paused)
				{
					result = " " + "Paused".Translate();
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060024C2 RID: 9410 RVA: 0x0013AF30 File Offset: 0x00139330
		protected override float StatusLineMinHeight
		{
			get
			{
				return (!this.CanUnpause()) ? 0f : 24f;
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060024C3 RID: 9411 RVA: 0x0013AF60 File Offset: 0x00139360
		public string RepeatInfoText
		{
			get
			{
				string result;
				if (this.repeatMode == BillRepeatModeDefOf.Forever)
				{
					result = "Forever".Translate();
				}
				else if (this.repeatMode == BillRepeatModeDefOf.RepeatCount)
				{
					result = this.repeatCount.ToString() + "x";
				}
				else
				{
					if (this.repeatMode != BillRepeatModeDefOf.TargetCount)
					{
						throw new InvalidOperationException();
					}
					result = this.recipe.WorkerCounter.CountProducts(this).ToString() + "/" + this.targetCount.ToString();
				}
				return result;
			}
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x0013B018 File Offset: 0x00139418
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Defs.Look<BillRepeatModeDef>(ref this.repeatMode, "repeatMode");
			Scribe_Values.Look<int>(ref this.repeatCount, "repeatCount", 0, false);
			Scribe_Defs.Look<BillStoreModeDef>(ref this.storeMode, "storeMode");
			Scribe_References.Look<Zone_Stockpile>(ref this.storeZone, "storeZone", false);
			Scribe_Values.Look<int>(ref this.targetCount, "targetCount", 0, false);
			Scribe_Values.Look<bool>(ref this.pauseWhenSatisfied, "pauseWhenSatisfied", false, false);
			Scribe_Values.Look<int>(ref this.unpauseWhenYouHave, "unpauseWhenYouHave", 0, false);
			Scribe_Values.Look<bool>(ref this.includeEquipped, "includeEquipped", false, false);
			Scribe_Values.Look<bool>(ref this.includeTainted, "includeTainted", false, false);
			Scribe_References.Look<Zone_Stockpile>(ref this.includeFromZone, "includeFromZone", false);
			Scribe_Values.Look<FloatRange>(ref this.hpRange, "hpRange", FloatRange.ZeroToOne, false);
			Scribe_Values.Look<QualityRange>(ref this.qualityRange, "qualityRange", QualityRange.All, false);
			Scribe_Values.Look<bool>(ref this.limitToAllowedStuff, "limitToAllowedStuff", false, false);
			Scribe_Values.Look<bool>(ref this.paused, "paused", false, false);
			if (this.repeatMode == null)
			{
				this.repeatMode = BillRepeatModeDefOf.RepeatCount;
			}
			if (this.storeMode == null)
			{
				this.storeMode = BillStoreModeDefOf.BestStockpile;
			}
		}

		// Token: 0x060024C5 RID: 9413 RVA: 0x0013B158 File Offset: 0x00139558
		public override BillStoreModeDef GetStoreMode()
		{
			return this.storeMode;
		}

		// Token: 0x060024C6 RID: 9414 RVA: 0x0013B174 File Offset: 0x00139574
		public override Zone_Stockpile GetStoreZone()
		{
			return this.storeZone;
		}

		// Token: 0x060024C7 RID: 9415 RVA: 0x0013B18F File Offset: 0x0013958F
		public override void SetStoreMode(BillStoreModeDef mode, Zone_Stockpile zone = null)
		{
			this.storeMode = mode;
			this.storeZone = zone;
			if (this.storeMode == BillStoreModeDefOf.SpecificStockpile != (this.storeZone != null))
			{
				Log.ErrorOnce("Inconsistent bill StoreMode data set", 75645354, false);
			}
		}

		// Token: 0x060024C8 RID: 9416 RVA: 0x0013B1D0 File Offset: 0x001395D0
		public override bool ShouldDoNow()
		{
			if (this.repeatMode != BillRepeatModeDefOf.TargetCount)
			{
				this.paused = false;
			}
			bool result;
			if (this.suspended)
			{
				result = false;
			}
			else if (this.repeatMode == BillRepeatModeDefOf.Forever)
			{
				result = true;
			}
			else if (this.repeatMode == BillRepeatModeDefOf.RepeatCount)
			{
				result = (this.repeatCount > 0);
			}
			else
			{
				if (this.repeatMode != BillRepeatModeDefOf.TargetCount)
				{
					throw new InvalidOperationException();
				}
				int num = this.recipe.WorkerCounter.CountProducts(this);
				if (this.pauseWhenSatisfied && num >= this.targetCount)
				{
					this.paused = true;
				}
				if (num <= this.unpauseWhenYouHave || !this.pauseWhenSatisfied)
				{
					this.paused = false;
				}
				result = (!this.paused && num < this.targetCount);
			}
			return result;
		}

		// Token: 0x060024C9 RID: 9417 RVA: 0x0013B2C8 File Offset: 0x001396C8
		public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
		{
			if (this.repeatMode == BillRepeatModeDefOf.RepeatCount)
			{
				if (this.repeatCount > 0)
				{
					this.repeatCount--;
				}
				if (this.repeatCount == 0)
				{
					Messages.Message("MessageBillComplete".Translate(new object[]
					{
						this.LabelCap
					}), (Thing)this.billStack.billGiver, MessageTypeDefOf.TaskCompletion, true);
				}
			}
		}

		// Token: 0x060024CA RID: 9418 RVA: 0x0013B348 File Offset: 0x00139748
		protected override void DoConfigInterface(Rect baseRect, Color baseColor)
		{
			Rect rect = new Rect(28f, 32f, 100f, 30f);
			GUI.color = new Color(1f, 1f, 1f, 0.65f);
			Widgets.Label(rect, this.RepeatInfoText);
			GUI.color = baseColor;
			WidgetRow widgetRow = new WidgetRow(baseRect.xMax, baseRect.y + 29f, UIDirection.LeftThenUp, 99999f, 4f);
			if (widgetRow.ButtonText("Details".Translate() + "...", null, true, false))
			{
				Find.WindowStack.Add(new Dialog_BillConfig(this, ((Thing)this.billStack.billGiver).Position));
			}
			if (widgetRow.ButtonText(this.repeatMode.LabelCap.PadRight(20), null, true, false))
			{
				BillRepeatModeUtility.MakeConfigFloatMenu(this);
			}
			if (widgetRow.ButtonIcon(TexButton.Plus, null, null))
			{
				if (this.repeatMode == BillRepeatModeDefOf.Forever)
				{
					this.repeatMode = BillRepeatModeDefOf.RepeatCount;
					this.repeatCount = 1;
				}
				else if (this.repeatMode == BillRepeatModeDefOf.TargetCount)
				{
					int num = this.recipe.targetCountAdjustment * GenUI.CurrentAdjustmentMultiplier();
					this.targetCount += num;
					this.unpauseWhenYouHave += num;
				}
				else if (this.repeatMode == BillRepeatModeDefOf.RepeatCount)
				{
					this.repeatCount += GenUI.CurrentAdjustmentMultiplier();
				}
				SoundDefOf.AmountIncrement.PlayOneShotOnCamera(null);
				if (TutorSystem.TutorialMode && this.repeatMode == BillRepeatModeDefOf.RepeatCount)
				{
					TutorSystem.Notify_Event(this.recipe.defName + "-RepeatCountSetTo-" + this.repeatCount);
				}
			}
			if (widgetRow.ButtonIcon(TexButton.Minus, null, null))
			{
				if (this.repeatMode == BillRepeatModeDefOf.Forever)
				{
					this.repeatMode = BillRepeatModeDefOf.RepeatCount;
					this.repeatCount = 1;
				}
				else if (this.repeatMode == BillRepeatModeDefOf.TargetCount)
				{
					int num2 = this.recipe.targetCountAdjustment * GenUI.CurrentAdjustmentMultiplier();
					this.targetCount = Mathf.Max(0, this.targetCount - num2);
					this.unpauseWhenYouHave = Mathf.Max(0, this.unpauseWhenYouHave - num2);
				}
				else if (this.repeatMode == BillRepeatModeDefOf.RepeatCount)
				{
					this.repeatCount = Mathf.Max(0, this.repeatCount - GenUI.CurrentAdjustmentMultiplier());
				}
				SoundDefOf.AmountDecrement.PlayOneShotOnCamera(null);
				if (TutorSystem.TutorialMode && this.repeatMode == BillRepeatModeDefOf.RepeatCount)
				{
					TutorSystem.Notify_Event(this.recipe.defName + "-RepeatCountSetTo-" + this.repeatCount);
				}
			}
		}

		// Token: 0x060024CB RID: 9419 RVA: 0x0013B640 File Offset: 0x00139A40
		private bool CanUnpause()
		{
			return this.repeatMode == BillRepeatModeDefOf.TargetCount && this.paused && this.pauseWhenSatisfied && this.recipe.WorkerCounter.CountProducts(this) < this.targetCount;
		}

		// Token: 0x060024CC RID: 9420 RVA: 0x0013B698 File Offset: 0x00139A98
		public override void DoStatusLineInterface(Rect rect)
		{
			if (this.paused)
			{
				WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenUp, 99999f, 4f);
				if (widgetRow.ButtonText("Unpause".Translate(), null, true, false))
				{
					this.paused = false;
				}
			}
		}

		// Token: 0x060024CD RID: 9421 RVA: 0x0013B6F4 File Offset: 0x00139AF4
		public override void ValidateSettings()
		{
			base.ValidateSettings();
			if (this.storeZone != null)
			{
				if (!this.storeZone.zoneManager.AllZones.Contains(this.storeZone))
				{
					if (this != BillUtility.Clipboard)
					{
						Messages.Message("MessageBillValidationStoreZoneDeleted".Translate(new object[]
						{
							this.LabelCap,
							this.billStack.billGiver.LabelShort.CapitalizeFirst(),
							this.storeZone.label
						}), this.billStack.billGiver as Thing, MessageTypeDefOf.NegativeEvent, true);
					}
					this.SetStoreMode(BillStoreModeDefOf.DropOnFloor, null);
				}
				else if (base.Map != null && !base.Map.zoneManager.AllZones.Contains(this.storeZone))
				{
					if (this != BillUtility.Clipboard)
					{
						Messages.Message("MessageBillValidationStoreZoneUnavailable".Translate(new object[]
						{
							this.LabelCap,
							this.billStack.billGiver.LabelShort.CapitalizeFirst(),
							this.storeZone.label
						}), this.billStack.billGiver as Thing, MessageTypeDefOf.NegativeEvent, true);
					}
					this.SetStoreMode(BillStoreModeDefOf.DropOnFloor, null);
				}
			}
			else if (this.storeMode == BillStoreModeDefOf.SpecificStockpile)
			{
				this.SetStoreMode(BillStoreModeDefOf.DropOnFloor, null);
				Log.ErrorOnce("Found SpecificStockpile bill store mode without associated stockpile, recovering", 46304128, false);
			}
			if (this.includeFromZone != null)
			{
				if (!this.includeFromZone.zoneManager.AllZones.Contains(this.includeFromZone))
				{
					if (this != BillUtility.Clipboard)
					{
						Messages.Message("MessageBillValidationIncludeZoneDeleted".Translate(new object[]
						{
							this.LabelCap,
							this.billStack.billGiver.LabelShort.CapitalizeFirst(),
							this.includeFromZone.label
						}), this.billStack.billGiver as Thing, MessageTypeDefOf.NegativeEvent, true);
					}
					this.includeFromZone = null;
				}
				else if (base.Map != null && !base.Map.zoneManager.AllZones.Contains(this.includeFromZone))
				{
					if (this != BillUtility.Clipboard)
					{
						Messages.Message("MessageBillValidationIncludeZoneUnavailable".Translate(new object[]
						{
							this.LabelCap,
							this.billStack.billGiver.LabelShort.CapitalizeFirst(),
							this.includeFromZone.label
						}), this.billStack.billGiver as Thing, MessageTypeDefOf.NegativeEvent, true);
					}
					this.includeFromZone = null;
				}
			}
		}

		// Token: 0x060024CE RID: 9422 RVA: 0x0013B9CC File Offset: 0x00139DCC
		public override Bill Clone()
		{
			Bill_Production bill_Production = (Bill_Production)base.Clone();
			bill_Production.repeatMode = this.repeatMode;
			bill_Production.repeatCount = this.repeatCount;
			bill_Production.storeMode = this.storeMode;
			bill_Production.storeZone = this.storeZone;
			bill_Production.targetCount = this.targetCount;
			bill_Production.pauseWhenSatisfied = this.pauseWhenSatisfied;
			bill_Production.unpauseWhenYouHave = this.unpauseWhenYouHave;
			bill_Production.includeEquipped = this.includeEquipped;
			bill_Production.includeTainted = this.includeTainted;
			bill_Production.includeFromZone = this.includeFromZone;
			bill_Production.hpRange = this.hpRange;
			bill_Production.qualityRange = this.qualityRange;
			bill_Production.limitToAllowedStuff = this.limitToAllowedStuff;
			bill_Production.paused = this.paused;
			return bill_Production;
		}
	}
}
