using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

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

            //disable player control before scene trasisition
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);
            wrapper.Save();
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            //disable player controller from the new scene as well
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            wrapper.Load();
            Portal otherPortal = GetPortal();
            UpdatePlayer(otherPortal);
            wrapper.Save();
            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);
            newPlayerController.enabled = true;
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
