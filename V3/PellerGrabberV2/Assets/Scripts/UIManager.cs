using UnityEngine;
using TMPro; // Assurez-vous d'avoir TextMeshPro dans votre projet

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton pour un accès facile depuis d'autres scripts

    public GameObject floatingTextPrefab; // Assignez votre préfabriqué de texte flottant dans l'éditeur

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void ShowFloatingText(string text, Vector3 worldPosition)
    {
        Debug.Log(worldPosition + "XXXXXXXX" + text);
        worldPosition[1] += 0.7f;
        // Convertir la position du monde en position d'écran si nécessaire
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Créer une instance du texte flottant
        var floatingTextInstance = Instantiate(floatingTextPrefab, transform);
        
        // Positionner le texte à l'endroit désiré
        floatingTextInstance.transform.position = worldPosition;
        
        // Définir le texte
        floatingTextInstance.GetComponent<TextMeshPro>().text = text;
        
        // Activer l'instance et détruire après un certain temps
        floatingTextInstance.SetActive(true);
        Destroy(floatingTextInstance, 1.0f); // Adaptez la durée selon les besoins
    }
    //     public void ShowFloatingText(string text, Vector3 worldPosition)
    // {
    //     // Créer une instance du texte flottant directement à la position dans le monde
    //     var floatingTextInstance = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity, canvas.transform);

    //     // Optionnel: Ajuster l'orientation pour faire face à la caméra
    //     floatingTextInstance.transform.LookAt(Camera.main.transform);
    //     floatingTextInstance.transform.rotation = Quaternion.LookRotation(floatingTextInstance.transform.position - Camera.main.transform.position);

    //     // Définir le texte
    //     floatingTextInstance.GetComponent<TextMeshPro>().text = text;
        
    //     // Activer l'instance et détruire après un certain temps
    //     floatingTextInstance.SetActive(true);
    //     Destroy(floatingTextInstance, 2.0f); // Adaptez la durée selon les besoins
    // }
}