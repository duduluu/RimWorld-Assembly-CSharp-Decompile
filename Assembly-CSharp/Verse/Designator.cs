﻿using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse.Sound;

namespace Verse
{
	// Token: 0x02000E14 RID: 3604
	public abstract class Designator : Command
	{
		// Token: 0x04003588 RID: 13704
		protected bool useMouseIcon = false;

		// Token: 0x04003589 RID: 13705
		public SoundDef soundDragSustain = null;

		// Token: 0x0400358A RID: 13706
		public SoundDef soundDragChanged = null;

		// Token: 0x0400358B RID: 13707
		protected SoundDef soundSucceeded = null;

		// Token: 0x0400358C RID: 13708
		protected SoundDef soundFailed = SoundDefOf.Designate_Failed;

		// Token: 0x0400358D RID: 13709
		protected bool hasDesignateAllFloatMenuOption;

		// Token: 0x0400358E RID: 13710
		protected string designateAllLabel;

		// Token: 0x0400358F RID: 13711
		private string cachedTutorTagSelect;

		// Token: 0x04003590 RID: 13712
		private string cachedTutorTagDesignate;

		// Token: 0x04003591 RID: 13713
		protected string cachedHighlightTag;

		// Token: 0x060051B0 RID: 20912 RVA: 0x001736E8 File Offset: 0x00171AE8
		public Designator()
		{
			this.activateSound = SoundDefOf.SelectDesignator;
			this.designateAllLabel = "DesignateAll".Translate();
		}

		// Token: 0x17000D64 RID: 3428
		// (get) Token: 0x060051B1 RID: 20913 RVA: 0x00173740 File Offset: 0x00171B40
		public Map Map
		{
			get
			{
				return Find.CurrentMap;
			}
		}

		// Token: 0x17000D65 RID: 3429
		// (get) Token: 0x060051B2 RID: 20914 RVA: 0x0017375C File Offset: 0x00171B5C
		public virtual int DraggableDimensions
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000D66 RID: 3430
		// (get) Token: 0x060051B3 RID: 20915 RVA: 0x00173774 File Offset: 0x00171B74
		public virtual bool DragDrawMeasurements
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000D67 RID: 3431
		// (get) Token: 0x060051B4 RID: 20916 RVA: 0x0017378C File Offset: 0x00171B8C
		protected override bool DoTooltip
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000D68 RID: 3432
		// (get) Token: 0x060051B5 RID: 20917 RVA: 0x001737A4 File Offset: 0x00171BA4
		protected virtual DesignationDef Designation
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000D69 RID: 3433
		// (get) Token: 0x060051B6 RID: 20918 RVA: 0x001737BC File Offset: 0x00171BBC
		public virtual float PanelReadoutTitleExtraRightMargin
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x17000D6A RID: 3434
		// (get) Token: 0x060051B7 RID: 20919 RVA: 0x001737D8 File Offset: 0x00171BD8
		public override string TutorTagSelect
		{
			get
			{
				string result;
				if (this.tutorTag == null)
				{
					result = null;
				}
				else
				{
					if (this.cachedTutorTagSelect == null)
					{
						this.cachedTutorTagSelect = "SelectDesignator-" + this.tutorTag;
					}
					result = this.cachedTutorTagSelect;
				}
				return result;
			}
		}

		// Token: 0x17000D6B RID: 3435
		// (get) Token: 0x060051B8 RID: 20920 RVA: 0x00173828 File Offset: 0x00171C28
		public string TutorTagDesignate
		{
			get
			{
				string result;
				if (this.tutorTag == null)
				{
					result = null;
				}
				else
				{
					if (this.cachedTutorTagDesignate == null)
					{
						this.cachedTutorTagDesignate = "Designate-" + this.tutorTag;
					}
					result = this.cachedTutorTagDesignate;
				}
				return result;
			}
		}

		// Token: 0x17000D6C RID: 3436
		// (get) Token: 0x060051B9 RID: 20921 RVA: 0x00173878 File Offset: 0x00171C78
		public override string HighlightTag
		{
			get
			{
				if (this.cachedHighlightTag == null && this.tutorTag != null)
				{
					this.cachedHighlightTag = "Designator-" + this.tutorTag;
				}
				return this.cachedHighlightTag;
			}
		}

