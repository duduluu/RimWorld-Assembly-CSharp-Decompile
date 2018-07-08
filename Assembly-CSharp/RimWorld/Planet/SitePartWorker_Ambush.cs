﻿using System;

namespace RimWorld.Planet
{
	public class SitePartWorker_Ambush : SitePartWorker
	{
		private const float ThreatPointsFactor = 0.8f;

		public SitePartWorker_Ambush()
		{
		}

		public override SiteCoreOrPartParams GenerateDefaultParams(Site site)
		{
			SiteCoreOrPartParams siteCoreOrPartParams = base.GenerateDefaultParams(site);
			siteCoreOrPartParams.threatPoints *= 0.8f;
			return siteCoreOrPartParams;
		}
	}
}