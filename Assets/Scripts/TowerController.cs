using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public List<BlockData> stackedBlocks = new List<BlockData>();
    public int GetTotalValue() {
        int total = 0;
        foreach (var block in stackedBlocks) total += block.value;
        return total;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBlock(BlockData block) {
        stackedBlocks.Add(block);
    }

    public void RemoveBlock(BlockData block) {
        stackedBlocks.Remove(block);
    }
}
