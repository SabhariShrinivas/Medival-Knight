using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool isTriggered;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !isTriggered)
            {
                GetComponent<PlayableDirector>().Play();
                isTriggered = true;
            }

        }
    }
}
