using System;
using System.Collections.Generic;

public class DinoQueueService
{
    private readonly List<DinoModel> _dinos = new List<DinoModel>();

    public IReadOnlyList<DinoModel> Dinos => _dinos;

    public event Action Changed;

    public void Add(DinoModel dino)
    {
        if (dino == null)
            return;

        _dinos.Add(dino);
        Changed?.Invoke();
    }

    public void Remove(DinoModel dino)
    {
        if (_dinos.Remove(dino))
            Changed?.Invoke();
    }
}