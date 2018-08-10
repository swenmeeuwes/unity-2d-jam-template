using UnityEngine;
using Zenject;

public class StartScreenController : MonoBehaviour
{
    [Tooltip("Scene to load after start screen")][SerializeField] private Scenes _nextScene;

    #region Injected

    private SignalBus _signalBus;

    #endregion

    private bool _isLoading = false; // to prevent the loading of more than one scene

    [Inject]
    private void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Update()
    {
        if (Input.anyKey && !_isLoading)
        {
            _isLoading = true;
            _signalBus.Fire(new LoadSceneSignal
            {
                Scene = Scenes.Game
            });
        }
    }
}
