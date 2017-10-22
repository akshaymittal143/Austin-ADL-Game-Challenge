using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class OptionButton : MonoBehaviour
{
	public TMP_Text buttonLabel;
	string label;
	public string nextConversation;
	public Conversation parentConversation;
	Button button;
	public OptionPanel optionPanel;

	void Awake()
	{
		button = GetComponent<Button>();
		button.onClick.AddListener(OnClick);
	}

	void OnEnable()
	{
		//buttonLabel = GetComponentInChildren<TMP_Text>();
	}

	public void SetLabel(string label)
	{
		this.label = label;
		buttonLabel.text = label;
	}

	public void OnClick()
	{
		Debug.LogFormat("OptionButton:: selected option {0} setting next conversation as {1}.", label, nextConversation);
		parentConversation.nextConversation = nextConversation;
		optionPanel.SetActive(false);
	}
}
