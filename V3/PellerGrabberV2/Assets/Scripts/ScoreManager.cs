using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assurez-vous d'ajouter cette directive pour utiliser TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public TextMeshPro textMesh; // Assignez votre objet TextMeshPro 3D ici dans l'éditeur Unity
    private List<float> scores = new List<float>(); // Liste pour stocker les scores

    // Méthode pour ajouter un score et mettre à jour la moyenne
    public void AddScore(float score)
    {
        scores.Add(score);
        if (scores.Count > 100)
        {
            scores.RemoveAt(0); // Supprime le score le plus ancien si on dépasse 100
        }
        UpdateScoreText();
    }

    //Calcule la moyenne des scores et met à jour le texte de l'élément TextMeshPro 3D
    void UpdateScoreText()
    {
        if (scores.Count == 0) return; // Vérifie qu'il y a des scores pour éviter la division par zéro

        float sum = 0f;
        foreach (float score in scores)
        {
            sum += score;
        }
        float average = sum / scores.Count;
        
        // Met à jour le texte de TextMeshPro 3D avec la moyenne des scores
        textMesh.text = $"{average.ToString("F1")}"; // "F2" formate le nombre avec 2 décimales
    }
}
