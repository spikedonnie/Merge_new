using UnityEngine;
using SolClovser.EasyMultipleHealthbar;

public class DemoEnemy : MonoBehaviour
{
    public int maxHp = 100;
    public int currentHp = 75;
    public Vector3 positionOffset = new Vector3(0, 2, 0);
    public int sortingLayer;

    private HealthbarController _healthbar;

    public void Start()
    {
        RequestAHealthbar();
    }

    public void Update()
    {
        // For demonstration purposes, I updated it in Update method. In normal use it is enough to update it when hp changes.
        UpdateHealthbar(currentHp);
    }

    // How to request a healthbar
    private void RequestAHealthbar()
    {
        // Request a healthbar object from manager
        _healthbar = EasyMultipleHealthbar.Instance.RequestHealthbar();
        
        // Then setup hp values, which transform health bar should follow, and with how much offset
        _healthbar.SetupUI(maxHp, currentHp, transform, positionOffset);
        
        // Set the sorting layer to something higher if you want this healthbar to stay on top of other healthbars
        // You might need to increase the layer count in Easy Multiple Healthbar object.
        _healthbar.SetSortingLayer(sortingLayer);
        
        // You can use this if two healthbars are in same layer and they are overlapping.
        // _healthbar.MoveToBottomInLayerHierarchy();
    }

    // How to update the healthbar
    private void UpdateHealthbar(float currentValue)
    {
        _healthbar.UpdateUI(currentValue);
    }
    
    // How to return healthbar to pool when you are finished. for example; when enemy dies
    private void ReturnHealthbar()
    {
        // Simply call Return function
        _healthbar.Return();
    }
    
    // !!
    // How to return all healthbars when you are finished with them.
    // for example you should return all healthbars when you are switching to a new scene
    private void ReturnAllHealthbars()
    {
        // Simply call ReturnAllHealthbars function
        EasyMultipleHealthbar.Instance.ReturnAllHealthbars();
    }
}
