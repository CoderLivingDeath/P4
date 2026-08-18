using UnityEngine;

[CreateAssetMenu(fileName = "DinoFoodSettings", menuName = "Domain/DinoFoodSettings")]
public class DinoFoodSettings : ScriptableObject
{
    [SerializeField]
    private int _minFood = 10;

    [SerializeField]
    private int _maxFood = 20;

    public int MinFood => _minFood;

    public int MaxFood => _maxFood;

    public int ConvertToFood(DinoModel dino)
    {
        if (dino == null)
            return 0;

        int range = dino.WeightMax - dino.WeightMin;
        if (range <= 0)
            return _maxFood;

        float t = Mathf.Clamp01((dino.Weight - dino.WeightMin) / (float)range);
        return Mathf.RoundToInt(Mathf.Lerp(_minFood, _maxFood, t));
    }
}