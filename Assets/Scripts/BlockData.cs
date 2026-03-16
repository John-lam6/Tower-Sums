using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BlockState {
    InHotbar,
    OnAddition,
    OnSubtraction
}

public class BlockData : MonoBehaviour {
    public int value = 1;
    public BlockState state;
    public float unitHeight = 1f;
    
    // Start is called before the first frame update
    void Start() {
        UpdateHeight();
    }

    void OnValidate() {
        UpdateHeight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHeight() {
        if (value < 1) value = 1;

        Vector3 scale = transform.localScale;
        scale.y = value * unitHeight;
        transform.localScale = scale;
        
        // keep base grounded so it grows upward and not outward
        Vector3 pos = transform.position;
        pos.y = scale.y / 2f;
        transform.position = pos;
    }
}
