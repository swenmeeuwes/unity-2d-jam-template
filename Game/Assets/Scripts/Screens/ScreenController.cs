using System;
using System.Collections;
using UnityEngine;
using Zenject;

public abstract class ScreenController : MonoBehaviour
{
    protected Animator Animator;

    private ScreenState _state = ScreenState.Hidden;
    protected ScreenState State {
        get { return _state; }
        set
        {
            _state = value;

            // todo: Make this a lower level local signal bus (Implement observer pattern?)            
            SignalBus.Fire(new ScreenStateChangedSignal
            {
                ScreenController = this,
                ScreenState = value
            });
        }
    }

    #region Injected

    protected SignalBus SignalBus;

    #endregion

    [Inject]
    public virtual void Construct(SignalBus signalBus)
    {
        SignalBus = signalBus;
    }

    protected virtual void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public virtual ScreenController Open()
    {
        State = ScreenState.Opening;

        if (Animator)
            Animator.SetTrigger("Open");
        else
            OpenAnimationFinished();

        return this;
    }

    public virtual ScreenController Close()
    {
        State = ScreenState.Closing;

        if (Animator)
            Animator.SetTrigger("Close");
        else
            CloseAnimationFinished();

        return this;
    }

    public virtual void Focus()
    {
        
    }

    public virtual void Blur()
    {
        
    }

    // Called from animation event
    public virtual void OpenAnimationFinished()
    {
        State = ScreenState.Ready;
    }

    // Called from animation event
    public virtual void CloseAnimationFinished()
    {
        State = ScreenState.Closed;
    }    
}
