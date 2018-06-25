﻿using System;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x02000834 RID: 2100
	public class Page_SelectLandingSite : Page
	{
		// Token: 0x040019AE RID: 6574
		private const float GapBetweenBottomButtons = 10f;

		// Token: 0x040019AF RID: 6575
		private const float UseTwoRowsIfScreenWidthBelow = 1340f;

		// Token: 0x06002F7B RID: 12155 RVA: 0x00196CE4 File Offset: 0x001950E4
		public Page_SelectLandingSite()
		{
			this.absorbInputAroundWindow = false;
			this.shadowAlpha = 0f;
			this.preventCameraMotion = false;
		}

		// Token: 0x17000786 RID: 1926
		// (get) Token: 0x06002F7C RID: 12156 RVA: 0x00196D08 File Offset: 0x00195108
		public override string PageTitle
		{
			get
			{
				return "SelectLandingSite".Translate();
			}
		}

		// Token: 0x17000787 RID: 1927
		// (get) Token: 0x06002F7D RID: 12157 RVA: 0x00196D28 File Offset: 0x00195128
		public override Vector2 InitialSize
		{
			get
			{
				return Vector2.zero;
			}
		}

		// Token: 0x17000788 RID: 1928
		// (get) Token: 0x06002F7E RID: 12158 RVA: 0x00196D44 File Offset: 0x00195144
		protected override float Margin
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06002F7F RID: 12159 RVA: 0x00196D5E File Offset: 0x0019515E
		public override void PreOpen()
		{
			base.PreOpen();
			Find.World.renderer.wantedMode = WorldRenderMode.Planet;
			Find.WorldInterface.Reset();
			((MainButtonWorker_ToggleWorld)MainButtonDefOf.World.Worker).resetViewNextTime = true;
		}

		// Token: 0x06002F80 RID: 12160 RVA: 0x00196D96 File Offset: 0x00195196
		public override void PostOpen()
		{
			base.PostOpen();
			Find.GameInitData.ChooseRandomStartingTile();
			LessonAutoActivator.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
			TutorSystem.Notify_Event("PageStart-SelectLandingSite");
		}

		// Token: 0x06002F81 RID: 12161 RVA: 0x00196DC3 File Offset: 0x001951C3
		public override void PostClose()
		{
			base.PostClose();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}

		// Token: 0x06002F82 RID: 12162 RVA: 0x00196DDC File Offset: 0x001951DC
		public override void DoWindowContents(Rect rect)
		{
			if (Find.WorldInterface.SelectedTile >= 0)
			{
				Find.GameInitData.startingTile = Find.WorldInterface.SelectedTile;
			}
			else if (Find.WorldSelector.FirstSelectedObject != null)
			{
				Find.GameInitData.startingTile = Find.WorldSelector.FirstSelectedObject.Tile;
			}
		}

		// Token: 0x06002F83 RID: 12163 RVA: 0x00196E3B File Offset: 0x0019523B
		public override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			Text.Anchor = TextAnchor.UpperCenter;
			base.DrawPageTitle(new Rect(0f, 5f, (float)UI.screenWidth, 300f));
			Text.Anchor = TextAnchor.UpperLeft;
			this.DoCustomBottomButtons();
		}

		// Token: 0x06002F84 RID: 12164 RVA: 0x00196E78 File Offset: 0x00195278
		protected override bool CanDoNext()
		{
			bool result;
			if (!base.CanDoNext())
			{
				result = false;
			}
			else
			{
				int selectedTile = Find.WorldInterface.SelectedTile;
				if (selectedTile < 0)
				{
					Messages.Message("MustSelectLandingSite".Translate(), MessageTypeDefOf.RejectInput, false);
					result = false;
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					if (!TileFinder.IsValidTileForNewSettlement(selectedTile, stringBuilder))
					{
						Messages.Message(stringBuilder.ToString(), MessageTypeDefOf.RejectInput, false);
						result = false;
					}
					else
					{
						Tile tile = Find.WorldGrid[selectedTile];
						result = TutorSystem.AllowAction("ChooseBiome-" + tile.biome.defName + "-" + tile.hilliness.ToString());
					}
				}
			}
			return result;
		}

		// Token: 0x06002F85 RID: 12165 RVA: 0x00196F48 File Offset: 0x00195348
		protected override void DoNext()
		{
			int selTile = Find.WorldInterface.SelectedTile;
			FactionBaseProximityGoodwillUtility.CheckConfirmSettle(selTile, delegate
			{
				Find.GameInitData.startingTile = selTile;
				this.<DoNext>__BaseCallProxy0();
			});
		}

		// Token: 0x06002F86 RID: 12166 RVA: 0x00196F8C File Offset: 0x0019538C
		private void DoCustomBottomButtons()
		{
			int num = (!TutorSystem.TutorialMode) ? 5 : 4;
			int num2;
			if (num >= 4 && (float)UI.screenWidth < 1340f)
			{
				num2 = 2;
			}
			else
			{
				num2 = 1;
			}
			int num3 = Mathf.CeilToInt((float)num / (float)num2);
			float num4 = Page.BottomButSize.x * (float)num3 + 10f * (float)(num3 + 1);
			float num5 = (float)num2 * Page.BottomButSize.y + 10f * (float)(num2 + 1);
			Rect rect = new Rect(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);
			WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
			if (worldInspectPane != null && rect.x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
			{
				rect.x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
			}
			Widgets.DrawWindowBackground(rect);
			float num6 = rect.xMin + 10f;
			float num7 = rect.yMin + 10f;
			Text.Font = GameFont.Small;
			if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Back".Translate(), true, false, true))
			{
				if (this.CanDoBack())
				{
					this.DoBack();
				}
			}
			num6 += Page.BottomButSize.x + 10f;
			if (!TutorSystem.TutorialMode)
			{
				if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Advanced".Translate(), true, false, true))
				{
					Find.WindowStack.Add(new Dialog_AdvancedGameConfig(Find.WorldInterface.SelectedTile));
				}
				num6 += Page.BottomButSize.x + 10f;
			}
			if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "SelectRandomSite".Translate(), true, false, true))
			{
				SoundDefOf.Click.PlayOneShotOnCamera(null);
				Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
				Find.WorldCameraDriver.JumpTo(Find.WorldGrid.GetTileCenter(Find.WorldInterface.SelectedTile));
			}
			num6 += Page.BottomButSize.x + 10f;
			if (num2 == 2)
			{
				num6 = rect.xMin + 10f;
				num7 += Page.BottomButSize.y + 10f;
			}
			if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "WorldFactionsTab".Translate(), true, false, true))
			{
				Find.WindowStack.Add(new Dialog_FactionDuringLanding());
			}
			num6 += Page.BottomButSize.x + 10f;
			if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Next".Translate(), true, false, true))
			{
				if (this.CanDoNext())
				{
					this.DoNext();
				}
			}
			num6 += Page.BottomButSize.x + 10f;
			GenUI.AbsorbClicksInRect(rect);
		}
	}
}
