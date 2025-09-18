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
        // Обновляем прогресс готовки
        if (progressBar != null)
        {
            progressBar.fillAmount = _cookingPot.GetCookingProgress();
        }

        // Индикатор готовки
        if (cookingIndicator != null)
        {
            cookingIndicator.SetActive(_cookingPot.IsCooking());
        }

        // Информация о текущем рецепте
        CookingPot.Recipe currentRecipe = _cookingPot.GetCurrentRecipe();
        if (currentRecipe != null && recipeNameText != null)
        {
            recipeNameText.text = currentRecipe.recipeName;
        }
    }

    // Метод для отображения доступных рецептов
    public void ShowAvailableRecipes(List<CookingPot.Recipe> availableRecipes)
    {
        // Реализуйте отображение рецептов в UI
    }

    // Метод для обновления счетчиков ингредиентов
    public void UpdateIngredientCounters(List<string> currentVegetables)
    {
        // Реализуйте обновление UI с ингредиентами
    }
}