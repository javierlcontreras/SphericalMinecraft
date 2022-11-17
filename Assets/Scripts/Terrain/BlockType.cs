using System.Collections.Generic;
using UnityEngine;

public interface BlockType {
    string GetName();
    Vector2 GetAtlasCoord(string side);
}

class Dirt : BlockType {
    string name = "dirt";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        int x, y; 
        if (side == "up") {
            x = 0;
            y = 0;
        }
        else if (side == "down") {
            x = 1;
            y = 1;
        }
        else {
            x = 0;
            y = 1;
        }

        return new Vector2(x,y);
    }
}

class Air : BlockType {
    string name = "air";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(-1, -1);
    }
}

class Wood : BlockType {
    string name = "wood";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        int x, y; 
        if (side == "up" || side == "down") {
            x = 1;
            y = 2;
        }
        else {
            x = 0;
            y = 2;
        }

        return new Vector2(x,y);
    }
}

class Leaves : BlockType {
    string name = "wood";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(2,2);
    }
}

public static class BlockTypeEnum {
    static Dictionary<string, BlockType> dict = new Dictionary<string, BlockType> () {
        ["dirt"] = new Dirt(),
        ["air"] = new Air(),
        ["wood"] = new Wood(),
        ["leaves"] = new Leaves()
    };

    public static BlockType GetBlockTypeByName(string name) {
        return dict[name];
    }
}