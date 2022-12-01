using System.Collections.Generic;
using UnityEngine;

public interface BlockType {
    string GetName();
    Vector2 GetAtlasCoord(string side);
}

class Air : BlockType {
    string name = "air";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(-1, -1);
    }
}

class Grass : BlockType {
    string name = "grass";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
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

        return new Vector2(x,y);
    }
}

class Dirt : BlockType {
    string name = "dirt";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(2,0);
    }
}

class Stone : BlockType {
    string name = "stone";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(1,0);
    }
}

class Cobblestone : BlockType {
    string name = "cobblestone";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(0,1);
    }
}

class Gravel : BlockType {
    string name = "gravel";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(3,1);
    }
}

class Sand : BlockType {
    string name = "sand";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(2,1);
    }
}

class Bedrock : BlockType {
    string name = "bedrock";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(1,1);
    }
}


class Wood : BlockType {
    string name = "wood";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        int x, y; 
        if (side == "up" || side == "down") {
            x = 5;
            y = 1;
        }
        else {
            x = 4;
            y = 1;
        }

        return new Vector2(x,y);
    }
}

class Leaves : BlockType {
    string name = "wood";

    public string GetName() { return name; }
    public Vector2 GetAtlasCoord(string side) {
        return new Vector2(11,1);
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
        ["leaves"] = new Leaves()
    };

    public static BlockType GetBlockTypeByName(string name) {
        return dict[name];
    }
}