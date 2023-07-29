using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class AmmonitionTextSetter : MonoBehaviour
{
    [SerializeField] private GlobalInventory _inventory;
    private TMP_Text ammonitionText;
    
    void Awake()
    {
        _inventory.ResetData();
        ammonitionText = GetComponent<TMP_Text>();
        _inventory.OnAmmonitionChanged += InventoryOnOnAmmonitionChanged;
        InventoryOnOnAmmonitionChanged(_inventory.Ammonition);
    }

    private void InventoryOnOnAmmonitionChanged(int newValue)
    {
        ammonitionText.text = newValue.ToString();
    }
}
