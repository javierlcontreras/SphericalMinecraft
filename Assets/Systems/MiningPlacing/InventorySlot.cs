public class InventorySlot {
    int quantity;
    BlockType blockType;

    public InventorySlot(int _quantity, BlockType _blockType) {
        quantity = _quantity;
        blockType = _blockType;
    }

    public int GetQuantity() {
        return quantity;
    }
    public BlockType GetBlockType() {
        return blockType;
    }

}