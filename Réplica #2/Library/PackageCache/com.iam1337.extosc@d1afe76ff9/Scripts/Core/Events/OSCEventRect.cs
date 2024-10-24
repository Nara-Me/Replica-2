﻿/* Copyright (c) 2024 dr. ext (Vladimir Sigalkin) */

using UnityEngine;
using UnityEngine.Events;

namespace extOSC.Core.Events
{
	[System.Serializable]
	public class OSCEventRect : UnityEvent<Rect>
	{ }
}