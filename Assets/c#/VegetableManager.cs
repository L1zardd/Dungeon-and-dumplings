using UnityEngine;
using System.Collections.Generic;

public class VegetableManager : MonoBehaviour
{
    [Header("Vegetable Prefabs")]
    public GameObject eggplantPrefab;
    public GameObject potatoPrefab;
    public GameObject kiwiPrefab;
    public GameObject cornPrefab;
    public GameObject onionPrefab;
    public GameObject pepperPrefab;
    public GameObject tomatoPrefab;

    [Header("Spawn Settings")]
    public int maxVegetables = 20;

    private List<GameObject> _activeVegetables = new List<GameObject>();

    void Start()
    {
        // Находим все овощи на сцене при старте
        FindAllVegetables();
    }

    void FindAllVegetables()
    {
        _activeVegetables.Clear();
        Vegetable[] vegetables = FindObjectsOfType<Vegetable>();
        foreach (Vegetable veg in vegetables)
        {
            _activeVegetables.Add(veg.gameObject);
        }
        Debug.Log("Found " + _activeVegetables.Count + " vegetables on scene");
    }

    // Добавить овощ в список
    public void AddVegetable(GameObject vegetable)
    {
        if (vegetable != null && !_activeVegetables.Contains(vegetable))
        {
            _activeVegetables.Add(vegetable);
            Debug.Log("Added vegetable: " + vegetable.name);
        }
    }

    // Метод для создания овоща по типу
    public GameObject CreateVegetable(string vegetableType, Vector3 position)
    {
        GameObject prefab = GetPrefabByType(vegetableType);
        if (prefab != null && _activeVegetables.Count < maxVegetables)
        {
            GameObject newVegetable = Instantiate(prefab, position, Quaternion.identity);
            newVegetable.name = prefab.name + "(Clone)";

            // Настраиваем чтобы копия не создавала свои копии
            DraggableVegetable draggable = newVegetable.GetComponent<DraggableVegetable>();
            if (draggable != null)
            {
                draggable.createCopyOnDrag = false;
                draggable.canBeDragged = true;
            }

            _activeVegetables.Add(newVegetable);
            return newVegetable;
        }
        return null;
    }

    GameObject GetPrefabByType(string type)
    {
        switch (type.ToLower())
        {
            case "eggplant": return eggplantPrefab;
            case "potato": return potatoPrefab;
            case "kiwi": return kiwiPrefab;
            case "corn": return cornPrefab;
            case "onion": return onionPrefab;
            case "pepper": return pepperPrefab;
            case "tomato": return tomatoPrefab;
            default: return null;
        }
    }

    // Очистка разрезанных овощей
    public void ClearSlicedVegetables()
    {
        for (int i = _activeVegetables.Count - 1; i >= 0; i--)
        {
            if (_activeVegetables[i] == null)
            {
                _activeVegetables.RemoveAt(i);
                continue;
            }

            Vegetable veg = _activeVegetables[i].GetComponent<Vegetable>();
            if (veg != null && veg.IsSliced)
            {
                Destroy(_activeVegetables[i]);
                _activeVegetables.RemoveAt(i);
                Debug.Log("Removed sliced vegetable");
            }
        }
    }

    // Обновление списка овощей
    public void UpdateVegetableList()
    {
        FindAllVegetables();
    }

    // Получить количество овощей
    public int GetVegetableCount()
    {
        return _activeVegetables.Count;
    }

    // Очистить все овощи (для перезапуска игры)
    public void ClearAllVegetables()
    {
        foreach (GameObject veg in _activeVegetables)
        {
            if (veg != null)
            {
                Destroy(veg);
            }
        }
        _activeVegetables.Clear();
        FindAllVegetables();
    }
}