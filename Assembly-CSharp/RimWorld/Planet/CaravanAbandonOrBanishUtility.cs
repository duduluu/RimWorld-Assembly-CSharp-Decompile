﻿using System;
using System.Text;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020005D3 RID: 1491
	public static class CaravanAbandonOrBanishUtility
	{
		// Token: 0x06001D57 RID: 7511 RVA: 0x000FC1CC File Offset: 0x000FA5CC
		public static void TryAbandonOrBanishViaInterface(Thing t, Caravan caravan)
		{
			Pawn p = t as Pawn;
			if (p != null)
			{
				if (!caravan.PawnsListForReading.Any((Pawn x) => x != p && caravan.IsOwner(x)))
				{
					Messages.Message("MessageCantBanishLastColonist".Translate(), caravan, MessageTypeDefOf.RejectInput, false);
				}
				else
				{
					PawnBanishUtility.ShowBanishPawnConfirmationDialog(p);
				}
			}
			else
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("ConfirmAbandonItemDialog".Translate(new object[]
				{
					t.Label
				}), delegate
				{
					Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, t);
					if (ownerOf == null)
					{
						Log.Error("Could not find owner of " + t, false);
					}
					else
					{
						ownerOf.inventory.innerContainer.Remove(t);
						t.Destroy(DestroyMode.Vanish);
						caravan.RecacheImmobilizedNow();
						caravan.RecacheDaysWorthOfFood();
					}
				}, true, null);
				Find.WindowStack.Add(window);
			}
		}

		// Token: 0x06001D58 RID: 7512 RVA: 0x000FC2A4 File Offset: 0x000FA6A4
		public static void TryAbandonOrBanishViaInterface(TransferableImmutable t, Caravan caravan)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				CaravanAbandonOrBanishUtility.TryAbandonOrBanishViaInterface(pawn, caravan);
			}
			else
			{
				Dialog_MessageBox window = Dialog_MessageBox.CreateConfirmation("ConfirmAbandonItemDialog".Translate(new object[]
				{
					t.LabelWithTotalStackCount
				}), delegate
				{
					for (int i = 0; i < t.things.Count; i++)
					{
						Thing thing = t.things[i];
						Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, thing);
						if (ownerOf == null)
						{
							Log.Error("Could not find owner of " + thing, false);
							return;
						}
						ownerOf.inventory.innerContainer.Remove(thing);
						thing.Destroy(DestroyMode.Vanish);
					}
					caravan.RecacheImmobilizedNow();
					caravan.RecacheDaysWorthOfFood();
				}, true, null);
				Find.WindowStack.Add(window);
			}
		}

		// Token: 0x06001D59 RID: 7513 RVA: 0x000FC330 File Offset: 0x000FA730
		public static void TryAbandonSpecificCountViaInterface(Thing t, Caravan caravan)
		{
			Find.WindowStack.Add(new Dialog_Slider("AbandonSliderText".Translate(new object[]
			{
				t.LabelNoCount
			}), 1, t.stackCount, delegate(int x)
			{
				Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, t);
				if (ownerOf == null)
				{
					Log.Error("Could not find owner of " + t, false);
				}
				else
				{
					if (x >= t.stackCount)
					{
						ownerOf.inventory.innerContainer.Remove(t);
						t.Destroy(DestroyMode.Vanish);
					}
					else
					{
						t.SplitOff(x).Destroy(DestroyMode.Vanish);
					}
					caravan.RecacheImmobilizedNow();
					caravan.RecacheDaysWorthOfFood();
				}
			}, int.MinValue));
		}

		// Token: 0x06001D5A RID: 7514 RVA: 0x000FC39C File Offset: 0x000FA79C
		public static void TryAbandonSpecificCountViaInterface(TransferableImmutable t, Caravan caravan)
		{
			Find.WindowStack.Add(new Dialog_Slider("AbandonSliderText".Translate(new object[]
			{
				t.Label
			}), 1, t.TotalStackCount, delegate(int x)
			{
				int num = x;
				for (int i = 0; i < t.things.Count; i++)
				{
					if (num <= 0)
					{
						break;
					}
					Thing thing = t.things[i];
					Pawn ownerOf = CaravanInventoryUtility.GetOwnerOf(caravan, thing);
					if (ownerOf == null)
					{
						Log.Error("Could not find owner of " + thing, false);
						return;
					}
					if (num >= thing.stackCount)
					{
						num -= thing.stackCount;
						ownerOf.inventory.innerContainer.Remove(thing);
						thing.Destroy(DestroyMode.Vanish);
					}
					else
					{
						thing.SplitOff(num).Destroy(DestroyMode.Vanish);
						num = 0;
					}
				}
				caravan.RecacheImmobilizedNow();
				caravan.RecacheDaysWorthOfFood();
			}, int.MinValue));
		}

		// Token: 0x06001D5B RID: 7515 RVA: 0x000FC408 File Offset: 0x000FA808
		public static string GetAbandonOrBanishButtonTooltip(Thing t, bool abandonSpecificCount)
		{
			Pawn pawn = t as Pawn;
			string result;
			if (pawn != null)
			{
				result = PawnBanishUtility.GetBanishButtonTip(pawn);
			}
			else
			{
				result = CaravanAbandonOrBanishUtility.GetAbandonItemButtonTooltip(t.stackCount, abandonSpecificCount);
			}
			return result;
		}

		// Token: 0x06001D5C RID: 7516 RVA: 0x000FC444 File Offset: 0x000FA844
		public static string GetAbandonOrBanishButtonTooltip(TransferableImmutable t, bool abandonSpecificCount)
		{
			Pawn pawn = t.AnyThing as Pawn;
			string result;
			if (pawn != null)
			{
				result = PawnBanishUtility.GetBanishButtonTip(pawn);
			}
			else
			{
				result = CaravanAbandonOrBanishUtility.GetAbandonItemButtonTooltip(t.TotalStackCount, abandonSpecificCount);
			}
			return result;
		}

		// Token: 0x06001D5D RID: 7517 RVA: 0x000FC484 File Offset: 0x000FA884
		private static string GetAbandonItemButtonTooltip(int currentStackCount, bool abandonSpecificCount)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (currentStackCount == 1)
			{
				stringBuilder.AppendLine("AbandonTip".Translate());
			}
			else if (abandonSpecificCount)
			{
				stringBuilder.AppendLine("AbandonSpecificCountTip".Translate());
			}
			else
			{
				stringBuilder.AppendLine("AbandonAllTip".Translate());
			}
			stringBuilder.AppendLine();
			stringBuilder.Append("AbandonItemTipExtraText".Translate());
			return stringBuilder.ToString();
		}
	}
}
