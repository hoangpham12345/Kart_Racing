using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
    public Material[] materials;
    public GameOptions gameOptions;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material = materials[gameOptions.kart];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
