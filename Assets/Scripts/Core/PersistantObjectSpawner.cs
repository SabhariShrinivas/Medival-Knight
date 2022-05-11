using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab;
        static bool hasSpawned; //static variables do no die when we switch scenes
        private void Awake()
        {
            if (hasSpawned) return;
            hasSpawned = true;
            SpawnPersistantObject();
        }

        private void SpawnPersistantObject()
        {
            GameObject persistantObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistantObject);
        }
    }
}
