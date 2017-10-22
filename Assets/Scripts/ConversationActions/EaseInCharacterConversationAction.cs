using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EaseInCharacterConversationAction : ConversationAction
{
	public enum EaseMode
	{
		Fade,
		Slide
	}

	public enum EaseSpeed
	{
		Slow,
		Normal,
		Fast
	}

	public enum RootPosition
	{
		Left,
		Center,
		Right
	}

	public AnimationCurve easeIn;
	public EaseMode easeInMode;
	public float easeInSpeed;
	public AnimationCurve easeOut;
	public EaseMode easeOutMode;
	public string characterName;
	public string characterAlias;
	public RootPosition rootPosition;

	public override IEnumerator Execute()
	{
		Debug.Log("Easing in character " + characterName);
		yield return null;
	}

	public float EaseSpeedToSeconds(EaseSpeed easeSpeed)
	{
		switch (easeSpeed) {
		case EaseSpeed.Slow:
			{
				return 3.0f;
			}
		case EaseSpeed.Normal:
			{
				return 1.0f;
			}
		case EaseSpeed.Fast:
			{
				return 0.4f;
			}
		}
		return 0f;
	}
}
