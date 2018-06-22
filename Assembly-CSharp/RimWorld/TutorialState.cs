﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x020008D4 RID: 2260
	public class TutorialState : IExposable
	{
		// Token: 0x060033C7 RID: 13255 RVA: 0x001BAAA8 File Offset: 0x001B8EA8
		public void ExposeData()
		{
			if (Scribe.mode == LoadSaveMode.Saving && this.startingItems != null)
			{
				this.startingItems.RemoveAll((Thing it) => it == null || it.Destroyed || (it.Map == null && it.MapHeld == null));
			}
			Scribe_Collections.Look<Thing>(ref this.startingItems, "startingItems", LookMode.Reference, new object[0]);
			Scribe_Values.Look<CellRect>(ref this.roomRect, "roomRect", default(CellRect), false);
			Scribe_Values.Look<CellRect>(ref this.sandbagsRect, "sandbagsRect", default(CellRect), false);
			Scribe_Values.Look<int>(ref this.endTick, "endTick", -1, false);
			Scribe_Values.Look<bool>(ref this.introDone, "introDone", false, false);
			if (this.startingItems != null)
			{
				this.startingItems.RemoveAll((Thing it) => it == null);
			}
		}

		// Token: 0x060033C8 RID: 13256 RVA: 0x001BAB98 File Offset: 0x001B8F98
		public void Notify_TutorialEnding()
		{
			this.startingItems.Clear();
			this.roomRect = default(CellRect);
			this.sandbagsRect = default(CellRect);
			this.endTick = Find.TickManager.TicksGame;
		}

		// Token: 0x060033C9 RID: 13257 RVA: 0x001BABDF File Offset: 0x001B8FDF
		public void AddStartingItem(Thing t)
		{
			if (!this.startingItems.Contains(t))
			{
				this.startingItems.Add(t);
			}
		}

		// Token: 0x04001BC4 RID: 7108
		public List<Thing> startingItems = new List<Thing>();

		// Token: 0x04001BC5 RID: 7109
		public CellRect roomRect;

		// Token: 0x04001BC6 RID: 7110
		public CellRect sandbagsRect;

		// Token: 0x04001BC7 RID: 7111
		public int endTick = -1;

		// Token: 0x04001BC8 RID: 7112
		public bool introDone = false;
	}
}