		// Token: 0x17000D6D RID: 3437
		// (get) Token: 0x060051BA RID: 20922 RVA: 0x001738C0 File Offset: 0x00171CC0
		public override IEnumerable<FloatMenuOption> RightClickFloatMenuOptions
		{
			get
			{
				foreach (FloatMenuOption o in this.<get_RightClickFloatMenuOptions>__BaseCallProxy0())
				{
					yield return o;
				}
				if (this.hasDesignateAllFloatMenuOption)
				{
					int count = 0;
					List<Thing> things = this.Map.listerThings.AllThings;
					for (int i = 0; i < things.Count; i++)
					{
						Thing t = things[i];
						if (!t.Fogged() && this.CanDesignateThing(t).Accepted)
						{
							count++;
						}
					}
					if (count > 0)
					{
						yield return new FloatMenuOption(this.designateAllLabel + " (" + "CountToDesignate".Translate(new object[]
						{
							count
						}) + ")", delegate()
						{
							for (int k = 0; k < things.Count; k++)
							{
								Thing t2 = things[k];
								if (!t2.Fogged() && this.CanDesignateThing(t2).Accepted)
								{
									this.DesignateThing(things[k]);
								}
							}
						}, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						yield return new FloatMenuOption(this.designateAllLabel + " (" + "NoneLower".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
				}
				DesignationDef designation = this.Designation;
				if (this.Designation != null)
				{
					int count2 = 0;
					List<Designation> designations = this.Map.designationManager.allDesignations;
					for (int j = 0; j < designations.Count; j++)
					{
						if (designations[j].def == designation && this.RemoveAllDesignationsAffects(designations[j].target))
						{
							count2++;
						}
					}
					if (count2 > 0)
					{
						yield return new FloatMenuOption(string.Concat(new object[]
						{
							"RemoveAllDesignations".Translate(),
							" (",
							count2,
							")"
						}), delegate()
						{
							for (int k = designations.Count - 1; k >= 0; k--)
							{
								if (designations[k].def == designation && this.RemoveAllDesignationsAffects(designations[k].target))
								{
									this.Map.designationManager.RemoveDesignation(designations[k]);
								}
							}
						}, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
					else
					{
						yield return new FloatMenuOption("RemoveAllDesignations".Translate() + " (" + "NoneLower".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null);
					}
				}
				yield break;
			}
		}

		// Token: 0x060051BB RID: 20923 RVA: 0x001738EC File Offset: 0x00171CEC
		protected bool CheckCanInteract()
		{
			return !TutorSystem.TutorialMode || TutorSystem.AllowAction(this.TutorTagSelect);
		}

		// Token: 0x060051BC RID: 20924 RVA: 0x00173928 File Offset: 0x00171D28
		public override void ProcessInput(Event ev)
		{
			if (this.CheckCanInteract())
			{
				base.ProcessInput(ev);
				Find.DesignatorManager.Select(this);
			}
		}

		// Token: 0x060051BD RID: 20925 RVA: 0x00173950 File Offset: 0x00171D50
		public virtual AcceptanceReport CanDesignateThing(Thing t)
		{
			return AcceptanceReport.WasRejected;
		}

		// Token: 0x060051BE RID: 20926 RVA: 0x0017396A File Offset: 0x00171D6A
		public virtual void DesignateThing(Thing t)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060051BF RID: 20927
		public abstract AcceptanceReport CanDesignateCell(IntVec3 loc);

		// Token: 0x060051C0 RID: 20928 RVA: 0x00173974 File Offset: 0x00171D74
		public virtual void DesignateMultiCell(IEnumerable<IntVec3> cells)
		{
			if (!TutorSystem.TutorialMode || TutorSystem.AllowAction(new EventPack(this.TutorTagDesignate, cells)))
			{
				bool somethingSucceeded = false;
				bool flag = false;
				foreach (IntVec3 intVec in cells)
				{
					if (this.CanDesignateCell(intVec).Accepted)
					{
						this.DesignateSingleCell(intVec);
						somethingSucceeded = true;
						if (!flag)
						{
							flag = this.ShowWarningForCell(intVec);
						}
					}
				}
				this.Finalize(somethingSucceeded);
				if (TutorSystem.TutorialMode)
				{
					TutorSystem.Notify_Event(new EventPack(this.TutorTagDesignate, cells));
				}
			}
		}

		// Token: 0x060051C1 RID: 20929 RVA: 0x00173A40 File Offset: 0x00171E40
		public virtual void DesignateSingleCell(IntVec3 c)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060051C2 RID: 20930 RVA: 0x00173A48 File Offset: 0x00171E48
		public virtual bool ShowWarningForCell(IntVec3 c)
		{
			return false;
		}

		// Token: 0x060051C3 RID: 20931 RVA: 0x00173A5E File Offset: 0x00171E5E
		public void Finalize(bool somethingSucceeded)
		{
			if (somethingSucceeded)
			{
				this.FinalizeDesignationSucceeded();
			}
			else
			{
				this.FinalizeDesignationFailed();
			}
		}

		// Token: 0x060051C4 RID: 20932 RVA: 0x00173A78 File Offset: 0x00171E78
		protected virtual void FinalizeDesignationSucceeded()
		{
			if (this.soundSucceeded != null)
			{
				this.soundSucceeded.PlayOneShotOnCamera(null);
			}
		}

		// Token: 0x060051C5 RID: 20933 RVA: 0x00173A94 File Offset: 0x00171E94
		protected virtual void FinalizeDesignationFailed()
		{
			if (this.soundFailed != null)
			{
				this.soundFailed.PlayOneShotOnCamera(null);
			}
			if (Find.DesignatorManager.Dragger.FailureReason != null)
			{
				Messages.Message(Find.DesignatorManager.Dragger.FailureReason, MessageTypeDefOf.RejectInput, false);
			}
		}

		// Token: 0x060051C6 RID: 20934 RVA: 0x00173AE8 File Offset: 0x00171EE8
		public virtual string LabelCapReverseDesignating(Thing t)
		{
			return this.LabelCap;
		}

		// Token: 0x060051C7 RID: 20935 RVA: 0x00173B04 File Offset: 0x00171F04
		public virtual string DescReverseDesignating(Thing t)
		{
			return this.Desc;
		}

		// Token: 0x060051C8 RID: 20936 RVA: 0x00173B20 File Offset: 0x00171F20
		public virtual Texture2D IconReverseDesignating(Thing t, out float angle, out Vector2 offset)
		{
			angle = this.iconAngle;
			offset = this.iconOffset;
			return this.icon;
		}

		// Token: 0x060051C9 RID: 20937 RVA: 0x00173B50 File Offset: 0x00171F50
		protected virtual bool RemoveAllDesignationsAffects(LocalTargetInfo target)
		{
			return true;
		}

		// Token: 0x060051CA RID: 20938 RVA: 0x00173B68 File Offset: 0x00171F68
		public virtual void DrawMouseAttachments()
		{
			if (this.useMouseIcon)
			{
				GenUI.DrawMouseAttachment(this.icon, "", this.iconAngle, this.iconOffset, null);
			}
		}

		// Token: 0x060051CB RID: 20939 RVA: 0x00173BA6 File Offset: 0x00171FA6
		public virtual void DrawPanelReadout(ref float curY, float width)
		{
		}

		// Token: 0x060051CC RID: 20940 RVA: 0x00173BA9 File Offset: 0x00171FA9
		public virtual void DoExtraGuiControls(float leftX, float bottomY)
		{
		}

		// Token: 0x060051CD RID: 20941 RVA: 0x00173BAC File Offset: 0x00171FAC
		public virtual void SelectedUpdate()
		{
		}

		// Token: 0x060051CE RID: 20942 RVA: 0x00173BAF File Offset: 0x00171FAF
		public virtual void SelectedProcessInput(Event ev)
		{
		}

		// Token: 0x060051CF RID: 20943 RVA: 0x00173BB2 File Offset: 0x00171FB2
		public virtual void Rotate(RotationDirection rotDir)
		{
		}

		// Token: 0x060051D0 RID: 20944 RVA: 0x00173BB8 File Offset: 0x00171FB8
		public virtual bool CanRemainSelected()
		{
			return true;
		}

		// Token: 0x060051D1 RID: 20945 RVA: 0x00173BCE File Offset: 0x00171FCE
		public virtual void Selected()
		{
		}

		// Token: 0x060051D2 RID: 20946 RVA: 0x00173BD1 File Offset: 0x00171FD1
		public virtual void RenderHighlight(List<IntVec3> dragCells)
		{
			DesignatorUtility.RenderHighlightOverSelectableThings(this, dragCells);
		}
	}
}
