using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockState {
    InHotbar,
    OnAddition,
    OnSubtraction
}

public class BlockData : MonoBehaviour {
    public int value;
    public BlockState state;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
