using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LoadingScreenController : ScreenController
{
    [SerializeField] private Slider _loadingBar;
    [Tooltip("The amount of seconds it takes to lerp to the loading progress")]
    [SerializeField] private float _lerpDuration = 0.2f;

    private float _lerpValue; // Lerp interpolater

    #region Injected

    private SceneLoader _sceneLoader;

    #endregion

    [Inject]
    private void Construct(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    private void Update()
    {
        _lerpValue = Time.deltaTime / _lerpDuration;

        if (_sceneLoader.CurrentAsyncOperation != null)
        {
            // Lerp?
            _loadingBar.value = Mathf.Lerp(_loadingBar.value, _sceneLoader.CurrentAsyncOperation.progress + 0.1f, _lerpValue);
        }
    }
}
