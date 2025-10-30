using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    Angam angam;

    private void Awake()
    {
        angam = GetComponent<Angam>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Attack"))
        {
            angam.TryToAttack();
        }
         
    }
}
