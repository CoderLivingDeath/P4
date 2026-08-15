using UnityEngine;

public class DinoSetuper : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;

    [SerializeField]
    private DinoModelHodler _hodler;

    [SerializeField]
    private DinoBrainBehaviour _brain;

    public void Setup(Dino dino, BoxCollider2D area)
    {
        if (_renderer != null && dino != null)
            _renderer.sprite = dino.Sprite;

        if (_hodler != null)
            _hodler.dino = dino;

        if (_brain != null)
            _brain.SetArea(area);
    }

    public void Clear()
    {
        if (_hodler != null)
            _hodler.dino = null;

        gameObject.SetActive(false);
    }
}