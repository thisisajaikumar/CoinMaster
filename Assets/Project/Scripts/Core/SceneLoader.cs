using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Configuration")]
    [SerializeField] private LoadingScreen loadingScreenPrefab;
    [SerializeField] private float minLoadingTime = 1.5f;
    [SerializeField] private float maxLoadingTime = 3f;

    private LoadingScreen currentLoadingScreen;
    private bool isLoading = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName, bool useLoading = true)
    {
        if (isLoading) return;

        if (useLoading)
        {
            StartCoroutine(LoadSceneWithLoading(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadSceneWithLoading(string sceneName)
    {
        isLoading = true;

        // Show loading screen
        yield return StartCoroutine(ShowLoadingScreen());

        // Minimum loading time for UX
        float loadingTime = Random.Range(minLoadingTime, maxLoadingTime);
        yield return new WaitForSeconds(loadingTime);

        // Load scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // Update loading progress
            if (currentLoadingScreen != null)
            {
                currentLoadingScreen.UpdateProgress(asyncLoad.progress);
            }
            yield return null;
        }

        // Hide loading screen
        yield return StartCoroutine(HideLoadingScreen());

        isLoading = false;
    }

    private IEnumerator ShowLoadingScreen()
    {
        if (loadingScreenPrefab != null)
        {
            currentLoadingScreen = Instantiate(loadingScreenPrefab);
            DontDestroyOnLoad(currentLoadingScreen.gameObject);
            yield return currentLoadingScreen.Show();
        }
    }

    private IEnumerator HideLoadingScreen()
    {
        if (currentLoadingScreen != null)
        {
            yield return currentLoadingScreen.Hide();
            Destroy(currentLoadingScreen.gameObject);
            currentLoadingScreen = null;
        }
    }

    public bool IsLoading => isLoading;
}