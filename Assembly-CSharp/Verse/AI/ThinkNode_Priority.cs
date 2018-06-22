﻿using System;

namespace Verse.AI
{
	// Token: 0x02000AB5 RID: 2741
	public class ThinkNode_Priority : ThinkNode
	{
		// Token: 0x06003D26 RID: 15654 RVA: 0x0005594C File Offset: 0x00053D4C
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			int count = this.subNodes.Count;
			for (int i = 0; i < count; i++)
			{
				ThinkResult result = ThinkResult.NoJob;
				try
				{
					result = this.subNodes[i].TryIssueJobPackage(pawn, jobParams);
				}
				catch (Exception ex)
				{
					Log.Error(string.Concat(new object[]
					{
						"Exception in ",
						base.GetType(),
						" TryIssueJobPackage: ",
						ex.ToString()
					}), false);
				}
				if (result.IsValid)
				{
					return result;
				}
			}
			return ThinkResult.NoJob;
		}
	}
}
