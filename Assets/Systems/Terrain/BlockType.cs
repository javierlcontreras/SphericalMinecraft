using System.Collections.Generic;
using UnityEngine;

public interface BlockType {
    int GetStackSize(); // 0 means not inventory, 1 means singleton
    string GetName();
    Vector2Int GetAtlasCoord(string side);
}

class Air : BlockType {
    public string GetName() { return "air"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(-1, -1);
    }
    public int GetStackSize() {
        return 0;
    }
}

class Grass : BlockType {
    public string GetName() { return "grass"; }
    public Vector2Int GetAtlasCoord(string side) {
        int x, y; 
        if (side == "up") {
            x = 10;
            y = 1;
        }
        else if (side == "down") {
            x = 2;
            y = 0;
        }
        else {
            x = 3;
            y = 0;
        }

        return new Vector2Int(x,y);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Dirt : BlockType {
    public string GetName() { return "dirt"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(2,0);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Stone : BlockType {
    public string GetName() { return "stone"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(1,0);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Cobblestone : BlockType {
    public string GetName() { return "cobblestone"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(0,1);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Gravel : BlockType {
    public string GetName() { return "gravel"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(3,1);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Sand : BlockType {
    public string GetName() { return "sand"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(2,1);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Bedrock : BlockType {
    public string GetName() { return "bedrock"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(1,1);
    }
    public int GetStackSize() {
        return 0;
    }
}


class Wood : BlockType {
    public string GetName() { return "wood"; }
    public Vector2Int GetAtlasCoord(string side) {
        int x, y; 
        if (side == "up" || side == "down") {
            x = 5;
            y = 1;
        }
        else {
            x = 4;
            y = 1;
        }

        return new Vector2Int(x,y);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Leaves : BlockType {
    public string GetName() { return "leaves"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(11,1);
    }
    public int GetStackSize() {
        return 64;
    }
}

class Invalid : BlockType {
    public string GetName() { return "invalid"; }
    public Vector2Int GetAtlasCoord(string side) {
        return new Vector2Int(-4,-2);
    }
    public int GetStackSize() {
        return 0;
    }
}


public static class BlockTypeEnum {
    static Dictionary<string, BlockType> dict = new Dictionary<string, BlockType> () {
        ["air"] = new Air(),
        ["grass"] = new Grass(),
        ["dirt"] = new Dirt(),
        ["stone"] = new Stone(),
        ["cobblestone"] = new Cobblestone(),
        ["gravel"] = new Gravel(),
        ["sand"] = new Sand(),
        ["bedrock"] = new Bedrock(),
        ["wood"] = new Wood(),
        ["leaves"] = new Leaves(),
        ["invalid"] = new Invalid()
    };

    public static BlockType GetBlockTypeByName(string name) {
        return dict[name];
    }
}