﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x02000874 RID: 2164
	public abstract class MainTabWindow_PawnTable : MainTabWindow
	{
		// Token: 0x04001AA4 RID: 6820
		private PawnTable table;

		// Token: 0x170007EC RID: 2028
		// (get) Token: 0x06003144 RID: 12612 RVA: 0x001AA3D8 File Offset: 0x001A87D8
		protected virtual float ExtraBottomSpace
		{
			get
			{
				return 53f;
			}
		}

		// Token: 0x170007ED RID: 2029
		// (get) Token: 0x06003145 RID: 12613 RVA: 0x001AA3F4 File Offset: 0x001A87F4
		protected virtual float ExtraTopSpace
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170007EE RID: 2030
		// (get) Token: 0x06003146 RID: 12614
		protected abstract PawnTableDef PawnTableDef { get; }

		// Token: 0x170007EF RID: 2031
		// (get) Token: 0x06003147 RID: 12615 RVA: 0x001AA410 File Offset: 0x001A8810
		protected override float Margin
		{
			get
			{
				return 6f;
			}
		}

		// Token: 0x170007F0 RID: 2032
		// (get) Token: 0x06003148 RID: 12616 RVA: 0x001AA42C File Offset: 0x001A882C
		public override Vector2 RequestedTabSize
		{
			get
			{
				Vector2 result;
				if (this.table == null)
				{
					result = Vector2.zero;
				}
				else
				{
					result = new Vector2(this.table.Size.x + this.Margin * 2f, this.table.Size.y + this.ExtraBottomSpace + this.ExtraTopSpace + this.Margin * 2f);
				}
				return result;
			}
		}

		// Token: 0x170007F1 RID: 2033
		// (get) Token: 0x06003149 RID: 12617 RVA: 0x001AA4AC File Offset: 0x001A88AC
		protected virtual IEnumerable<Pawn> Pawns
		{
			get
			{
				return Find.CurrentMap.mapPawns.FreeColonists;
			}
		}

		// Token: 0x0600314A RID: 12618 RVA: 0x001AA4D0 File Offset: 0x001A88D0
		public override void PostOpen()
		{
			if (this.table == null)
			{
				this.table = this.CreateTable();
			}
			this.SetDirty();
		}

		// Token: 0x0600314B RID: 12619 RVA: 0x001AA4F0 File Offset: 0x001A88F0
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			this.table.PawnTableOnGUI(new Vector2(rect.x, rect.y + this.ExtraTopSpace));
		}

		// Token: 0x0600314C RID: 12620 RVA: 0x001AA51F File Offset: 0x001A891F
		public void Notify_PawnsChanged()
		{
			this.SetDirty();
		}

		// Token: 0x0600314D RID: 12621 RVA: 0x001AA528 File Offset: 0x001A8928
		public override void Notify_ResolutionChanged()
		{
			this.table = this.CreateTable();
			base.Notify_ResolutionChanged();
		}

		// Token: 0x0600314E RID: 12622 RVA: 0x001AA540 File Offset: 0x001A8940
		private PawnTable CreateTable()
		{
			return (PawnTable)Activator.CreateInstance(this.PawnTableDef.workerClass, new object[]
			{
				this.PawnTableDef,
				new Func<IEnumerable<Pawn>>(() => this.Pawns),
				UI.screenWidth - (int)(this.Margin * 2f),
				(int)((float)(UI.screenHeight - 35) - this.ExtraBottomSpace - this.ExtraTopSpace - this.Margin * 2f)
			});
		}

		// Token: 0x0600314F RID: 12623 RVA: 0x001AA5D0 File Offset: 0x001A89D0
		protected void SetDirty()
		{
			this.table.SetDirty();
			this.SetInitialSizeAndPosition();
		}
	}
}
