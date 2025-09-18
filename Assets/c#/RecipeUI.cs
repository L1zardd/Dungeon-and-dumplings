using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeUI : MonoBehaviour
{
    [Header("UI References")]
    public Text recipeNameText;
    public Transform ingredientsPanel;
    public GameObject ingredientIconPrefab;
    public Image progressBar;
    public GameObject cookingIndicator;

    private CookingPot _cookingPot;
    private Dictionary<string, Text> _ingredientCounters = new Dictionary<string, Text>();

    void Start()
    {
        _cookingPot = FindObjectOfType<CookingPot>();
    }

    void Update()
    {
        if (_cookingPot != null)
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        // ��������� �������� �������
        if (progressBar != null)
        {
            progressBar.fillAmount = _cookingPot.GetCookingProgress();
        }

        // ��������� �������
        if (cookingIndicator != null)
        {
            cookingIndicator.SetActive(_cookingPot.IsCooking());
        }

        // ���������� � ������� �������
        CookingPot.Recipe currentRecipe = _cookingPot.GetCurrentRecipe();
        if (currentRecipe != null && recipeNameText != null)
        {
            recipeNameText.text = currentRecipe.recipeName;
        }
    }

    // ����� ��� ����������� ��������� ��������
    public void ShowAvailableRecipes(List<CookingPot.Recipe> availableRecipes)
    {
        // ���������� ����������� �������� � UI
    }

    // ����� ��� ���������� ��������� ������������
    public void UpdateIngredientCounters(List<string> currentVegetables)
    {
        // ���������� ���������� UI � �������������
    }
}