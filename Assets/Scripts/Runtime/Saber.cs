using System;
using UnityEngine;

public class Saber : MonoBehaviour
{
    [SerializeField] private LayerMask _slicableMassk;

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & _slicableMassk) == 0)
        {
            return;
        }
        
        
        
    }
}
