﻿using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020007E5 RID: 2021
	public abstract class Designator_Place : Designator
	{
		// Token: 0x040017B4 RID: 6068
		protected Rot4 placingRot = Rot4.North;

		// Token: 0x040017B5 RID: 6069
		protected static float middleMouseDownTime;

		// Token: 0x040017B6 RID: 6070
		private const float RotButSize = 64f;

		// Token: 0x040017B7 RID: 6071
		private const float RotButSpacing = 10f;

		// Token: 0x06002CE9 RID: 11497 RVA: 0x00179067 File Offset: 0x00177467
		public Designator_Place()
		{
			this.soundDragSustain = SoundDefOf.Designate_DragBuilding;
			this.soundDragChanged = null;
			this.soundSucceeded = SoundDefOf.Designate_PlaceBuilding;
		}

		// Token: 0x1700071D RID: 1821
		// (get) Token: 0x06002CEA RID: 11498
		public abstract BuildableDef PlacingDef { get; }

		// Token: 0x06002CEB RID: 11499 RVA: 0x00179098 File Offset: 0x00177498
		public override void DoExtraGuiControls(float leftX, float bottomY)
		{
			ThingDef thingDef = this.PlacingDef as ThingDef;
			if (thingDef != null && thingDef.rotatable)
			{
				Rect winRect = new Rect(leftX, bottomY - 90f, 200f, 90f);
				Find.WindowStack.ImmediateWindow(73095, winRect, WindowLayer.GameUI, delegate
				{
					RotationDirection rotationDirection = RotationDirection.None;
					Text.Anchor = TextAnchor.MiddleCenter;
					Text.Font = GameFont.Medium;
					Rect rect = new Rect(winRect.width / 2f - 64f - 5f, 15f, 64f, 64f);
					if (Widgets.ButtonImage(rect, TexUI.RotLeftTex))
					{
						SoundDefOf.AmountDecrement.PlayOneShotOnCamera(null);
						rotationDirection = RotationDirection.Counterclockwise;
						Event.current.Use();
					}
					Widgets.Label(rect, KeyBindingDefOf.Designator_RotateLeft.MainKeyLabel);
					Rect rect2 = new Rect(winRect.width / 2f + 5f, 15f, 64f, 64f);
					if (Widgets.ButtonImage(rect2, TexUI.RotRightTex))
					{
						SoundDefOf.AmountIncrement.PlayOneShotOnCamera(null);
						rotationDirection = RotationDirection.Clockwise;
						Event.current.Use();
					}
					Widgets.Label(rect2, KeyBindingDefOf.Designator_RotateRight.MainKeyLabel);
					if (rotationDirection != RotationDirection.None)
					{
						this.placingRot.Rotate(rotationDirection);
					}
					Text.Anchor = TextAnchor.UpperLeft;
					Text.Font = GameFont.Small;
				}, true, false, 1f);
			}
		}

		// Token: 0x06002CEC RID: 11500 RVA: 0x00179118 File Offset: 0x00177518
		public override void SelectedProcessInput(Event ev)
		{
			base.SelectedProcessInput(ev);
			ThingDef thingDef = this.PlacingDef as ThingDef;
			if (thingDef != null && thingDef.rotatable)
			{
				this.HandleRotationShortcuts();
			}
		}

		// Token: 0x06002CED RID: 11501 RVA: 0x00179150 File Offset: 0x00177550
		public override void SelectedUpdate()
		{
			GenDraw.DrawNoBuildEdgeLines();
			if (!ArchitectCategoryTab.InfoRect.Contains(UI.MousePositionOnUIInverted))
			{
				IntVec3 intVec = UI.MouseCell();
				if (this.PlacingDef is TerrainDef)
				{
					GenUI.RenderMouseoverBracket();
				}
				else
				{
					Color ghostCol;
					if (this.CanDesignateCell(intVec).Accepted)
					{
						ghostCol = new Color(0.5f, 1f, 0.6f, 0.4f);
					}
					else
					{
						ghostCol = new Color(1f, 0f, 0f, 0.4f);
					}
					this.DrawGhost(ghostCol);
					if (this.CanDesignateCell(intVec).Accepted)
					{
						if (this.PlacingDef.specialDisplayRadius > 0.01f)
						{
							GenDraw.DrawRadiusRing(UI.MouseCell(), this.PlacingDef.specialDisplayRadius);
						}
					}
					GenDraw.DrawInteractionCell((ThingDef)this.PlacingDef, intVec, this.placingRot);
				}
			}
		}

		// Token: 0x06002CEE RID: 11502 RVA: 0x0017924D File Offset: 0x0017764D
		protected virtual void DrawGhost(Color ghostCol)
		{
			GhostDrawer.DrawGhostThing(UI.MouseCell(), this.placingRot, (ThingDef)this.PlacingDef, null, ghostCol, AltitudeLayer.Blueprint);
		}

		// Token: 0x06002CEF RID: 11503 RVA: 0x00179270 File Offset: 0x00177670
		private void HandleRotationShortcuts()
		{
			RotationDirection rotationDirection = RotationDirection.None;
			if (Event.current.button == 2)
			{
				if (Event.current.type == EventType.MouseDown)
				{
					Event.current.Use();
					Designator_Place.middleMouseDownTime = Time.realtimeSinceStartup;
				}
				if (Event.current.type == EventType.MouseUp && Time.realtimeSinceStartup - Designator_Place.middleMouseDownTime < 0.15f)
				{
					rotationDirection = RotationDirection.Clockwise;
				}
			}
			if (KeyBindingDefOf.Designator_RotateRight.KeyDownEvent)
			{
				rotationDirection = RotationDirection.Clockwise;
			}
			if (KeyBindingDefOf.Designator_RotateLeft.KeyDownEvent)
			{
				rotationDirection = RotationDirection.Counterclockwise;
			}
			if (rotationDirection == RotationDirection.Clockwise)
			{
				SoundDefOf.AmountIncrement.PlayOneShotOnCamera(null);
				this.placingRot.Rotate(RotationDirection.Clockwise);
			}
			if (rotationDirection == RotationDirection.Counterclockwise)
			{
				SoundDefOf.AmountDecrement.PlayOneShotOnCamera(null);
				this.placingRot.Rotate(RotationDirection.Counterclockwise);
			}
		}

		// Token: 0x06002CF0 RID: 11504 RVA: 0x00179340 File Offset: 0x00177740
		public override void Selected()
		{
			this.placingRot = this.PlacingDef.defaultPlacingRot;
		}
	}
}
