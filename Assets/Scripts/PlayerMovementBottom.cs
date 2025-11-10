using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Transform topPlayer; // le chat du haut
    public Transform topMap;    // la map du haut
    public Transform bottomMap; // la map du bas

    void Update()
    {
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;

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
    }
}
