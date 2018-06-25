﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse
{
	// Token: 0x02000C05 RID: 3077
	public class MapFileCompressor : IExposable
	{
		// Token: 0x04002E04 RID: 11780
		private Map map;

		// Token: 0x04002E05 RID: 11781
		private byte[] compressedData;

		// Token: 0x04002E06 RID: 11782
		public CompressibilityDecider compressibilityDecider;

		// Token: 0x06004346 RID: 17222 RVA: 0x002393F6 File Offset: 0x002377F6
		public MapFileCompressor(Map map)
		{
			this.map = map;
		}

		// Token: 0x06004347 RID: 17223 RVA: 0x00239406 File Offset: 0x00237806
		public void ExposeData()
		{
			DataExposeUtility.ByteArray(ref this.compressedData, "compressedThingMap");
		}

		// Token: 0x06004348 RID: 17224 RVA: 0x00239419 File Offset: 0x00237819
		public void BuildCompressedString()
		{
			this.compressibilityDecider = new CompressibilityDecider(this.map);
			this.compressibilityDecider.DetermineReferences();
			this.compressedData = MapSerializeUtility.SerializeUshort(this.map, new Func<IntVec3, ushort>(this.HashValueForSquare));
		}

		// Token: 0x06004349 RID: 17225 RVA: 0x00239458 File Offset: 0x00237858
		private ushort HashValueForSquare(IntVec3 curSq)
		{
			ushort num = 0;
			foreach (Thing thing in this.map.thingGrid.ThingsAt(curSq))
			{
				if (thing.IsSaveCompressible())
				{
					if (num != 0)
					{
						Log.Error(string.Concat(new object[]
						{
							"Found two compressible things in ",
							curSq,
							". The last was ",
							thing
						}), false);
					}
					num = thing.def.shortHash;
				}
			}
			return num;
		}

		// Token: 0x0600434A RID: 17226 RVA: 0x00239510 File Offset: 0x00237910
		public IEnumerable<Thing> ThingsToSpawnAfterLoad()
		{
			Dictionary<ushort, ThingDef> thingDefsByShortHash = new Dictionary<ushort, ThingDef>();
			foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs)
			{
				if (thingDefsByShortHash.ContainsKey(thingDef.shortHash))
				{
					Log.Error(string.Concat(new object[]
					{
						"Hash collision between ",
						thingDef,
						" and  ",
						thingDefsByShortHash[thingDef.shortHash],
						": both have short hash ",
						thingDef.shortHash
					}), false);
				}
				else
				{
					thingDefsByShortHash.Add(thingDef.shortHash, thingDef);
				}
			}
			int major = VersionControl.MajorFromVersionString(ScribeMetaHeaderUtility.loadedGameVersion);
			int minor = VersionControl.MinorFromVersionString(ScribeMetaHeaderUtility.loadedGameVersion);
			List<Thing> loadables = new List<Thing>();
			MapSerializeUtility.LoadUshort(this.compressedData, this.map, delegate(IntVec3 c, ushort val)
			{
				if (val != 0)
				{
					ThingDef thingDef2 = BackCompatibility.BackCompatibleThingDefWithShortHash_Force(val, major, minor);
					if (thingDef2 == null)
					{
						try
						{
							thingDef2 = thingDefsByShortHash[val];
						}
						catch (KeyNotFoundException)
						{
							ThingDef thingDef3 = BackCompatibility.BackCompatibleThingDefWithShortHash(val);
							if (thingDef3 != null)
							{
								thingDef2 = thingDef3;
								thingDefsByShortHash.Add(val, thingDef3);
							}
							else
							{
								Log.Error("Map compressor decompression error: No thingDef with short hash " + val + ". Adding as null to dictionary.", false);
								thingDefsByShortHash.Add(val, null);
							}
						}
					}
					if (thingDef2 != null)
					{
						try
						{
							Thing thing = ThingMaker.MakeThing(thingDef2, null);
							thing.SetPositionDirect(c);
							loadables.Add(thing);
						}
						catch (Exception arg)
						{
							Log.Error("Could not instantiate compressed thing: " + arg, false);
						}
					}
				}
			});
			return loadables;
		}
	}
}
