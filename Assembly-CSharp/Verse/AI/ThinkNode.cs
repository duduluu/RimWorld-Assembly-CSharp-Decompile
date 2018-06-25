﻿using System;
using System.Collections.Generic;

namespace Verse.AI
{
	// Token: 0x02000ACE RID: 2766
	public abstract class ThinkNode
	{
		// Token: 0x040026B9 RID: 9913
		public List<ThinkNode> subNodes = new List<ThinkNode>();

		// Token: 0x040026BA RID: 9914
		public bool leaveJoinableLordIfIssuesJob;

		// Token: 0x040026BB RID: 9915
		protected float priority = -1f;

		// Token: 0x040026BC RID: 9916
		[Unsaved]
		private int uniqueSaveKeyInt = -2;

		// Token: 0x040026BD RID: 9917
		[Unsaved]
		public ThinkNode parent;

		// Token: 0x040026BE RID: 9918
		public const int InvalidSaveKey = -1;

		// Token: 0x040026BF RID: 9919
		protected const int UnresolvedSaveKey = -2;

		// Token: 0x17000942 RID: 2370
		// (get) Token: 0x06003D6C RID: 15724 RVA: 0x0002F8A0 File Offset: 0x0002DCA0
		public int UniqueSaveKey
		{
			get
			{
				return this.uniqueSaveKeyInt;
			}
		}

		// Token: 0x17000943 RID: 2371
		// (get) Token: 0x06003D6D RID: 15725 RVA: 0x0002F8BC File Offset: 0x0002DCBC
		public IEnumerable<ThinkNode> ThisAndChildrenRecursive
		{
			get
			{
				yield return this;
				foreach (ThinkNode c in this.ChildrenRecursive)
				{
					yield return c;
				}
				yield break;
			}
		}

		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x06003D6E RID: 15726 RVA: 0x0002F8E8 File Offset: 0x0002DCE8
		public IEnumerable<ThinkNode> ChildrenRecursive
		{
			get
			{
				for (int i = 0; i < this.subNodes.Count; i++)
				{
					foreach (ThinkNode subSubNode in this.subNodes[i].ThisAndChildrenRecursive)
					{
						yield return subSubNode;
					}
				}
				yield break;
			}
		}

		// Token: 0x06003D6F RID: 15727 RVA: 0x0002F914 File Offset: 0x0002DD14
		public virtual float GetPriority(Pawn pawn)
		{
			float result;
			if (this.priority < 0f)
			{
				Log.ErrorOnce("ThinkNode_PrioritySorter has child node which didn't give a priority: " + this, this.GetHashCode(), false);
				result = 0f;
			}
			else
			{
				result = this.priority;
			}
			return result;
		}

		// Token: 0x06003D70 RID: 15728
		public abstract ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams);

		// Token: 0x06003D71 RID: 15729 RVA: 0x0002F962 File Offset: 0x0002DD62
		protected virtual void ResolveSubnodes()
		{
		}

		// Token: 0x06003D72 RID: 15730 RVA: 0x0002F968 File Offset: 0x0002DD68
		public void ResolveSubnodesAndRecur()
		{
			if (this.uniqueSaveKeyInt == -2)
			{
				this.ResolveSubnodes();
				for (int i = 0; i < this.subNodes.Count; i++)
				{
					this.subNodes[i].ResolveSubnodesAndRecur();
				}
			}
		}

		// Token: 0x06003D73 RID: 15731 RVA: 0x0002F9BE File Offset: 0x0002DDBE
		public virtual void ResolveReferences()
		{
		}

		// Token: 0x06003D74 RID: 15732 RVA: 0x0002F9C4 File Offset: 0x0002DDC4
		public virtual ThinkNode DeepCopy(bool resolve = true)
		{
			ThinkNode thinkNode = (ThinkNode)Activator.CreateInstance(base.GetType());
			for (int i = 0; i < this.subNodes.Count; i++)
			{
				thinkNode.subNodes.Add(this.subNodes[i].DeepCopy(resolve));
			}
			thinkNode.priority = this.priority;
			thinkNode.leaveJoinableLordIfIssuesJob = this.leaveJoinableLordIfIssuesJob;
			thinkNode.uniqueSaveKeyInt = this.uniqueSaveKeyInt;
			if (resolve)
			{
				thinkNode.ResolveSubnodesAndRecur();
			}
			ThinkTreeKeyAssigner.AssignSingleKey(thinkNode, 0);
			return thinkNode;
		}

		// Token: 0x06003D75 RID: 15733 RVA: 0x0002FA5F File Offset: 0x0002DE5F
		internal void SetUniqueSaveKey(int key)
		{
			this.uniqueSaveKeyInt = key;
		}

		// Token: 0x06003D76 RID: 15734 RVA: 0x0002FA6C File Offset: 0x0002DE6C
		public override int GetHashCode()
		{
			return Gen.HashCombineInt(this.uniqueSaveKeyInt, 1157295731);
		}
	}
}
