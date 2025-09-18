using UnityEngine;
using System.Collections.Generic;

public class CookingPot : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public List<string> requiredVegetables; // ������ ������ ������
        public GameObject resultDish; // ������� ����� (������)
        public float cookingTime = 5f;
    }

    [Header("Cooking Settings")]
    public List<Recipe> recipes = new List<Recipe>();
    public Transform contentPoint; // ����� ��� ���������� ����������
    public float stirDetectionThreshold = 0.5f; // ����� ����������� �������������

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

            // ��������� �������������
            CheckStirring();

            if (_cookingTimer <= 0f)
            {
                CompleteCooking();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ��������� ��� ��� ����
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

        // ����������� ����
        if (addVegetableSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(addVegetableSound);
        }

        // ���������� ���� ��� ������ ��� ���������
        Destroy(vegetable);

        // ��������� �������
        CheckRecipes();

        Debug.Log("�������� ����: " + vegetableName + ". ����� � ��������: " + _currentVegetables.Count);
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

        // ������� ����� ��� ��������
        List<string> currentVeggies = new List<string>(_currentVegetables);
        List<string> requiredVeggies = new List<string>(recipe.requiredVegetables);

        // ��������� ��� ���������
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

        // ��������� ���������� �������
        if (cookingParticles != null)
        {
            cookingParticles.Play();
        }

        if (cookingSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(cookingSound);
        }

        Debug.Log("�������� ��������: " + recipe.recipeName);
    }

    void CheckStirring()
    {
        float moveDistance = Vector3.Distance(transform.position, _lastStirPosition);

        if (moveDistance > stirDetectionThreshold)
        {
            // �������� ������� ��� �������������
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

        // ������� ������� ����� � ��������
        if (_currentRecipe.resultDish != null && contentPoint != null)
        {
            GameObject dish = Instantiate(_currentRecipe.resultDish, contentPoint.position, Quaternion.identity);

            // ������ ����� ������������ ��� ����������� � serving area
            Dish dishComponent = dish.GetComponent<Dish>();
            if (dishComponent != null)
            {
                dishComponent.isReadyToServe = true;
                dishComponent.sourcePot = this; // ���������� ��������
            }

            // ��������� ��������� ��� ����� ���� ��� ���
            if (dish.GetComponent<Collider2D>() == null)
            {
                dish.AddComponent<BoxCollider2D>();
            }
        }

        Debug.Log("����� ������! �������� �� ���� ����� ������");

        // ������� ��������
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
        // ������� "(Clone)" � ������ ���������
        return fullName.Replace("(Clone)", "").Trim();
    }

    // ������ ��� UI
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

    // �������������� ������� ��������
    public void ForceClearPot()
    {
        ClearPot();
        if (cookingParticles != null)
        {
            cookingParticles.Stop();
        }
    }
}