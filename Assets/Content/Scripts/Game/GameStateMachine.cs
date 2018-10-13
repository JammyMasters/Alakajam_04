using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    [Serializable]
    public struct StateObserverGroup
    {
        public GameState State;

        public GameObject[] Observers;
    }

    public StateObserverGroup[] StateObserverGroups;

    private GameState m_currentState = GameState.IDLE;

    public GameState CurrentState { get; }

    private void Start()
    {
        Transition(GameState.SUICIDE_NOTE);
    }

    private void ActivateGameObjects(IEnumerable<GameObject> observers)
    {
        foreach (var observer in observers)
        {
            observer.gameObject.SetActive(true);
        }
    }

    private void DeactivateGameObjects(IEnumerable<GameObject> observers)
    {
        foreach (var observer in observers)
        {
            observer.gameObject.SetActive(false);
        }
    }

    public void Transition(GameState state)
    {
        if (state == m_currentState)
        {
            return;
        }

        // TODO: validate state transitions

        ActivateGameObjectsByState(state);

        foreach (var gameStateObservers in GetGameStateObservers())
        {
            gameStateObservers.OnLeaveState(state);
        }

        m_currentState = state;

        foreach (var gameStateObservers in GetGameStateObservers())
        {
            gameStateObservers.OnEnterState(state);
        }

        DeactivateGameObjectsByState(state);
    }

    private IEnumerable<IGameStateObserver> GetGameStateObservers()
    {
        return StateObserverGroups.SelectMany(x => x.Observers).Select(x => x.GetComponent<IGameStateObserver>()).Where(x => x != null);
    }

    private void ActivateGameObjectsByState(GameState state)
    {
        IEnumerable<GameObject> gameObjects = new List<GameObject>();
        foreach (var stateObserverGroup in StateObserverGroups)
        {
            if (stateObserverGroup.State == state)
            {
                gameObjects = gameObjects.Concat(stateObserverGroup.Observers);
            }
        }
        ActivateGameObjects(gameObjects);
    }

    private void DeactivateGameObjectsByState(GameState state)
    {
        IEnumerable<GameObject> gameObjects = new List<GameObject>();
        foreach (var stateObserverGroup in StateObserverGroups)
        {
            if (stateObserverGroup.State != state)
            {
                gameObjects = gameObjects.Concat(stateObserverGroup.Observers);
            }
        }
        DeactivateGameObjects(gameObjects);
    }

    private void Update()
    {
        switch (m_currentState)
        {
            case GameState.SUICIDE_NOTE:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Transition(GameState.FALLING);
                }
                break;
            case GameState.NEWSPAPER_FLASH:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    Transition(GameState.SUICIDE_NOTE);
                }
                break;
        }
    }
}
