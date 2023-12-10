using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Enumeration of different scenes in the game.
/// </summary>
public enum SceneEnum
{
    MainMenu,
    Credits,
    CharacterSelection,
    DayManagmentScene,
    NightManagementScene,
    FloorIsLava_Minigame,
    RunnerAfter_Minigame
}

/// <summary>
/// The SceneManager class manages scene transitions and provides events for scene changes.
/// </summary>
public class SceneManager : Singleton<SceneManager>
{
    /// <summary>
    /// Event triggered when the scene changes.
    /// </summary>
    public Action OnSceneChanged;

    /// <summary>
    /// Event triggered when the new scene is fully loaded.
    /// </summary>
    public Action OnSceneFullyLoaded;

    [SerializeField] FadeScreen fadeScreen;

    /// <summary>
    /// Loads the scene with the specified index.
    /// </summary>
    /// <param name="sceneIndex">Index of the scene to load.</param>
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneRoutine(sceneIndex));
    }

    /// <summary>
    /// Loads the scene based on the provided SceneEnum.
    /// </summary>
    /// <param name="sceneEnum">Enumeration representing the scene to load.</param>
    public void LoadScene(SceneEnum sceneEnum)
    {
        int sceneIndex = (int)sceneEnum;
        LoadScene(sceneIndex);
    }

    /// <summary>
    /// Coroutine that handles the loading of a scene with a fade effect.
    /// </summary>
    /// <param name="sceneIndex">Index of the scene to load.</param>
    /// <returns>Yield instruction.</returns>
    IEnumerator LoadSceneRoutine(int sceneIndex)
    {
        // Fade out the screen before loading the new scene
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.FadeDuration);

        // Load the scene asynchronously
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        operation.allowSceneActivation = false;

        float timer = 0f;
        // Wait for the fade duration or until the operation is done
        while (timer <= fadeScreen.FadeDuration && !operation.isDone)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Trigger the OnSceneChanged event
        OnSceneChanged?.Invoke();
        operation.allowSceneActivation = true;

        // Fade in the screen after the new scene is loaded
        fadeScreen.FadeIn();

        // Trigger the OnSceneFullyLoaded event
        OnSceneFullyLoaded?.Invoke();
    }
}