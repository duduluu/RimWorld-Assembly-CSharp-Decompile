﻿using System;
using System.Diagnostics;

namespace Verse.Noise
{
	// Token: 0x02000FA1 RID: 4001
	public class Subtract : ModuleBase
	{
		// Token: 0x0600608C RID: 24716 RVA: 0x0030F5B9 File Offset: 0x0030D9B9
		public Subtract() : base(2)
		{
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x0030F5C3 File Offset: 0x0030D9C3
		public Subtract(ModuleBase lhs, ModuleBase rhs) : base(2)
		{
			this.modules[0] = lhs;
			this.modules[1] = rhs;
		}

		// Token: 0x0600608E RID: 24718 RVA: 0x0030F5E0 File Offset: 0x0030D9E0
		public override double GetValue(double x, double y, double z)
		{
			Debug.Assert(this.modules[0] != null);
			Debug.Assert(this.modules[1] != null);
			return this.modules[0].GetValue(x, y, z) - this.modules[1].GetValue(x, y, z);
		}
	}
}
