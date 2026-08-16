using UnityEngine;

[CreateAssetMenu(fileName = "QteSequence", menuName = "Domain/QteSequence")]
public class QteSequence : ScriptableObject
{
    [SerializeField]
    private int[] _elements;

    public int Length => _elements != null ? _elements.Length : 0;

    public int GetElement(int index)
    {
        if (_elements == null || index < 0 || index >= _elements.Length)
            return -1;

        return _elements[index];
    }

    public int[] GenerateRandom(int length, int elementCount)
    {
        int maxLength = Mathf.Min(length, elementCount);
        _elements = new int[maxLength];

        // Fisher-Yates shuffle of all possible indices
        int[] pool = new int[elementCount];
        for (int i = 0; i < elementCount; i++)
            pool[i] = i;

        for (int i = elementCount - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        for (int i = 0; i < maxLength; i++)
            _elements[i] = pool[i];

        return _elements;
    }
}