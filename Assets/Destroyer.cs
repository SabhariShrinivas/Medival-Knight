using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] GameObject targetToDestroy;

    public void DestroyText()
    {
        Destroy(targetToDestroy);
    }
}
