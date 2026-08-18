using UnityEngine;

public class DinoModel
{
    public string Name { get; }

    public Sprite Sprite { get; }

    public DinoType Type { get; }

    public DinoSex Sex { get; }

    public int Weight { get; }

    public int WeightMin { get; }

    public int WeightMax { get; }

    public int Age { get; }

    public int Fertility { get; }

    public DinoModel(string name, Sprite sprite, DinoType type, DinoSex sex, int weight, int weightMin, int weightMax, int age, int fertility)
    {
        Name = name;
        Sprite = sprite;
        Type = type;
        Sex = sex;
        Weight = weight;
        WeightMin = weightMin;
        WeightMax = weightMax;
        Age = age;
        Fertility = fertility;
    }

    public int GetStatValue(StatType type)
    {
        switch (type)
        {
            case StatType.Sex:
                return (int)Sex;
            case StatType.Weight:
                return Weight;
            case StatType.Age:
                return Age;
            case StatType.Fertility:
                return Fertility;
            default:
                return 0;
        }
    }
}