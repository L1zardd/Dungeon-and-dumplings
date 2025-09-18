using UnityEngine;
using System.Collections.Generic;

public class CookingPot : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public List<string> requiredVegetables; // Список нужных овощей
        public GameObject resultDish; // Готовое блюдо (префаб)
        public float cookingTime = 5f;
    }

    [Header("Cooking Settings")]
    public List<Recipe> recipes = new List<Recipe>();
    public Transform contentPoint; // Точка где появляется содержимое
    public float stirDetectionThreshold = 0.5f; // Порог определения перемешивания

    [Header("Visual Effects")]
    public ParticleSystem cookingParticles;
    public AudioClip addVegetableSound;
    public AudioClip cookingSound;
    public AudioClip completeSound;

    private List<string> _currentVegetables = new List<string>();
    private Recipe _currentRecipe;
    private float _cookingTimer = 0f;
    private bool _isCooking = false;
    private Vector3 _lastStirPosition;
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _lastStirPosition = transform.position;
    }

    void Update()
    {
        if (_isCooking)
        {
            _cookingTimer -= Time.deltaTime;

            // Проверяем перемешивание
            CheckStirring();

            if (_cookingTimer <= 0f)
            {
                CompleteCooking();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем что это овощ
        Vegetable vegetable = other.GetComponent<Vegetable>();
        if (vegetable != null && vegetable.IsSliced)
        {
            AddVegetableToPot(vegetable.gameObject);
        }
    }

    void AddVegetableToPot(GameObject vegetable)
    {
        string vegetableName = GetVegetableBaseName(vegetable.name);
        _currentVegetables.Add(vegetableName);

        // Проигрываем звук
        if (addVegetableSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(addVegetableSound);
        }

        // Уничтожаем овощ или делаем его невидимым
        Destroy(vegetable);

        // Проверяем рецепты
        CheckRecipes();

        Debug.Log("Добавлен овощ: " + vegetableName + ". Всего в кастрюле: " + _currentVegetables.Count);
    }

    void CheckRecipes()
    {
        foreach (Recipe recipe in recipes)
        {
            if (IsRecipeMatch(recipe))
            {
                StartCooking(recipe);
                return;
            }
        }
    }

    bool IsRecipeMatch(Recipe recipe)
    {
        if (_currentVegetables.Count != recipe.requiredVegetables.Count)
            return false;

        // Создаем копии для проверки
        List<string> currentVeggies = new List<string>(_currentVegetables);
        List<string> requiredVeggies = new List<string>(recipe.requiredVegetables);

        // Сортируем для сравнения
        currentVeggies.Sort();
        requiredVeggies.Sort();

        for (int i = 0; i < currentVeggies.Count; i++)
        {
            if (currentVeggies[i] != requiredVeggies[i])
                return false;
        }

        return true;
    }

    void StartCooking(Recipe recipe)
    {
        _currentRecipe = recipe;
        _cookingTimer = recipe.cookingTime;
        _isCooking = true;

        // Запускаем визуальные эффекты
        if (cookingParticles != null)
        {
            cookingParticles.Play();
        }

        if (cookingSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(cookingSound);
        }

        Debug.Log("Начинаем готовить: " + recipe.recipeName);
    }

    void CheckStirring()
    {
        float moveDistance = Vector3.Distance(transform.position, _lastStirPosition);

        if (moveDistance > stirDetectionThreshold)
        {
            // Ускоряем готовку при перемешивании
            _cookingTimer -= Time.deltaTime * 0.5f;
            _lastStirPosition = transform.position;
        }
    }

    void CompleteCooking()
    {
        _isCooking = false;

        if (cookingParticles != null)
        {
            cookingParticles.Stop();
        }

        if (completeSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(completeSound);
        }

        // Создаем готовое блюдо в кастрюле
        if (_currentRecipe.resultDish != null && contentPoint != null)
        {
            GameObject dish = Instantiate(_currentRecipe.resultDish, contentPoint.position, Quaternion.identity);

            // Делаем блюдо кликабельным для перемещения в serving area
            Dish dishComponent = dish.GetComponent<Dish>();
            if (dishComponent != null)
            {
                dishComponent.isReadyToServe = true;
                dishComponent.sourcePot = this; // Запоминаем кастрюлю
            }

            // Добавляем коллайдер для клика если его нет
            if (dish.GetComponent<Collider2D>() == null)
            {
                dish.AddComponent<BoxCollider2D>();
            }
        }

        Debug.Log("Блюдо готово! Кликните на него чтобы подать");

        // Очищаем кастрюлю
        ClearPot();
    }

    void ClearPot()
    {
        _currentVegetables.Clear();
        _currentRecipe = null;
        _cookingTimer = 0f;
    }

    string GetVegetableBaseName(string fullName)
    {
        // Убираем "(Clone)" и другие постфиксы
        return fullName.Replace("(Clone)", "").Trim();
    }

    // Методы для UI
    public List<string> GetCurrentVegetables()
    {
        return new List<string>(_currentVegetables);
    }

    public Recipe GetCurrentRecipe()
    {
        return _currentRecipe;
    }

    public bool IsCooking()
    {
        return _isCooking;
    }

    public float GetCookingProgress()
    {
        if (_currentRecipe == null) return 0f;
        return 1f - (_cookingTimer / _currentRecipe.cookingTime);
    }

    // Принудительная очистка кастрюли
    public void ForceClearPot()
    {
        ClearPot();
        if (cookingParticles != null)
        {
            cookingParticles.Stop();
        }
    }
}