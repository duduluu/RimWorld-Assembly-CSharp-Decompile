﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	// Token: 0x02000424 RID: 1060
	public static class PowerConnectionMaker
	{
		// Token: 0x04000B38 RID: 2872
		private const int ConnectMaxDist = 6;

		// Token: 0x06001272 RID: 4722 RVA: 0x0009FD94 File Offset: 0x0009E194
		public static void ConnectAllConnectorsToTransmitter(CompPower newTransmitter)
		{
			foreach (CompPower compPower in PowerConnectionMaker.PotentialConnectorsForTransmitter(newTransmitter))
			{
				if (compPower.connectParent == null)
				{
					compPower.ConnectToTransmitter(newTransmitter, false);
				}
			}
		}

		// Token: 0x06001273 RID: 4723 RVA: 0x0009FE00 File Offset: 0x0009E200
		public static void DisconnectAllFromTransmitterAndSetWantConnect(CompPower deadPc, Map map)
		{
			if (deadPc.connectChildren != null)
			{
				for (int i = 0; i < deadPc.connectChildren.Count; i++)
				{
					CompPower compPower = deadPc.connectChildren[i];
					compPower.connectParent = null;
					CompPowerTrader compPowerTrader = compPower as CompPowerTrader;
					if (compPowerTrader != null)
					{
						compPowerTrader.PowerOn = false;
					}
					map.powerNetManager.Notify_ConnectorWantsConnect(compPower);
				}
			}
		}

		// Token: 0x06001274 RID: 4724 RVA: 0x0009FE70 File Offset: 0x0009E270
		public static void TryConnectToAnyPowerNet(CompPower pc, List<PowerNet> disallowedNets = null)
		{
			if (pc.connectParent == null)
			{
				if (pc.parent.Spawned)
				{
					CompPower compPower = PowerConnectionMaker.BestTransmitterForConnector(pc.parent.Position, pc.parent.Map, disallowedNets);
					if (compPower != null)
					{
						pc.ConnectToTransmitter(compPower, false);
					}
					else
					{
						pc.connectParent = null;
					}
				}
			}
		}

		// Token: 0x06001275 RID: 4725 RVA: 0x0009FEDC File Offset: 0x0009E2DC
		public static void DisconnectFromPowerNet(CompPower pc)
		{
			if (pc.connectParent != null)
			{
				if (pc.PowerNet != null)
				{
					pc.PowerNet.DeregisterConnector(pc);
				}
				if (pc.connectParent.connectChildren != null)
				{
					pc.connectParent.connectChildren.Remove(pc);
					if (pc.connectParent.connectChildren.Count == 0)
					{
						pc.connectParent.connectChildren = null;
					}
				}
				pc.connectParent = null;
			}
		}

		// Token: 0x06001276 RID: 4726 RVA: 0x0009FF60 File Offset: 0x0009E360
		private static IEnumerable<CompPower> PotentialConnectorsForTransmitter(CompPower b)
		{
			if (!b.parent.Spawned)
			{
				Log.Warning("Can't check potential connectors for " + b + " because it's unspawned.", false);
				yield break;
			}
			CellRect rect = b.parent.OccupiedRect().ExpandedBy(6).ClipInsideMap(b.parent.Map);
			for (int z = rect.minZ; z <= rect.maxZ; z++)
			{
				for (int x = rect.minX; x <= rect.maxX; x++)
				{
					IntVec3 c = new IntVec3(x, 0, z);
					List<Thing> thingList = b.parent.Map.thingGrid.ThingsListAt(c);
					for (int i = 0; i < thingList.Count; i++)
					{
						if (thingList[i].def.ConnectToPower)
						{
							yield return ((Building)thingList[i]).PowerComp;
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x06001277 RID: 4727 RVA: 0x0009FF8C File Offset: 0x0009E38C
		public static CompPower BestTransmitterForConnector(IntVec3 connectorPos, Map map, List<PowerNet> disallowedNets = null)
		{
			CellRect cellRect = CellRect.SingleCell(connectorPos).ExpandedBy(6).ClipInsideMap(map);
			cellRect.ClipInsideMap(map);
			float num = 999999f;
			CompPower result = null;
			for (int i = cellRect.minZ; i <= cellRect.maxZ; i++)
			{
				for (int j = cellRect.minX; j <= cellRect.maxX; j++)
				{
					IntVec3 c = new IntVec3(j, 0, i);
					Building transmitter = c.GetTransmitter(map);
					if (transmitter != null && !transmitter.Destroyed)
					{
						CompPower powerComp = transmitter.PowerComp;
						if (powerComp != null && powerComp.TransmitsPowerNow && (transmitter.def.building == null || transmitter.def.building.allowWireConnection))
						{
							if (disallowedNets == null || !disallowedNets.Contains(powerComp.transNet))
							{
								float num2 = (float)(transmitter.Position - connectorPos).LengthHorizontalSquared;
								if (num2 < num)
								{
									num = num2;
									result = powerComp;
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
