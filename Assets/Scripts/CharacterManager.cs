using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour
{
	public static CharacterManager instance;

	public enum RootPosition
	{
		Left,
		Center,
		Right
	}

	Dictionary<string, Image> activeCharacters = new Dictionary<string, Image>();

	void Awake()
	{
		instance = this;
	}

	public void LoadPoseForExistingCharacter(string characterName, string characterPose)
	{
		Image img = activeCharacters[characterName];
		img.sprite = Resources.Load<Sprite>(
			string.Format("Character Poses/{0}/{0}_{1}", characterName, characterPose)
		);
		Debug.Log("Attempted to load: " + string.Format("Character Poses/{0}/{0}_{1}", characterName, characterPose));
	}

	public void LoadCharacter(RootPosition rootPosition, string characterName, string pose)
	{
		// Load character image.
		switch (rootPosition) {
		case RootPosition.Left:
			{
				Image img = GameObject.Find("Image, Left Character").GetComponent<Image>();
				GameObject characterLabel = GameObject.Find("Left Character Name");
				Image characterLabelBg = characterLabel.GetComponent<Image>();
				characterLabelBg.enabled = true;
				TMP_Text nameLabel = characterLabel.GetComponentInChildren<TMP_Text>(true);
				nameLabel.text = characterName;
				nameLabel.enabled = true;
				characterLabel.SetActive(true);
				Debug.Log("Found character image location: " + img);
				img.sprite = Resources.Load<Sprite>(
					string.Format("Character Poses/{0}/{0}_{1}", characterName, pose)
				);

				Debug.Log("Attempted to load: " + string.Format("Character Poses/{0}/{0}_{1}", characterName, pose));
				img.enabled = true;
				activeCharacters.Add(characterName, img);
				break;
			}
		case RootPosition.Center:
			{
				Image img = GameObject.Find("Image, Center Character").GetComponent<Image>();
				GameObject characterLabel = GameObject.Find("Left Character Name");
				Image characterLabelBg = characterLabel.GetComponent<Image>();
				characterLabelBg.enabled = true;
				TMP_Text nameLabel = characterLabel.GetComponentInChildren<TMP_Text>(true);
				nameLabel.text = characterName;
				nameLabel.enabled = true;
				characterLabel.SetActive(true);
				img.sprite = Resources.Load<Sprite>(
					string.Format("Character Poses/{0}/{0}_{1}", characterName, pose)
				);
				img.enabled = true;
				activeCharacters.Add(characterName, img);
				break;
			}
		case RootPosition.Right:
			{
				Image img = GameObject.Find("Image, Right Character").GetComponent<Image>();
				GameObject characterLabel = GameObject.Find("Right Character Name");
				Image characterLabelBg = characterLabel.GetComponent<Image>();
				characterLabelBg.enabled = true;
				TMP_Text nameLabel = characterLabel.GetComponentInChildren<TMP_Text>(true);
				nameLabel.text = characterName;
				nameLabel.enabled = true;
				characterLabel.SetActive(true);
				img.sprite = Resources.Load<Sprite>(
					string.Format("Character Poses/{0}/{0}_{1}", characterName, pose)
				);
				img.enabled = true;
				activeCharacters.Add(characterName, img);
				break;
			}
		}
	}
}
