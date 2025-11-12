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
        Debug.Log("Update() s'exécute bien ✅");
        float move = Keyboard.current.aKey.isPressed ? -speed * Time.deltaTime :
              Keyboard.current.dKey.isPressed ? speed * Time.deltaTime : 0f;

        // Déplacement du joueur bas
        transform.Translate(move, 0, 0);

        // La map du bas bouge dans le même sens (effet de défilement)
        if (bottomMap != null)
            bottomMap.Translate(-move, 0, 0);

        // Le joueur du haut bouge en sens inverse
        if (topPlayer != null)
            topPlayer.Translate(-move, 0, 0);

        // Et la map du haut bouge dans le même sens que le joueur du haut
        if (topMap != null)
            topMap.Translate(move, 0, 0);

        Debug.Log(varTest);
    }
}
