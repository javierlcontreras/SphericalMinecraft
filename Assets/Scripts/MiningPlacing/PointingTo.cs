using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(ControllerSettings))]
public class PointingTo : MonoBehaviour
{
    ControllerSettings settings;
    public Vector3 GlobalPointingPoint(){
        RaycastHit hit;
        if (Physics.Raycast(settings.CameraTransform.position, settings.CameraTransform.forward, out hit, settings.reach, settings.groundedMask)) {
            return hit.point - 0.1f*hit.normal.normalized;
        }
        return Vector3.zero;
    }

    public Block PointingToBlock() {
        Vector3 pointing = GlobalPointingPoint();
        if (pointing == Vector3.zero) return null;
        Block blockPointed = TerrainManager.instance.BlockClosestToGlobalPoint(pointing);
        return blockPointed;
    }

    void Start() {
        settings = GetComponent<ControllerSettings>();
    }

    void Update() {
        bool wantToBreak = Input.GetButton("Fire1");
        if (wantToBreak) {
            Debug.Log("Break!");
            Block blockPointed = PointingToBlock();
            if (blockPointed != null && blockPointed.inChunkPosition.y >= 1) { 
                Chunk chunkPointed = blockPointed.chunk;
                int side = chunkPointed.sideCoord;
                int chunkX = chunkPointed.xCoord;
                int chunkZ = chunkPointed.zCoord;
                int blockX = (int)blockPointed.inChunkPosition.x;
                int blockY = (int)blockPointed.inChunkPosition.y;
                int blockZ = (int)blockPointed.inChunkPosition.z;
                TerrainManager.instance.planet.chunks[side, chunkX, chunkZ].blocks[blockX, blockY, blockZ].type = BlockTypeEnum.GetBlockTypeByName("air");
                
                TerrainManager.instance.DestroyChunkMesh(chunkPointed.sideCoord, chunkPointed.xCoord, chunkPointed.zCoord);
                TerrainManager.instance.GenerateChunkMesh(chunkPointed.sideCoord, chunkPointed.xCoord, chunkPointed.zCoord);
            }
        }
    }

}
