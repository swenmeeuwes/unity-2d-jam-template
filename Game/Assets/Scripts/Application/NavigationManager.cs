using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[Serializable]
public class ScreenStackItem
{
    public ScreenType Type;
    public ScreenController ScreenController;
}

public class NavigationManager : IInitializable, IDisposable, ITickable
{
    private readonly List<ScreenStackItem> _navigationStack = new List<ScreenStackItem>();

    private ScreenStackItem _currentFocussed;

    #region Injected

    private readonly SignalBus _signalBus;
    private readonly ScreenFactory _screenFactory;

    #endregion

    public NavigationManager(SignalBus signalBus, ScreenFactory screenFactory)
    {
        _signalBus = signalBus;
        _screenFactory = screenFactory;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<OpenScreenRequestSignal>(OnOpenScreenRequest);
        _signalBus.Subscribe<CloseScreenRequestSignal>(OnCloseScreenRequest);
        _signalBus.Subscribe<CloseAllScreensRequestSignal>(OnCloseAllScreensRequest);
        _signalBus.Subscribe<ScreenStateChangedSignal>(OnScreenStateChanged);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<OpenScreenRequestSignal>(OnOpenScreenRequest);
        _signalBus.Unsubscribe<CloseScreenRequestSignal>(OnCloseScreenRequest);
        _signalBus.Unsubscribe<CloseAllScreensRequestSignal>(OnCloseAllScreensRequest);
        _signalBus.Unsubscribe<ScreenStateChangedSignal>(OnScreenStateChanged);
    }

    public void Tick()
    {
        // Check if the Android back button is held down
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_navigationStack.Count > 1)
            {
                var topScreen = _navigationStack.Last();
                topScreen.ScreenController.Close();
            }
        }
    }    

    private void OnOpenScreenRequest(OpenScreenRequestSignal signal)
    {
        // First check if the type of screen is already opened (unless it's forced by setting the 'ForceOpen' property)
        if (!signal.ForceOpen)
        {
            var screenExistsInNavigationStack = _navigationStack.Any(item => item.Type == signal.Type);
            if (screenExistsInNavigationStack)
                return; // If the type of screen does exists in the navigation stack we ignore the request
        }

        var screenController = _screenFactory.Create(signal.Type);

        // Blur the previous screen (if there is one)
        if (_navigationStack.Count > 0)
            _navigationStack.Last().ScreenController.Blur();

        // Push the new screen to the navigation stack and 'open' it
        var newScreenStackItem = new ScreenStackItem
        {
            Type = signal.Type,
            ScreenController = screenController
        };
        _navigationStack.Push(newScreenStackItem);

        // Focus the new screen
        screenController.Focus();
        _currentFocussed = newScreenStackItem;

        screenController.Open();
    }

    private void OnCloseScreenRequest(CloseScreenRequestSignal signal)
    {
        var subjectScreenStackItem = _navigationStack.FirstOrDefault(screenStackItem => screenStackItem.ScreenController == signal.ScreenController);
        if (subjectScreenStackItem == null)
        {
            Debug.LogWarningFormat("Trying to close a screen '{0}', but it doesn't exist in the navigation stack!", signal.ScreenController.GetType().ToString());
            return;
        }

        // If the closed screen was the screen in front, focus the next screen
        var frontScreen = _navigationStack.Last();
        if (signal.ScreenController == frontScreen.ScreenController)
        {
            var newFrontScreenStackItem = _navigationStack[_navigationStack.Count - 2];
            newFrontScreenStackItem.ScreenController.Focus();
            _currentFocussed = newFrontScreenStackItem;
        }

        // Close the screen
        subjectScreenStackItem.ScreenController.Close();
    }

    private void OnCloseAllScreensRequest(CloseAllScreensRequestSignal signal)
    {
        _navigationStack.ForEach(screenItem => screenItem.ScreenController.Close());

        if (signal.ForceInstant)
            _navigationStack.ForEach(screenItem => screenItem.ScreenController.CloseAnimationFinished());
    }

    private void OnScreenStateChanged(ScreenStateChangedSignal signal)
    {
        switch (signal.ScreenState)
        {
            case ScreenState.Hidden:
                break;
            case ScreenState.Opening:
                break;
            case ScreenState.Ready:
                break;
            case ScreenState.Closing:
                break;
            case ScreenState.Closed:
                RemoveScreenFromNavigationStack(signal.ScreenController);
                break;
        }
    }

    private void RemoveScreenFromNavigationStack(ScreenController screenController)
    {
        var subjectScreenStackItem = _navigationStack.FirstOrDefault(screenStackItem => screenStackItem.ScreenController == screenController);
        if (subjectScreenStackItem == null)
        {
            Debug.LogWarningFormat("Trying to close a screen '{0}', but it doesn't exist in the navigation stack!", screenController.GetType().ToString());
            return;
        }

        _navigationStack.Remove(subjectScreenStackItem);
        GameObject.Destroy(subjectScreenStackItem.ScreenController.gameObject);
    }
}