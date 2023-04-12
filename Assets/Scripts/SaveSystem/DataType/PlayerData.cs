using UnityEngine;

[System.Serializable]
class PlayerData {
    public Vector3 position;
    public Quaternion rotation;
    public bool isFlying;
    public float verticalVelocity;
    public PlayerData(GameObject player) {
        position = player.transform.position;
        rotation = player.transform.rotation;
        isFlying = player.GetComponent<FirstPersonController>().GetIsFlying();
        verticalVelocity = player.GetComponent<FirstPersonController>().GetVerticalVelocity();
    }
}