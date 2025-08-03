using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quitter le jeu"); // Juste pour voir dans la console que ça marche
        Application.Quit();

        // Si tu testes dans l'éditeur Unity, Application.Quit() ne ferme pas l'éditeur.
        // On force donc la sortie du mode Play dans l'éditeur Unity (optionnel):
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
