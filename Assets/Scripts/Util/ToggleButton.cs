﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ToggleButton : Toggle
{
	//#region Inspector
	// ReSharper disable InconsistentNaming
	//Sprite normalSprite;
	// ReSharper restore InconsistentNaming
	//#endregion

	protected override void Start()
	{
		base.Start();
		//normalSprite = ((Image)targetGraphic).sprite;
		OnChangeValue (isOn);
		onValueChanged.AddListener(OnChangeValue);
	}

	private void OnChangeValue(bool flag)
	{
		switch (transition)
		{
		case Transition.ColorTint: 
			image.color = isOn ? colors.pressedColor : colors.disabledColor; 
			break;
		case Transition.SpriteSwap: 
			image.sprite = isOn ? spriteState.pressedSprite : spriteState.disabledSprite; 
			break;
		default: 
			throw new NotImplementedException();
		}
	}
}