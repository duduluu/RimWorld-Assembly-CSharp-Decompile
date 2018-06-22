﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000CE6 RID: 3302
	public class PawnUIOverlay
	{
		// Token: 0x060048C9 RID: 18633 RVA: 0x00263630 File Offset: 0x00261A30
		public PawnUIOverlay(Pawn pawn)
		{
			this.pawn = pawn;
		}

		// Token: 0x060048CA RID: 18634 RVA: 0x00263640 File Offset: 0x00261A40
		public void DrawPawnGUIOverlay()
		{
			if (this.pawn.Spawned && !this.pawn.Map.fogGrid.IsFogged(this.pawn.Position))
			{
				if (!this.pawn.RaceProps.Humanlike)
				{
					AnimalNameDisplayMode animalNameMode = Prefs.AnimalNameMode;
					if (animalNameMode == AnimalNameDisplayMode.None)
					{
						return;
					}
					if (animalNameMode != AnimalNameDisplayMode.TameAll)
					{
						if (animalNameMode == AnimalNameDisplayMode.TameNamed)
						{
							if (this.pawn.Name == null || this.pawn.Name.Numerical)
							{
								return;
							}
						}
					}
					else if (this.pawn.Name == null)
					{
						return;
					}
				}
				Vector2 pos = GenMapUI.LabelDrawPosFor(this.pawn, -0.6f);
				GenMapUI.DrawPawnLabel(this.pawn, pos, 1f, 9999f, null, GameFont.Tiny, true, true);
				if (this.pawn.CanTradeNow)
				{
					this.pawn.Map.overlayDrawer.DrawOverlay(this.pawn, OverlayTypes.QuestionMark);
				}
			}
		}

		// Token: 0x04003143 RID: 12611
		private Pawn pawn;

		// Token: 0x04003144 RID: 12612
		private const float PawnLabelOffsetY = -0.6f;

		// Token: 0x04003145 RID: 12613
		private const int PawnStatBarWidth = 32;

		// Token: 0x04003146 RID: 12614
		private const float ActivityIconSize = 13f;

		// Token: 0x04003147 RID: 12615
		private const float ActivityIconOffsetY = 12f;
	}
}
