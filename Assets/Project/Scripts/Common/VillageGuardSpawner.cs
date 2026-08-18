using UnityEngine;

public class VillageGuardSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _dinoPrefab;

    [SerializeField]
    private BoxCollider2D _area;

    [SerializeField]
    private Transform _spawnRoot;

    public void SpawnGuard(DinoModel dino)
    {
        if (_dinoPrefab == null || _spawnRoot == null || _area == null || dino == null)
            return;

        Bounds bounds = _area.bounds;
        Vector3 pos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            0f);

        GameObject go = Instantiate(_dinoPrefab, _spawnRoot);
        go.name = "Guard_" + dino.Name;
        go.transform.position = pos;

        DinoSetuper setuper = go.GetComponentInChildren<DinoSetuper>();
        if (setuper != null)
            setuper.Setup(dino, _area);

        go.SetActive(true);
    }
}
