using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public event Action BeforeSceneUnload;
	public event Action AfterSceneLoad;

	[SerializeField]
	private string startingSceneName;

	[SerializeField]
	private bool doNotLoadStartScene;

	public static SceneLoader instance;

	private bool isBusySwitching = false;

	private IEnumerator Start()
	{
		instance = this;
		if (doNotLoadStartScene) yield break;
		yield return LoadSceneIter(startingSceneName);
	}

	private IEnumerator LoadSceneIter(string sceneName)
	{
		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
		SceneManager.SetActiveScene(loadedScene);
	}

	private IEnumerator SwitchSceneIter(string nextSceneName)
	{
		isBusySwitching = true;

		if (BeforeSceneUnload != null) BeforeSceneUnload();

		// Unload current scene.
		Scene currentScene = SceneManager.GetActiveScene();
		yield return SceneManager.UnloadSceneAsync(currentScene.buildIndex);

		// Load next scene.
		yield return LoadSceneIter(nextSceneName);

		if (AfterSceneLoad != null) AfterSceneLoad();

		isBusySwitching = false;
	}

	public void LoadSceneAsync(string sceneName)
	{
		if (!isBusySwitching)
		{
			// We assume an initial scene will have been loaded in Start() and
			// call SwitchSceneIter to switch it out with the next scene to load.
			StartCoroutine(SwitchSceneIter(sceneName));
		}
		else
		{
			Debug.LogError("SceneLoader:: Busy, cannot load scene: " + sceneName);
		}
	}
}