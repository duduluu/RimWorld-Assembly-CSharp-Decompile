﻿using System;
using System.Collections;
using UnityEngine;

namespace RimWorld
{
	// Token: 0x020008A8 RID: 2216
	public static class PriceTypeUtlity
	{
		// Token: 0x060032CA RID: 13002 RVA: 0x001B5E4C File Offset: 0x001B424C
		public static float PriceMultiplier(this PriceType pType)
		{
			float result;
			switch (pType)
			{
			case PriceType.VeryCheap:
				result = 0.4f;
				break;
			case PriceType.Cheap:
				result = 0.7f;
				break;
			case PriceType.Normal:
				result = 1f;
				break;
			case PriceType.Expensive:
				result = 2f;
				break;
			case PriceType.Exorbitant:
				result = 5f;
				break;
			default:
				result = -1f;
				break;
			}
			return result;
		}

		// Token: 0x060032CB RID: 13003 RVA: 0x001B5EC0 File Offset: 0x001B42C0
		public static PriceType ClosestPriceType(float priceFactor)
		{
			float num = 99999f;
			PriceType priceType = PriceType.Undefined;
			IEnumerator enumerator = Enum.GetValues(typeof(PriceType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					PriceType priceType2 = (PriceType)obj;
					float num2 = Mathf.Abs(priceFactor - priceType2.PriceMultiplier());
					if (num2 < num)
					{
						num = num2;
						priceType = priceType2;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			if (priceType == PriceType.Undefined)
			{
				priceType = PriceType.VeryCheap;
			}
			return priceType;
		}
	}
}
