using UnityEngine;

public static class DinoFactory
{
    private const int MinAge = 5;
    private const int MaxAge = 20;
    private const int MinFertility = 5;
    private const int MaxFertility = 100;

    public static DinoModel Create(Dino template)
    {
        if (template == null)
            return null;

        DinoSex sex = Random.value < 0.5f ? DinoSex.Male : DinoSex.Female;
        int weight = Random.Range(template.WeightMin, template.WeightMax + 1);
        int age = Random.Range(MinAge, MaxAge + 1);
        int fertility = Random.Range(MinFertility, MaxFertility + 1);

        DinoModel dino = new DinoModel(template.Name, template.Sprite, template.Type, sex, weight, template.WeightMin, template.WeightMax, age, fertility);

        Debug.Log(string.Format("New dino created: {0} ({1}) | sex: {2} | weight: {3} | age: {4} | fertility: {5}%",
            dino.Name, dino.Type, dino.Sex, dino.Weight, dino.Age, dino.Fertility));

        return dino;
    }
}