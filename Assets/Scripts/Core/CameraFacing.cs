using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Core
{
    
    public class CameraFacing : MonoBehaviour
    {

        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
