using System.Collections.Generic;
using UnityEngine;

public interface BlockType {
    string GetName();
    //Vector2 GetUV();
}

class Dirt : BlockType {
    string name = "dirt";
    //Vector2 uv = Vector2.zero;

    public string GetName() { return name; }
    //public Vector2 GetUV() { return uv; }
}

class Air : BlockType {
    string name = "air";
    //Vector2 uv = Vector2.zero;

    public string GetName() { return name; }
    //public Vector2 GetUV() { return uv; }
}

public static class BlockTypeEnum {
    static Dictionary<string, BlockType> dict = new Dictionary<string, BlockType> () {
        ["dirt"] = new Dirt(),
        ["air"] = new Air()
    };

    public static BlockType GetBlockTypeByName(string name) {
        return dict[name];
    }
}