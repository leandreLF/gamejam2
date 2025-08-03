using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu"); // Juste pour voir dans la console que �a marche
        Application.Quit();

        // Si tu testes dans l'�diteur Unity, Application.Quit() ne ferme pas l'�diteur.
        // On force donc la sortie du mode Play dans l'�diteur Unity (optionnel):
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
