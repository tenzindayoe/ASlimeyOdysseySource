using TMPro;
using UnityEngine;

public class lakeDetailedViewPanelManager : MonoBehaviour
{
    [Header("Lake Detailed View UI Objects")]
    [SerializeField]
    private TextMeshProUGUI lakeNameText;
    
    public void SetLakeName(string lakeName)
    {
        lakeNameText.text = lakeName;
    }
}
