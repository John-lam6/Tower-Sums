using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BlockState {
    InHotbar,
    OnAddition,
    OnSubtraction
}

public class BlockData : MonoBehaviour {
    public int value = 1;
    public BlockState state;
    public float unitHeight = 1f;
    public float hotbarHeight = 0.6f;
    private Vector3 originalScale;
    public TextMeshPro textbox;

    [HideInInspector] public Vector3 hotbarPosition;
    [HideInInspector] public float targetHeight;
    [HideInInspector] public bool hasTower;
    public bool isHotbarBlock = false;
    
    public Material default_outline;
    public Material addition_outline;
    public Material subtraction_outline;
    
    private MeshRenderer mesh_renderer;

    void Awake() {
        mesh_renderer = GetComponent<MeshRenderer>();
        originalScale = transform.localScale;
    }
    
    void Start() {
        hasTower = false;

        if (isHotbarBlock) {
            ApplyHotbarScale();
        }

        UpdateMaterial();
    }

    void OnValidate() {
        UpdateHeight();
        if (mesh_renderer != null) UpdateMaterial();
        textbox.text = value.ToString();
    }

    void UpdateMaterial() {
        Material[] mats = mesh_renderer.materials;
        switch (state) {
            case BlockState.InHotbar:
                mats[1] = default_outline;
                break;
            case BlockState.OnAddition:
                mats[1] = addition_outline;
                break;
            case BlockState.OnSubtraction:
                mats[1] = subtraction_outline;
                break;
        }
        mesh_renderer.materials = mats;
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
        targetHeight = pos.y;

        if (textbox != null) {
            textbox.transform.localScale = new Vector3(1f, 1f / scale.y, 1f);
        }
    }

    public void ApplyHotbarScale() {
        Vector3 scale = originalScale;
        scale.y = hotbarHeight;
        transform.localScale = scale;

        Vector3 pos = hotbarPosition;
        pos.y += scale.y / 2f;
        transform.position = pos;

        if (textbox != null) {
            textbox.transform.localScale = Vector3.one;
        }
    }

    public void ApplyTowerScale() {
        UpdateHeight();
    }

    public void SetState(BlockState newState) {
        state = newState;
        UpdateMaterial();
    }

    public void SetNeutralDragState() {
        state = BlockState.InHotbar;
        UpdateMaterial();
    }

    public void SetValue(int newValue)
    {
        value = Mathf.Max(1, newValue);
        textbox.text = value.ToString();

        if (hasTower) {
            ApplyTowerScale();
        }
        else if (isHotbarBlock) {
            ApplyHotbarScale();
        }
        else {
            UpdateHeight();
        }
    }
}