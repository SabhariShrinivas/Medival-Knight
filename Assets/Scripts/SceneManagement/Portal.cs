using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace RPG.SceneManagement
{
    public enum DestinationIdentifier
    {
        A, B, C, D, E
    }
    public class Portal : MonoBehaviour
    {
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;
        [SerializeField] int sceneToLoad = -1;
        Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        private void Awake()
        {
            spawnPoint = transform.GetChild(0);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                StartCoroutine(Transition());
        }
        IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError("Scene to load index not set");
                yield break;
            }
           
            DontDestroyOnLoad(gameObject);
            Fader fader = FindObjectOfType<Fader>(); 
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();
            yield return fader.FadeOut(fadeOutTime);
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            wrapper.Load();
            Portal otherPortal = GetPortal();
            UpdatePlayer(otherPortal);
            wrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            player.transform.rotation = otherPortal.spawnPoint.rotation;

        }

        private Portal GetPortal()
        {
           foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this || portal.destination != destination) continue;
                return portal;
            }
            return null;

        }
    }
    
    
}
