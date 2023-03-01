using System;
using UnityEngine;

public class InItTest : MonoBehaviour
{
    [SerializeField] private GameObject userInputStor;
    
    private void Awake()
    {
        Instantiate(userInputStor);
    }
    
    
}
