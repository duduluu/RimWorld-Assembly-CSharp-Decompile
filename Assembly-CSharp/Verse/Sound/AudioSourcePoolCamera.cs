﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound
{
	// Token: 0x02000DBB RID: 3515
	public class AudioSourcePoolCamera
	{
		// Token: 0x06004E8D RID: 20109 RVA: 0x00290D40 File Offset: 0x0028F140
		public AudioSourcePoolCamera()
		{
			this.cameraSourcesContainer = new GameObject("OneShotSourcesCameraContainer");
			this.cameraSourcesContainer.transform.parent = Find.Camera.transform;
			this.cameraSourcesContainer.transform.localPosition = Vector3.zero;
			for (int i = 0; i < 16; i++)
			{
				AudioSource audioSource = AudioSourceMaker.NewAudioSourceOn(new GameObject("OneShotSourceCamera_" + i.ToString())
				{
					transform = 
					{
						parent = this.cameraSourcesContainer.transform,
						localPosition = Vector3.zero
					}
				});
				audioSource.bypassReverbZones = true;
				this.sourcesCamera.Add(audioSource);
			}
		}

		// Token: 0x06004E8E RID: 20110 RVA: 0x00290E10 File Offset: 0x0028F210
		public AudioSource GetSourceCamera()
		{
			for (int i = 0; i < this.sourcesCamera.Count; i++)
			{
				AudioSource audioSource = this.sourcesCamera[i];
				if (!audioSource.isPlaying)
				{
					audioSource.clip = null;
					SoundFilterUtility.DisableAllFiltersOn(audioSource);
					return audioSource;
				}
			}
			return null;
		}

		// Token: 0x04003447 RID: 13383
		public GameObject cameraSourcesContainer;

		// Token: 0x04003448 RID: 13384
		private List<AudioSource> sourcesCamera = new List<AudioSource>();

		// Token: 0x04003449 RID: 13385
		private const int NumSourcesCamera = 16;
	}
}
