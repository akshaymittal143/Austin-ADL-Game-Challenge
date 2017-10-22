using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;

public class ScenarioManager : MonoBehaviour
{
	Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();

	public Image backgroundImage;
	public TMP_Text dialogText;
	public Image dialogBackground;

	public string scriptName;

	Conversation currentConversation;

	ShowOptionsConversationAction currentOptionsAction;

	public string startConversation = "1";

	void Awake()
	{
		LoadScenario(scriptName);
	}

	void Start()
	{
		StartCoroutine(StartScenario());
	}

	IEnumerator StartScenario()
	{
		string nextConversation = startConversation;
		while (nextConversation != "") {
			print("Running conversation: " + nextConversation);
			currentConversation = conversations[nextConversation];
			yield return currentConversation.Execute();
			nextConversation = currentConversation.nextConversation;
		}
	}

	void LoadScenario(string name)
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Scenarios/"+ name);
		ParseScenarioText(textAsset.text);
	}

	void ParseScenarioText(string text)
	{
		string patConversation = @"\s*""\s*([a-zA-Z0-9]+)\s*""[ ]*\-+\s*([\s\S]+?)\-{3,}";

		// Instantiate the regular expression object.
		Regex r = new Regex(patConversation, RegexOptions.IgnoreCase | RegexOptions.Multiline);

		Match m = r.Match(text);
		while (m.Success) 
		{
			string conversationId = m.Groups[1].Value;
			string conversationContent = m.Groups[2].Value;
			ParseConversation(conversationContent, conversationId);
			m = m.NextMatch();
		}
	}

	void ParseConversation(string body, string id)
	{
		print("Parsing conversation: " + id);
		Conversation conversation = new Conversation();

		string patBackgroundImage = @"~\s*use scenario background ""\s*(.*?)\s*""";
		string patDialogBackground = @"~\s*use dialog background ""\s*(.*?)\s*""";
		string patMusic = @"~\s*use music ""\s*(.*?)\s*""";
		string patCharacterAction = @"~\s*character ""(.*?)""\s*(eases|fades){0,1}\s*(in|out){0,1}\s*(left|right|center){0,1}\s*(slowly|normally|fast){0,1}\s*with pose ""(.*?)""";
		string patSwitchCharacter = @"~\s*""(.*?)"" speaks";
		string patWriteParagraphCommand = @"-\s*(.*)";
		string patOptionCommand = @"~ show options";
		string patOption = @"-(.*?)=> ""(.*?)""";
		string patConversationLink = @"~\s*go to conversation ""(.*?)""";

		Regex r;
		Match m;
		bool waitingForContentFromCharacter = false;
		bool waitingForFullOptions = false;
		string speakingCharacterName = "";

		foreach (string line in body.Split('\n')) {

			Debug.Log("Processing line: " + line);
			// Check if line is a background image action.
			r = new Regex(patBackgroundImage, RegexOptions.IgnoreCase);
			m = r.Match(line);
			if (m.Success) 
			{
				SetBackgroundConversationAction action = new SetBackgroundConversationAction();
				action.backgroundImage = backgroundImage;
				action.spriteToSet = Resources.Load<Sprite>("Scenario Backgrounds/" + m.Groups[1].Value);
				conversation.AddAction(action);
				Debug.LogFormat("Background will change to {0}", m.Groups[1].Value);
				waitingForContentFromCharacter = false;
				waitingForFullOptions = false;
				continue; // Go to next line.
			}

			// Check if line is a dialog action.
			r = new Regex(patDialogBackground, RegexOptions.IgnoreCase);
			m = r.Match(line);
			if (m.Success) 
			{
				SetDialogBackgroundConversationAction action = new SetDialogBackgroundConversationAction();
				action.dialogImage = dialogBackground;
				action.spriteToSet = Resources.Load<Sprite>("Dialog Backgrounds/" + m.Groups[1].Value);
				conversation.AddAction(action);
				Debug.LogFormat("Dialog background will change to {0}", m.Groups[1].Value);
				waitingForContentFromCharacter = false;
				waitingForFullOptions = false;
				continue; // Go to next line.
			}

			// Check if line is a background music action.
			r = new Regex(patMusic, RegexOptions.IgnoreCase);
			m = r.Match(line);
			if (m.Success) 
			{
				Debug.LogFormat("Background music will change to {0}", m.Groups[1].Value);
				waitingForContentFromCharacter = false;
				waitingForFullOptions = false;
				continue; // Go to next line.
			}

			// Check if line is a conversation link.
			r = new Regex(patConversationLink, RegexOptions.IgnoreCase);
			m = r.Match(line);
			if (m.Success) 
			{
				Debug.LogFormat("Next conversation set to {0}", m.Groups[1].Value);
				waitingForContentFromCharacter = false;
				waitingForFullOptions = false;
				conversation.nextConversation = m.Groups[1].Value;
				continue; // Go to next line.
			}

			// Check if line is a character action.
			r = new Regex(patCharacterAction, RegexOptions.IgnoreCase);
			m = r.Match(line);
			if (m.Success)
			{
				// Change pose action.
				if (!m.Groups[2].Success) {
					string characterName = m.Groups[1].Value;
					string characterPose = m.Groups[6].Value;

					ChangeCharacterPoseConversationAction action = new ChangeCharacterPoseConversationAction();
					action.characterName = characterName;
					action.characterPose = characterPose;
					conversation.AddAction(action);

					Debug.LogFormat("{0} will change pose to {1}", characterName, characterPose);
				
				// Ease in or ease out action.
				} else {
					string characterName = m.Groups[1].Value;
					string easeMode = m.Groups[2].Value;
					string inOut = m.Groups[3].Value;
					string screenPosition = m.Groups[4].Value;
					string speed = m.Groups[5].Value;
					string characterPose = m.Groups[6].Value;

					if (inOut == "in") {
						EaseInCharacterConversationAction action = new EaseInCharacterConversationAction();
						action.characterName = characterName;
						action.SetEaseMode(easeMode);
						action.SetRootPosition(screenPosition);
						action.SetEaseSpeed(speed);
						action.startPose = characterPose;
						conversation.AddAction(action);
					}

					Debug.LogFormat("{0} will {1} {2} {3} at {4} with pose {5}", characterName, easeMode, inOut, speed, screenPosition, characterPose);
				}

				waitingForContentFromCharacter = false;
				waitingForFullOptions = false;
				continue; // Go to next line.
			}

			// If a character switch just happened we need to add some dialog.
			// Otherwise, just check if the line is a character switch command.
			if (waitingForContentFromCharacter) {
				// Check if line is a speak/write paragraph command.
				r = new Regex(patWriteParagraphCommand, RegexOptions.IgnoreCase);
				m = r.Match(line);
				if (m.Success) {
					WriteParagraphConversationAction action = new WriteParagraphConversationAction();
					action.message = m.Groups[1].Value;
					action.speakingCharacter = speakingCharacterName;
					action.dialogTextBox = dialogText;
					conversation.AddAction(action);
					Debug.LogFormat("Will write paragraph: {0}", action.message);
					waitingForContentFromCharacter = true;
					waitingForFullOptions = false;
					continue; // Go to next line.
				}
			} else {
				// Check if line is a character switch command.
				r = new Regex(patSwitchCharacter, RegexOptions.IgnoreCase);
				m = r.Match(line);
				if (m.Success) {
					SwitchCharacterConversationAction action = new SwitchCharacterConversationAction();
					action.characterName = m.Groups[1].Value;
					conversation.AddAction(action);
					Debug.LogFormat("Will switch to character: {0}", action.characterName);
					waitingForContentFromCharacter = true;
					waitingForFullOptions = false;
					speakingCharacterName = action.characterName;
					continue; // Go to next line.
				}
			}

			if (waitingForFullOptions) {
				// Check if line is an option.
				r = new Regex(patOption, RegexOptions.IgnoreCase);
				m = r.Match(line);
				if (m.Success) {
					string optionLabel = m.Groups[1].Value;
					string optionResultConversation = m.Groups[2].Value;
					currentOptionsAction.AddOption(optionLabel, optionResultConversation, conversation);
					Debug.LogFormat("Added option: {0} for result {1} to options panel.", optionLabel, optionResultConversation);
					waitingForFullOptions = true;
					waitingForContentFromCharacter = false;
					continue; // Go to next line.
				}
			} else {
				// Check if line is an options dialog command.
				r = new Regex(patOptionCommand, RegexOptions.IgnoreCase);
				m = r.Match(line);
				if (m.Success) {
					Debug.LogFormat("ScenarioManager:: Creating options dialog.");
					currentOptionsAction = new ShowOptionsConversationAction();
					conversation.AddAction(currentOptionsAction);
					waitingForFullOptions = true;
					waitingForContentFromCharacter = false;
					continue; // Go to next line.
				}
			}
		}

		waitingForContentFromCharacter = false;
		waitingForFullOptions = false;

		conversations.Add(id, conversation);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return)) {
			currentConversation.Command("continue");
		}
	}
}
