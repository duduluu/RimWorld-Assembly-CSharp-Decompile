﻿using System;

namespace Verse
{
	// Token: 0x020006D0 RID: 1744
	public class MoteCounter
	{
		// Token: 0x0400151C RID: 5404
		private int moteCount;

		// Token: 0x0400151D RID: 5405
		private const int SaturatedCount = 250;

		// Token: 0x170005AB RID: 1451
		// (get) Token: 0x060025BC RID: 9660 RVA: 0x00143A7C File Offset: 0x00141E7C
		public int MoteCount
		{
			get
			{
				return this.moteCount;
			}
		}

		// Token: 0x170005AC RID: 1452
		// (get) Token: 0x060025BD RID: 9661 RVA: 0x00143A98 File Offset: 0x00141E98
		public float Saturation
		{
			get
			{
				return (float)this.moteCount / 250f;
			}
		}

		// Token: 0x170005AD RID: 1453
		// (get) Token: 0x060025BE RID: 9662 RVA: 0x00143ABC File Offset: 0x00141EBC
		public bool Saturated
		{
			get
			{
				return this.Saturation > 1f;
			}
		}

		// Token: 0x170005AE RID: 1454
		// (get) Token: 0x060025BF RID: 9663 RVA: 0x00143AE0 File Offset: 0x00141EE0
		public bool SaturatedLowPriority
		{
			get
			{
				return this.Saturation > 0.8f;
			}
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x00143B02 File Offset: 0x00141F02
		public void Notify_MoteSpawned()
		{
			this.moteCount++;
		}

		// Token: 0x060025C1 RID: 9665 RVA: 0x00143B13 File Offset: 0x00141F13
		public void Notify_MoteDespawned()
		{
			this.moteCount--;
		}
	}
}
