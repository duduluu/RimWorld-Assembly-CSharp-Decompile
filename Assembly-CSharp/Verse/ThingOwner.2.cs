﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000DFA RID: 3578
	public abstract class ThingOwner : IExposable, IList<Thing>, ICollection<Thing>, IEnumerable<Thing>, IEnumerable
	{
		// Token: 0x0400353B RID: 13627
		protected IThingHolder owner;

		// Token: 0x0400353C RID: 13628
		protected int maxStacks = 999999;

		// Token: 0x0400353D RID: 13629
		protected LookMode contentsLookMode = LookMode.Deep;

		// Token: 0x0400353E RID: 13630
		private const int InfMaxStacks = 999999;

		// Token: 0x060050A0 RID: 20640 RVA: 0x0029862F File Offset: 0x00296A2F
		public ThingOwner()
		{
		}

		// Token: 0x060050A1 RID: 20641 RVA: 0x0029864A File Offset: 0x00296A4A
		public ThingOwner(IThingHolder owner)
		{
			this.owner = owner;
		}

		// Token: 0x060050A2 RID: 20642 RVA: 0x0029866C File Offset: 0x00296A6C
		public ThingOwner(IThingHolder owner, bool oneStackOnly, LookMode contentsLookMode = LookMode.Deep) : this(owner)
		{
			this.maxStacks = ((!oneStackOnly) ? 999999 : 1);
			this.contentsLookMode = contentsLookMode;
		}

		// Token: 0x17000D36 RID: 3382
		// (get) Token: 0x060050A3 RID: 20643 RVA: 0x00298694 File Offset: 0x00296A94
		public IThingHolder Owner
		{
			get
			{
				return this.owner;
			}
		}

		// Token: 0x17000D37 RID: 3383
		// (get) Token: 0x060050A4 RID: 20644
		public abstract int Count { get; }

		// Token: 0x17000D38 RID: 3384
		public Thing this[int index]
		{
			get
			{
				return this.GetAt(index);
			}
		}

		// Token: 0x17000D39 RID: 3385
		// (get) Token: 0x060050A6 RID: 20646 RVA: 0x002986CC File Offset: 0x00296ACC
		public bool Any
		{
			get
			{
				return this.Count > 0;
			}
		}

		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x060050A7 RID: 20647 RVA: 0x002986EC File Offset: 0x00296AEC
		public int TotalStackCount
		{
			get
			{
				int num = 0;
				int count = this.Count;
				for (int i = 0; i < count; i++)
				{
					num += this.GetAt(i).stackCount;
				}
				return num;
			}
		}

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x060050A8 RID: 20648 RVA: 0x00298730 File Offset: 0x00296B30
		public string ContentsString
		{
			get
			{
				int count = this.Count;
				string result;
				if (count == 0)
				{
					result = "NothingLower".Translate();
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < count; i++)
					{
						if (i != 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(this.GetAt(i).Label);
					}
					result = stringBuilder.ToString();
				}
				return result;
			}
		}

		// Token: 0x17000D34 RID: 3380
		Thing IList<Thing>.this[int index]
		{
			get
			{
				return this.GetAt(index);
			}
			set
			{
				throw new InvalidOperationException("ThingOwner doesn't allow setting individual elements.");
			}
		}

		// Token: 0x17000D35 RID: 3381
		// (get) Token: 0x060050AB RID: 20651 RVA: 0x002987D4 File Offset: 0x00296BD4
		bool ICollection<Thing>.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060050AC RID: 20652 RVA: 0x002987EA File Offset: 0x00296BEA
		public virtual void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.maxStacks, "maxStacks", 999999, false);
			Scribe_Values.Look<LookMode>(ref this.contentsLookMode, "contentsLookMode", LookMode.Deep, false);
		}

		// Token: 0x060050AD RID: 20653 RVA: 0x00298818 File Offset: 0x00296C18
		public void ThingOwnerTick(bool removeIfDestroyed = true)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				Thing at = this.GetAt(i);
				if (at.def.tickerType == TickerType.Normal)
				{
					at.Tick();
					if (at.Destroyed && removeIfDestroyed)
					{
						this.Remove(at);
					}
				}
			}
		}

		// Token: 0x060050AE RID: 20654 RVA: 0x0029887C File Offset: 0x00296C7C
		public void ThingOwnerTickRare(bool removeIfDestroyed = true)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				Thing at = this.GetAt(i);
				if (at.def.tickerType == TickerType.Rare)
				{
					at.TickRare();
					if (at.Destroyed && removeIfDestroyed)
					{
						this.Remove(at);
					}
				}
			}
		}

		// Token: 0x060050AF RID: 20655 RVA: 0x002988E0 File Offset: 0x00296CE0
		public void ThingOwnerTickLong(bool removeIfDestroyed = true)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				Thing at = this.GetAt(i);
				if (at.def.tickerType == TickerType.Long)
				{
					at.TickRare();
					if (at.Destroyed && removeIfDestroyed)
					{
						this.Remove(at);
					}
				}
			}
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x00298944 File Offset: 0x00296D44
		public void Clear()
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.Remove(this.GetAt(i));
			}
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x0029897C File Offset: 0x00296D7C
		public void ClearAndDestroyContents(DestroyMode mode = DestroyMode.Vanish)
		{
			while (this.Any)
			{
				for (int i = this.Count - 1; i >= 0; i--)
				{
					Thing at = this.GetAt(i);
					at.Destroy(mode);
					this.Remove(at);
				}
			}
		}

		// Token: 0x060050B2 RID: 20658 RVA: 0x002989D0 File Offset: 0x00296DD0
		public void ClearAndDestroyContentsOrPassToWorld(DestroyMode mode = DestroyMode.Vanish)
		{
			while (this.Any)
			{
				for (int i = this.Count - 1; i >= 0; i--)
				{
					Thing at = this.GetAt(i);
					at.DestroyOrPassToWorld(mode);
					this.Remove(at);
				}
			}
		}

		// Token: 0x060050B3 RID: 20659 RVA: 0x00298A24 File Offset: 0x00296E24
		public bool CanAcceptAnyOf(Thing item, bool canMergeWithExistingStacks = true)
		{
			return this.GetCountCanAccept(item, canMergeWithExistingStacks) > 0;
		}

		// Token: 0x060050B4 RID: 20660 RVA: 0x00298A44 File Offset: 0x00296E44
		public virtual int GetCountCanAccept(Thing item, bool canMergeWithExistingStacks = true)
		{
			int result;
			if (item == null || item.stackCount <= 0)
			{
				result = 0;
			}
			else if (this.maxStacks == 999999)
			{
				result = item.stackCount;
			}
			else
			{
				int num = 0;
				if (this.Count < this.maxStacks)
				{
					num += (this.maxStacks - this.Count) * item.def.stackLimit;
				}
				if (num >= item.stackCount)
				{
					result = Mathf.Min(num, item.stackCount);
				}
				else
				{
					if (canMergeWithExistingStacks)
					{
						int i = 0;
						int count = this.Count;
						while (i < count)
						{
							Thing at = this.GetAt(i);
							if (at.stackCount < at.def.stackLimit && at.CanStackWith(item))
							{
								num += at.def.stackLimit - at.stackCount;
								if (num >= item.stackCount)
								{
									return Mathf.Min(num, item.stackCount);
								}
							}
							i++;
						}
					}
					result = Mathf.Min(num, item.stackCount);
				}
			}
			return result;
		}

		// Token: 0x060050B5 RID: 20661
		public abstract int TryAdd(Thing item, int count, bool canMergeWithExistingStacks = true);

		// Token: 0x060050B6 RID: 20662
		public abstract bool TryAdd(Thing item, bool canMergeWithExistingStacks = true);

		// Token: 0x060050B7 RID: 20663
		public abstract int IndexOf(Thing item);

		// Token: 0x060050B8 RID: 20664
		public abstract bool Remove(Thing item);

		// Token: 0x060050B9 RID: 20665
		protected abstract Thing GetAt(int index);

		// Token: 0x060050BA RID: 20666 RVA: 0x00298B70 File Offset: 0x00296F70
		public bool Contains(Thing item)
		{
			return item != null && item.holdingOwner == this;
		}

		// Token: 0x060050BB RID: 20667 RVA: 0x00298B9B File Offset: 0x00296F9B
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.Remove(this.GetAt(index));
		}

		// Token: 0x060050BC RID: 20668 RVA: 0x00298BCC File Offset: 0x00296FCC
		public int TryAddOrTransfer(Thing item, int count, bool canMergeWithExistingStacks = true)
		{
			int result;
			if (item == null)
			{
				Log.Warning("Tried to add or transfer null item to ThingOwner.", false);
				result = 0;
			}
			else if (item.holdingOwner != null)
			{
				result = item.holdingOwner.TryTransferToContainer(item, this, count, canMergeWithExistingStacks);
			}
			else
			{
				result = this.TryAdd(item, count, canMergeWithExistingStacks);
			}
			return result;
		}

		// Token: 0x060050BD RID: 20669 RVA: 0x00298C24 File Offset: 0x00297024
		public bool TryAddOrTransfer(Thing item, bool canMergeWithExistingStacks = true)
		{
			bool result;
			if (item == null)
			{
				Log.Warning("Tried to add or transfer null item to ThingOwner.", false);
				result = false;
			}
			else if (item.holdingOwner != null)
			{
				result = item.holdingOwner.TryTransferToContainer(item, this, canMergeWithExistingStacks);
			}
			else
			{
				result = this.TryAdd(item, canMergeWithExistingStacks);
			}
			return result;
		}

		// Token: 0x060050BE RID: 20670 RVA: 0x00298C7C File Offset: 0x0029707C
		public void TryAddRangeOrTransfer(IEnumerable<Thing> things, bool canMergeWithExistingStacks = true, bool destroyLeftover = false)
		{
			if (things != this)
			{
				ThingOwner thingOwner = things as ThingOwner;
				if (thingOwner != null)
				{
					thingOwner.TryTransferAllToContainer(this, canMergeWithExistingStacks);
					if (destroyLeftover)
					{
						thingOwner.ClearAndDestroyContents(DestroyMode.Vanish);
					}
				}
				else
				{
					IList<Thing> list = things as IList<Thing>;
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (!this.TryAddOrTransfer(list[i], canMergeWithExistingStacks) && destroyLeftover)
							{
								list[i].Destroy(DestroyMode.Vanish);
							}
						}
					}
					else
					{
						foreach (Thing thing in things)
						{
							if (!this.TryAddOrTransfer(thing, canMergeWithExistingStacks) && destroyLeftover)
							{
								thing.Destroy(DestroyMode.Vanish);
							}
						}
					}
				}
			}
		}

		// Token: 0x060050BF RID: 20671 RVA: 0x00298D78 File Offset: 0x00297178
		public int RemoveAll(Predicate<Thing> predicate)
		{
			int num = 0;
			for (int i = this.Count - 1; i >= 0; i--)
			{
				if (predicate(this.GetAt(i)))
				{
					this.Remove(this.GetAt(i));
					num++;
				}
			}
			return num;
		}

		// Token: 0x060050C0 RID: 20672 RVA: 0x00298DD4 File Offset: 0x002971D4
		public bool TryTransferToContainer(Thing item, ThingOwner otherContainer, bool canMergeWithExistingStacks = true)
		{
			return this.TryTransferToContainer(item, otherContainer, item.stackCount, canMergeWithExistingStacks) == item.stackCount;
		}

		// Token: 0x060050C1 RID: 20673 RVA: 0x00298E00 File Offset: 0x00297200
		public int TryTransferToContainer(Thing item, ThingOwner otherContainer, int count, bool canMergeWithExistingStacks = true)
		{
			Thing thing;
			return this.TryTransferToContainer(item, otherContainer, count, out thing, canMergeWithExistingStacks);
		}

		// Token: 0x060050C2 RID: 20674 RVA: 0x00298E24 File Offset: 0x00297224
		public int TryTransferToContainer(Thing item, ThingOwner otherContainer, int count, out Thing resultingTransferredItem, bool canMergeWithExistingStacks = true)
		{
			int result;
			if (!this.Contains(item))
			{
				Log.Error(string.Concat(new object[]
				{
					"Can't transfer item ",
					item,
					" because it's not here. owner=",
					this.owner.ToStringSafe<IThingHolder>()
				}), false);
				resultingTransferredItem = null;
				result = 0;
			}
			else if (otherContainer == this && count > 0)
			{
				resultingTransferredItem = item;
				result = item.stackCount;
			}
			else if (!otherContainer.CanAcceptAnyOf(item, canMergeWithExistingStacks))
			{
				resultingTransferredItem = null;
				result = 0;
			}
			else if (count <= 0)
			{
				resultingTransferredItem = null;
				result = 0;
			}
			else if (this.owner is Map || otherContainer.owner is Map)
			{
				Log.Warning("Can't transfer items to or from Maps directly. They must be spawned or despawned manually. Use TryAdd(item.SplitOff(count))", false);
				resultingTransferredItem = null;
				result = 0;
			}
			else
			{
				int num = Mathf.Min(item.stackCount, count);
				Thing thing = item.SplitOff(num);
				if (this.Contains(thing))
				{
					this.Remove(thing);
				}
				bool flag = otherContainer.TryAdd(thing, canMergeWithExistingStacks);
				if (flag)
				{
					resultingTransferredItem = thing;
					result = thing.stackCount;
				}
				else
				{
					resultingTransferredItem = null;
					if (!otherContainer.Contains(thing) && thing.stackCount > 0 && !thing.Destroyed)
					{
						int num2 = num - thing.stackCount;
						if (item != thing)
						{
							item.TryAbsorbStack(thing, false);
						}
						else
						{
							this.TryAdd(thing, false);
						}
						result = num2;
					}
					else
					{
						result = thing.stackCount;
					}
				}
			}
			return result;
		}

		// Token: 0x060050C3 RID: 20675 RVA: 0x00298FB0 File Offset: 0x002973B0
		public void TryTransferAllToContainer(ThingOwner other, bool canMergeWithExistingStacks = true)
		{
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.TryTransferToContainer(this.GetAt(i), other, canMergeWithExistingStacks);
			}
		}

		// Token: 0x060050C4 RID: 20676 RVA: 0x00298FEC File Offset: 0x002973EC
		public Thing Take(Thing thing, int count)
		{
			Thing result;
			if (!this.Contains(thing))
			{
				Log.Error("Tried to take " + thing.ToStringSafe<Thing>() + " but it's not here.", false);
				result = null;
			}
			else
			{
				if (count > thing.stackCount)
				{
					Log.Error(string.Concat(new object[]
					{
						"Tried to get ",
						count,
						" of ",
						thing.ToStringSafe<Thing>(),
						" while only having ",
						thing.stackCount
					}), false);
					count = thing.stackCount;
				}
				if (count == thing.stackCount)
				{
					this.Remove(thing);
					result = thing;
				}
				else
				{
					Thing thing2 = thing.SplitOff(count);
					thing2.holdingOwner = null;
					result = thing2;
				}
			}
			return result;
		}

		// Token: 0x060050C5 RID: 20677 RVA: 0x002990BC File Offset: 0x002974BC
		public Thing Take(Thing thing)
		{
			return this.Take(thing, thing.stackCount);
		}

		// Token: 0x060050C6 RID: 20678 RVA: 0x002990E0 File Offset: 0x002974E0
		public bool TryDrop(Thing thing, ThingPlaceMode mode, int count, out Thing lastResultingThing, Action<Thing, int> placedAction = null, Predicate<IntVec3> nearPlaceValidator = null)
		{
			Map rootMap = ThingOwnerUtility.GetRootMap(this.owner);
			IntVec3 rootPosition = ThingOwnerUtility.GetRootPosition(this.owner);
			bool result;
			if (rootMap == null || !rootPosition.IsValid)
			{
				Log.Error("Cannot drop " + thing + " without a dropLoc and with an owner whose map is null.", false);
				lastResultingThing = null;
				result = false;
			}
			else
			{
				result = this.TryDrop(thing, rootPosition, rootMap, mode, count, out lastResultingThing, placedAction, nearPlaceValidator);
			}
			return result;
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x00299154 File Offset: 0x00297554
		public bool TryDrop(Thing thing, IntVec3 dropLoc, Map map, ThingPlaceMode mode, int count, out Thing resultingThing, Action<Thing, int> placedAction = null, Predicate<IntVec3> nearPlaceValidator = null)
		{
			bool result;
			if (!this.Contains(thing))
			{
				Log.Error("Tried to drop " + thing.ToStringSafe<Thing>() + " but it's not here.", false);
				resultingThing = null;
				result = false;
			}
			else
			{
				if (thing.stackCount < count)
				{
					Log.Error(string.Concat(new object[]
					{
						"Tried to drop ",
						count,
						" of ",
						thing,
						" while only having ",
						thing.stackCount
					}), false);
					count = thing.stackCount;
				}
				if (count == thing.stackCount)
				{
					if (GenDrop.TryDropSpawn(thing, dropLoc, map, mode, out resultingThing, placedAction, nearPlaceValidator))
					{
						this.Remove(thing);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					Thing thing2 = thing.SplitOff(count);
					if (GenDrop.TryDropSpawn(thing2, dropLoc, map, mode, out resultingThing, placedAction, nearPlaceValidator))
					{
						result = true;
					}
					else
					{
						thing.TryAbsorbStack(thing2, false);
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x00299264 File Offset: 0x00297664
		public bool TryDrop(Thing thing, ThingPlaceMode mode, out Thing lastResultingThing, Action<Thing, int> placedAction = null, Predicate<IntVec3> nearPlaceValidator = null)
		{
			Map rootMap = ThingOwnerUtility.GetRootMap(this.owner);
			IntVec3 rootPosition = ThingOwnerUtility.GetRootPosition(this.owner);
			bool result;
			if (rootMap == null || !rootPosition.IsValid)
			{
				Log.Error("Cannot drop " + thing + " without a dropLoc and with an owner whose map is null.", false);
				lastResultingThing = null;
				result = false;
			}
			else
			{
				result = this.TryDrop(thing, rootPosition, rootMap, mode, out lastResultingThing, placedAction, nearPlaceValidator);
			}
			return result;
		}

		// Token: 0x060050C9 RID: 20681 RVA: 0x002992D4 File Offset: 0x002976D4
		public bool TryDrop(Thing thing, IntVec3 dropLoc, Map map, ThingPlaceMode mode, out Thing lastResultingThing, Action<Thing, int> placedAction = null, Predicate<IntVec3> nearPlaceValidator = null)
		{
			bool result;
			if (!this.Contains(thing))
			{
				Log.Error(this.owner.ToStringSafe<IThingHolder>() + " container tried to drop  " + thing.ToStringSafe<Thing>() + " which it didn't contain.", false);
				lastResultingThing = null;
				result = false;
			}
			else if (GenDrop.TryDropSpawn(thing, dropLoc, map, mode, out lastResultingThing, placedAction, nearPlaceValidator))
			{
				this.Remove(thing);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060050CA RID: 20682 RVA: 0x00299350 File Offset: 0x00297750
		public bool TryDropAll(IntVec3 dropLoc, Map map, ThingPlaceMode mode, Action<Thing, int> placeAction = null, Predicate<IntVec3> nearPlaceValidator = null)
		{
			bool result = true;
			for (int i = this.Count - 1; i >= 0; i--)
			{
				Thing thing;
				if (!this.TryDrop(this.GetAt(i), dropLoc, map, mode, out thing, placeAction, nearPlaceValidator))
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060050CB RID: 20683 RVA: 0x002993A0 File Offset: 0x002977A0
		public bool Contains(ThingDef def)
		{
			return this.Contains(def, 1);
		}

		// Token: 0x060050CC RID: 20684 RVA: 0x002993C0 File Offset: 0x002977C0
		public bool Contains(ThingDef def, int minCount)
		{
			bool result;
			if (minCount <= 0)
			{
				result = true;
			}
			else
			{
				int num = 0;
				int count = this.Count;
				for (int i = 0; i < count; i++)
				{
					if (this.GetAt(i).def == def)
					{
						num += this.GetAt(i).stackCount;
					}
					if (num >= minCount)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060050CD RID: 20685 RVA: 0x00299430 File Offset: 0x00297830
		public int TotalStackCountOfDef(ThingDef def)
		{
			int num = 0;
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.GetAt(i).def == def)
				{
					num += this.GetAt(i).stackCount;
				}
			}
			return num;
		}

		// Token: 0x060050CE RID: 20686 RVA: 0x00299484 File Offset: 0x00297884
		public void Notify_ContainedItemDestroyed(Thing t)
		{
			if (ThingOwnerUtility.ShouldAutoRemoveDestroyedThings(this.owner))
			{
				this.Remove(t);
			}
		}

		// Token: 0x060050CF RID: 20687 RVA: 0x002994A0 File Offset: 0x002978A0
		protected void NotifyAdded(Thing item)
		{
			if (ThingOwnerUtility.ShouldAutoExtinguishInnerThings(this.owner) && item.HasAttachment(ThingDefOf.Fire))
			{
				item.GetAttachment(ThingDefOf.Fire).Destroy(DestroyMode.Vanish);
			}
			if (ThingOwnerUtility.ShouldRemoveDesignationsOnAddedThings(this.owner))
			{
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					maps[i].designationManager.RemoveAllDesignationsOn(item, false);
				}
			}
			CompTransporter compTransporter = this.owner as CompTransporter;
			if (compTransporter != null)
			{
				compTransporter.Notify_ThingAdded(item);
			}
			Caravan caravan = this.owner as Caravan;
			if (caravan != null)
			{
				caravan.Notify_PawnAdded((Pawn)item);
			}
			Pawn_ApparelTracker pawn_ApparelTracker = this.owner as Pawn_ApparelTracker;
			if (pawn_ApparelTracker != null)
			{
				pawn_ApparelTracker.Notify_ApparelAdded((Apparel)item);
			}
			Pawn_EquipmentTracker pawn_EquipmentTracker = this.owner as Pawn_EquipmentTracker;
			if (pawn_EquipmentTracker != null)
			{
				pawn_EquipmentTracker.Notify_EquipmentAdded((ThingWithComps)item);
			}
			this.NotifyColonistBarIfColonistCorpse(item);
		}

		// Token: 0x060050D0 RID: 20688 RVA: 0x002995A4 File Offset: 0x002979A4
		protected void NotifyAddedAndMergedWith(Thing item, int mergedCount)
		{
			CompTransporter compTransporter = this.owner as CompTransporter;
			if (compTransporter != null)
			{
				compTransporter.Notify_ThingAddedAndMergedWith(item, mergedCount);
			}
		}

		// Token: 0x060050D1 RID: 20689 RVA: 0x002995CC File Offset: 0x002979CC
		protected void NotifyRemoved(Thing item)
		{
			Pawn_InventoryTracker pawn_InventoryTracker = this.owner as Pawn_InventoryTracker;
			if (pawn_InventoryTracker != null)
			{
				pawn_InventoryTracker.Notify_ItemRemoved(item);
			}
			Pawn_ApparelTracker pawn_ApparelTracker = this.owner as Pawn_ApparelTracker;
			if (pawn_ApparelTracker != null)
			{
				pawn_ApparelTracker.Notify_ApparelRemoved((Apparel)item);
			}
			Pawn_EquipmentTracker pawn_EquipmentTracker = this.owner as Pawn_EquipmentTracker;
			if (pawn_EquipmentTracker != null)
			{
				pawn_EquipmentTracker.Notify_EquipmentRemoved((ThingWithComps)item);
			}
			Caravan caravan = this.owner as Caravan;
			if (caravan != null)
			{
				caravan.Notify_PawnRemoved((Pawn)item);
			}
			this.NotifyColonistBarIfColonistCorpse(item);
		}

		// Token: 0x060050D2 RID: 20690 RVA: 0x00299654 File Offset: 0x00297A54
		private void NotifyColonistBarIfColonistCorpse(Thing thing)
		{
			Corpse corpse = thing as Corpse;
			if (corpse != null && !corpse.Bugged && corpse.InnerPawn.Faction != null && corpse.InnerPawn.Faction.IsPlayer && Current.ProgramState == ProgramState.Playing)
			{
				Find.ColonistBar.MarkColonistsDirty();
			}
		}

		// Token: 0x060050D3 RID: 20691 RVA: 0x002996B6 File Offset: 0x00297AB6
		void IList<Thing>.Insert(int index, Thing item)
		{
			throw new InvalidOperationException("ThingOwner doesn't allow inserting individual elements at any position.");
		}

		// Token: 0x060050D4 RID: 20692 RVA: 0x002996C3 File Offset: 0x00297AC3
		void ICollection<Thing>.Add(Thing item)
		{
			this.TryAdd(item, true);
		}

		// Token: 0x060050D5 RID: 20693 RVA: 0x002996D0 File Offset: 0x00297AD0
		void ICollection<Thing>.CopyTo(Thing[] array, int arrayIndex)
		{
			for (int i = 0; i < this.Count; i++)
			{
				array[i + arrayIndex] = this.GetAt(i);
			}
		}

		// Token: 0x060050D6 RID: 20694 RVA: 0x00299704 File Offset: 0x00297B04
		IEnumerator<Thing> IEnumerable<Thing>.GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return this.GetAt(i);
			}
			yield break;
		}

		// Token: 0x060050D7 RID: 20695 RVA: 0x00299728 File Offset: 0x00297B28
		IEnumerator IEnumerable.GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++)
			{
				yield return this.GetAt(i);
			}
			yield break;
		}
	}
}
