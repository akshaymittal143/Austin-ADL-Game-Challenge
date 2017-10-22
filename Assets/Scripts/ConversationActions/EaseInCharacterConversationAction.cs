using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

	public AnimationCurve easeInCurve;
	public EaseMode easeInMode;
	public float easeInSpeed;
	public string characterName;
	public CharacterManager.RootPosition rootPosition;
	public string startPose;

	public override IEnumerator Execute()
	{
		CharacterManager.instance.LoadCharacter(rootPosition, characterName, startPose);

		// TODO Position off screen.
		// TODO Ease in character.
		yield return null;
	}

	public void SetEaseSpeed(string easeSpeed)
	{
		switch (easeSpeed) {
		case "slowly":
			{
				easeInSpeed = 3.0f;
				break;
			}
		case "normally":
			{
				easeInSpeed = 1.0f;
				break;
			}
		case "fast":
			{
				easeInSpeed = 0.4f;
				break;
			}
		default:
			{
				Debug.LogFormat("EaseInCharacterConversationAction:: did not understand speed {0}.", easeSpeed);
				easeInSpeed = 0f;
				break;
			}
		}
	}

	public void SetEaseMode(string mode)
	{
		switch (mode) {
		case "slides":
			{
				easeInMode = EaseMode.Slide;
				break;
			}
		case "fades":
			{
				easeInMode = EaseMode.Fade;
				break;
			}
		default:
			{
				Debug.LogFormat("EaseInCharacterConversationAction:: did not understand ease mode {0}", mode);
				break;
			}
		}
	}

	public void SetRootPosition(string position)
	{
		switch (position) {
		case "left":
			{
				rootPosition = CharacterManager.RootPosition.Left;
				break;
			}
		case "center":
			{
				rootPosition = CharacterManager.RootPosition.Center;
				break;
			}
		case "right":
			{
				rootPosition = CharacterManager.RootPosition.Right;
				break;
			}
		default:
			{
				Debug.LogFormat("EaseInCharacterConversationAction:: did not understand root position {0}", position);
				break;
			}
		}
	}
}
