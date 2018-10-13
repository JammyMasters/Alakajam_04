using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class CrowdSpawner : MonoBehaviour, IGameStateObserver
{
    [Serializable]
    public class SuicideNoteRecipientPrefabOptions
    {
        [EnumFlags]
        public SuicideNote.Recipient Recipients;

        public GameObject[] Prefabs;

        public bool IsEmpty => Prefabs.Length == 0;

        // Source: https://forum.unity.com/threads/multiple-enum-select-from-inspector.184729/
        public List<SuicideNote.Recipient> ApplicableRecipients
        {
            get
            {
                var applicableRecipients = new List<SuicideNote.Recipient>();
                var recipients = Enum.GetValues(typeof(SuicideNote.Recipient));
                for (int i = 0; i < recipients.Length; i++)
                {
                    int layer = 1 << i;
                    if (((int)Recipients & layer) != 0)
                    {
                        applicableRecipients.Add((SuicideNote.Recipient)recipients.GetValue(i));
                    }
                }
                return applicableRecipients;
            }
        }

        public GameObject RandomPrefab
        {
            get
            {
                return Prefabs[UnityEngine.Random.Range(0, Prefabs.Length)];
            }
        }
    }

    private const string c_crowdGameObjectName = "Crowd";

    public SuicideNote SuicideNote;

    public int MinNumberOfPeople;

    public int MaxNumberOfPeople;

    public GameObject[] Prefabs;

    public SpawnableSurface SpawnableSurface;

    public SuicideNoteRecipientPrefabOptions[] PrefabOptionsPerRecipientType;

    public bool SpawnInEditMode;

    public void Update()
    {
        if (Application.isEditor && SpawnInEditMode)
        {
            Spawn();
            SpawnInEditMode = false;
        }
    }

    public void OnEnterState(GameState state)
    {
        if (state != GameState.Falling)
        {
            return;
        }

        Spawn();
    }

    private void Spawn()
    {
        DestroyCrowdGameObject();

        var crowdGameObject = new GameObject(c_crowdGameObjectName);
        crowdGameObject.transform.parent = transform;

        var maxNumberOfPeople = MaxNumberOfPeople;
        if (SuicideNote.NoteIntention == SuicideNote.Intention.RevengeAgainstRecipient)
        {
            SpawnSuicideNoteRecipient(crowdGameObject);
            maxNumberOfPeople--;
        }

        var numberOfPeople = UnityEngine.Random.Range(MinNumberOfPeople, maxNumberOfPeople + 1);
        for (int i = 0; i < numberOfPeople; i++)
        {
            SpawnRandomPerson(crowdGameObject);
        }
    }

    private void SpawnRandomPerson(GameObject crowdGameObject)
    {
        if (Prefabs.Length == 0)
        {
            return;
        }
        InstantiatePersonPrefab(Prefabs[UnityEngine.Random.Range(0, Prefabs.Length)], crowdGameObject.transform);
    }

    private void SpawnSuicideNoteRecipient(GameObject crowdGameObject)
    {
        var prefabOptions = PrefabOptionsPerRecipientType.Where(x => x.ApplicableRecipients.Contains(SuicideNote.NoteRecipient)).FirstOrDefault();
        if (prefabOptions == null)
        {
            return;
        }
        if (prefabOptions.IsEmpty)
        {
            return;
        }
        InstantiatePersonPrefab(prefabOptions.RandomPrefab, crowdGameObject.transform);
    }

    private void InstantiatePersonPrefab(GameObject prefab, Transform parent)
    {
        Vector3 position;
        if (!SpawnableSurface.GetUnoccupiedPosition(out position))
        {
            // TODO:
            Debug.Log("Couldn't find a vacant position on spawnable surface");
            return;
        }
        Instantiate(prefab, position, Quaternion.identity, parent);
    }

    private void DestroyCrowdGameObject()
    {
        var piecesGameObject = transform.Find(c_crowdGameObjectName)?.gameObject;
        if (piecesGameObject != null)
        {
            if (Application.isPlaying)
            {
                Destroy(piecesGameObject);
            }
            else
            {
                DestroyImmediate(piecesGameObject);
            }
        }
    }

    public void OnLeaveState(GameState state)
    {
        if (state != GameState.Falling)
        {
            return;
        }

        DestroyCrowdGameObject();
    }
}
