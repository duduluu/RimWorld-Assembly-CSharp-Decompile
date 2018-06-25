﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000E89 RID: 3721
	public class Message : IArchivable, IExposable
	{
		// Token: 0x04003A10 RID: 14864
		public MessageTypeDef def;

		// Token: 0x04003A11 RID: 14865
		private int ID;

		// Token: 0x04003A12 RID: 14866
		public string text;

		// Token: 0x04003A13 RID: 14867
		private float startingTime;

		// Token: 0x04003A14 RID: 14868
		public int startingFrame;

		// Token: 0x04003A15 RID: 14869
		public int startingTick;

		// Token: 0x04003A16 RID: 14870
		public LookTargets lookTargets;

		// Token: 0x04003A17 RID: 14871
		private Vector2 cachedSize = new Vector2(-1f, -1f);

		// Token: 0x04003A18 RID: 14872
		public Rect lastDrawRect;

		// Token: 0x04003A19 RID: 14873
		private const float DefaultMessageLifespan = 13f;

		// Token: 0x04003A1A RID: 14874
		private const float FadeoutDuration = 0.6f;

		// Token: 0x060057C7 RID: 22471 RVA: 0x002D1180 File Offset: 0x002CF580
		public Message()
		{
		}

		// Token: 0x060057C8 RID: 22472 RVA: 0x002D11A0 File Offset: 0x002CF5A0
		public Message(string text, MessageTypeDef def)
		{
			this.text = text;
			this.def = def;
			this.startingFrame = RealTime.frameCount;
			this.startingTime = RealTime.LastRealTime;
			this.startingTick = GenTicks.TicksGame;
			this.ID = Rand.Int;
		}

		// Token: 0x060057C9 RID: 22473 RVA: 0x002D1203 File Offset: 0x002CF603
		public Message(string text, MessageTypeDef def, LookTargets lookTargets) : this(text, def)
		{
			this.lookTargets = lookTargets;
		}

		// Token: 0x17000DE1 RID: 3553
		// (get) Token: 0x060057CA RID: 22474 RVA: 0x002D1218 File Offset: 0x002CF618
		protected float Age
		{
			get
			{
				return RealTime.LastRealTime - this.startingTime;
			}
		}

		// Token: 0x17000DE2 RID: 3554
		// (get) Token: 0x060057CB RID: 22475 RVA: 0x002D123C File Offset: 0x002CF63C
		protected float TimeLeft
		{
			get
			{
				return 13f - this.Age;
			}
		}

		// Token: 0x17000DE3 RID: 3555
		// (get) Token: 0x060057CC RID: 22476 RVA: 0x002D1260 File Offset: 0x002CF660
		public bool Expired
		{
			get
			{
				return this.TimeLeft <= 0f;
			}
		}

		// Token: 0x17000DE4 RID: 3556
		// (get) Token: 0x060057CD RID: 22477 RVA: 0x002D1288 File Offset: 0x002CF688
		public float Alpha
		{
			get
			{
				float result;
				if (this.TimeLeft < 0.6f)
				{
					result = this.TimeLeft / 0.6f;
				}
				else
				{
					result = 1f;
				}
				return result;
			}
		}

		// Token: 0x17000DE5 RID: 3557
		// (get) Token: 0x060057CE RID: 22478 RVA: 0x002D12C4 File Offset: 0x002CF6C4
		private static bool ShouldDrawBackground
		{
			get
			{
				bool result;
				if (Current.ProgramState != ProgramState.Playing)
				{
					result = true;
				}
				else
				{
					WindowStack windowStack = Find.WindowStack;
					for (int i = 0; i < windowStack.Count; i++)
					{
						if (windowStack[i].CausesMessageBackground())
						{
							return true;
						}
					}
					result = false;
				}
				return result;
			}
		}

		// Token: 0x17000DDA RID: 3546
		// (get) Token: 0x060057CF RID: 22479 RVA: 0x002D1328 File Offset: 0x002CF728
		Texture IArchivable.ArchivedIcon
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000DDB RID: 3547
		// (get) Token: 0x060057D0 RID: 22480 RVA: 0x002D1340 File Offset: 0x002CF740
		Color IArchivable.ArchivedIconColor
		{
			get
			{
				return Color.white;
			}
		}

		// Token: 0x17000DDC RID: 3548
		// (get) Token: 0x060057D1 RID: 22481 RVA: 0x002D135C File Offset: 0x002CF75C
		string IArchivable.ArchivedLabel
		{
			get
			{
				return this.text.Flatten();
			}
		}

		// Token: 0x17000DDD RID: 3549
		// (get) Token: 0x060057D2 RID: 22482 RVA: 0x002D137C File Offset: 0x002CF77C
		string IArchivable.ArchivedTooltip
		{
			get
			{
				return this.text;
			}
		}

		// Token: 0x17000DDE RID: 3550
		// (get) Token: 0x060057D3 RID: 22483 RVA: 0x002D1398 File Offset: 0x002CF798
		int IArchivable.CreatedTicksGame
		{
			get
			{
				return this.startingTick;
			}
		}

		// Token: 0x17000DDF RID: 3551
		// (get) Token: 0x060057D4 RID: 22484 RVA: 0x002D13B4 File Offset: 0x002CF7B4
		bool IArchivable.CanCullArchivedNow
		{
			get
			{
				return !Messages.IsLive(this);
			}
		}

		// Token: 0x17000DE0 RID: 3552
		// (get) Token: 0x060057D5 RID: 22485 RVA: 0x002D13D4 File Offset: 0x002CF7D4
		LookTargets IArchivable.LookTargets
		{
			get
			{
				return this.lookTargets;
			}
		}

		// Token: 0x060057D6 RID: 22486 RVA: 0x002D13F0 File Offset: 0x002CF7F0
		public void ExposeData()
		{
			Scribe_Defs.Look<MessageTypeDef>(ref this.def, "def");
			Scribe_Values.Look<int>(ref this.ID, "ID", 0, false);
			Scribe_Values.Look<string>(ref this.text, "text", null, false);
			Scribe_Values.Look<float>(ref this.startingTime, "startingTime", 0f, false);
			Scribe_Values.Look<int>(ref this.startingFrame, "startingFrame", 0, false);
			Scribe_Values.Look<int>(ref this.startingTick, "startingTick", 0, false);
			Scribe_Deep.Look<LookTargets>(ref this.lookTargets, "lookTargets", new object[0]);
		}

		// Token: 0x060057D7 RID: 22487 RVA: 0x002D1484 File Offset: 0x002CF884
		public Rect CalculateRect(float x, float y)
		{
			Text.Font = GameFont.Small;
			if (this.cachedSize.x < 0f)
			{
				this.cachedSize = Text.CalcSize(this.text);
			}
			this.lastDrawRect = new Rect(x, y, this.cachedSize.x, this.cachedSize.y);
			this.lastDrawRect = this.lastDrawRect.ContractedBy(-2f);
			return this.lastDrawRect;
		}

		// Token: 0x060057D8 RID: 22488 RVA: 0x002D1504 File Offset: 0x002CF904
		public void Draw(int xOffset, int yOffset)
		{
			Rect rect = this.CalculateRect((float)xOffset, (float)yOffset);
			Find.WindowStack.ImmediateWindow(Gen.HashCombineInt(this.ID, 45574281), rect, WindowLayer.Super, delegate
			{
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleLeft;
				Rect rect = rect.AtZero();
				float alpha = this.Alpha;
				GUI.color = new Color(1f, 1f, 1f, alpha);
				if (Message.ShouldDrawBackground)
				{
					GUI.color = new Color(0.15f, 0.15f, 0.15f, 0.8f * alpha);
					GUI.DrawTexture(rect, BaseContent.WhiteTex);
					GUI.color = new Color(1f, 1f, 1f, alpha);
				}
				if (CameraJumper.CanJump(this.lookTargets.TryGetPrimaryTarget()))
				{
					UIHighlighter.HighlightOpportunity(rect, "Messages");
					Widgets.DrawHighlightIfMouseover(rect);
				}
				Rect rect2 = new Rect(2f, 0f, rect.width - 2f, rect.height);
				Widgets.Label(rect2, this.text);
				if (Current.ProgramState == ProgramState.Playing && CameraJumper.CanJump(this.lookTargets.TryGetPrimaryTarget()))
				{
					if (Widgets.ButtonInvisible(rect, false))
					{
						CameraJumper.TryJumpAndSelect(this.lookTargets.TryGetPrimaryTarget());
						PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.ClickingMessages, KnowledgeAmount.Total);
					}
				}
				Text.Anchor = TextAnchor.UpperLeft;
				GUI.color = Color.white;
				if (Mouse.IsOver(rect))
				{
					Messages.Notify_Mouseover(this);
				}
			}, false, false, 0f);
		}

		// Token: 0x060057D9 RID: 22489 RVA: 0x002D1564 File Offset: 0x002CF964
		void IArchivable.OpenArchived()
		{
			Find.WindowStack.Add(new Dialog_MessageBox(this.text, null, null, null, null, null, false, null, null));
		}
	}
}
