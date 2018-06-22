﻿using System;

namespace Verse
{
	// Token: 0x02000C7C RID: 3196
	public class FloodFillRangeQueue
	{
		// Token: 0x06004603 RID: 17923 RVA: 0x0024E7C3 File Offset: 0x0024CBC3
		public FloodFillRangeQueue(int initialSize)
		{
			this.array = new FloodFillRange[initialSize];
			this.head = 0;
			this.count = 0;
		}

		// Token: 0x17000B04 RID: 2820
		// (get) Token: 0x06004604 RID: 17924 RVA: 0x0024E7F4 File Offset: 0x0024CBF4
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000B05 RID: 2821
		// (get) Token: 0x06004605 RID: 17925 RVA: 0x0024E810 File Offset: 0x0024CC10
		public FloodFillRange First
		{
			get
			{
				return this.array[this.head];
			}
		}

		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x06004606 RID: 17926 RVA: 0x0024E83C File Offset: 0x0024CC3C
		public string PerfDebugString
		{
			get
			{
				return string.Concat(new object[]
				{
					"NumTimesExpanded: ",
					this.debugNumTimesExpanded,
					", MaxUsedSize= ",
					this.debugMaxUsedSpace,
					", ClaimedSize=",
					this.array.Length,
					", UnusedSpace=",
					this.array.Length - this.debugMaxUsedSpace
				});
			}
		}

		// Token: 0x06004607 RID: 17927 RVA: 0x0024E8C0 File Offset: 0x0024CCC0
		public void Enqueue(FloodFillRange r)
		{
			if (this.count + this.head == this.array.Length)
			{
				FloodFillRange[] destinationArray = new FloodFillRange[2 * this.array.Length];
				Array.Copy(this.array, this.head, destinationArray, 0, this.count);
				this.array = destinationArray;
				this.head = 0;
				this.debugNumTimesExpanded++;
			}
			this.array[this.head + this.count++] = r;
			this.debugMaxUsedSpace = this.count + this.head;
		}

		// Token: 0x06004608 RID: 17928 RVA: 0x0024E96C File Offset: 0x0024CD6C
		public FloodFillRange Dequeue()
		{
			FloodFillRange result = default(FloodFillRange);
			if (this.count > 0)
			{
				result = this.array[this.head];
				this.array[this.head] = default(FloodFillRange);
				this.head++;
				this.count--;
			}
			return result;
		}

		// Token: 0x04002FB9 RID: 12217
		private FloodFillRange[] array;

		// Token: 0x04002FBA RID: 12218
		private int count;

		// Token: 0x04002FBB RID: 12219
		private int head;

		// Token: 0x04002FBC RID: 12220
		private int debugNumTimesExpanded = 0;

		// Token: 0x04002FBD RID: 12221
		private int debugMaxUsedSpace = 0;
	}
}
