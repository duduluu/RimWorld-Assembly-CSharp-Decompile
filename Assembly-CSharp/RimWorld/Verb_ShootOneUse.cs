﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x020009D9 RID: 2521
	public class Verb_ShootOneUse : Verb_Shoot
	{
		// Token: 0x0600387F RID: 14463 RVA: 0x001E39A4 File Offset: 0x001E1DA4
		protected override bool TryCastShot()
		{
			bool result;
			if (base.TryCastShot())
			{
				if (this.burstShotsLeft <= 1)
				{
					this.SelfConsume();
				}
				result = true;
			}
			else
			{
				if (this.burstShotsLeft < this.verbProps.burstShotCount)
				{
					this.SelfConsume();
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06003880 RID: 14464 RVA: 0x001E39FC File Offset: 0x001E1DFC
		public override void Notify_EquipmentLost()
		{
			base.Notify_EquipmentLost();
			if (this.state == VerbState.Bursting && this.burstShotsLeft < this.verbProps.burstShotCount)
			{
				this.SelfConsume();
			}
		}

		// Token: 0x06003881 RID: 14465 RVA: 0x001E3A2D File Offset: 0x001E1E2D
		private void SelfConsume()
		{
			if (this.ownerEquipment != null && !this.ownerEquipment.Destroyed)
			{
				this.ownerEquipment.Destroy(DestroyMode.Vanish);
			}
		}
	}
}
