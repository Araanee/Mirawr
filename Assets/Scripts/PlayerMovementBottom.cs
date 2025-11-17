using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform topPlayer; // le chat du haut
    public Transform topMap;    // la map du haut
    public Transform bottomMap; // la map du bas

    public int varTest = 0;

    void Update()
    {
        float move = Keyboard.current.aKey.isPressed ? -speed * Time.deltaTime :
              Keyboard.current.dKey.isPressed ? speed * Time.deltaTime : 0f;

        // Déplacement du joueur bas (ShadowPlayer)
        transform.Translate(move, 0, 0);

        // Le joueur du haut bouge en sens inverse (synchronisé)
        if (topPlayer != null)
            topPlayer.Translate(-move, 0, 0);

        // Les maps sont maintenant gérées par les MapGenerator
        // Plus besoin de les déplacer manuellement
    }
}
