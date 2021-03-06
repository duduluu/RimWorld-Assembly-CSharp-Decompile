﻿using System;
using UnityEngine;

namespace Verse
{
	public class MoteProperties
	{
		public bool realTime;

		public float fadeInTime;

		public float solidTime = 1f;

		public float fadeOutTime;

		public Vector3 acceleration = Vector3.zero;

		public float speedPerTime;

		public float growthRate;

		public bool collide;

		public SoundDef landSound;

		public Vector3 attachedDrawOffset;

		public bool needsMaintenance;

		public bool rotateTowardsTarget;

		public bool rotateTowardsMoveDirection;

		public MoteProperties()
		{
		}

		public float Lifespan
		{
			get
			{
				return this.fadeInTime + this.solidTime + this.fadeOutTime;
			}
		}
	}
}
