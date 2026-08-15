using _Project.Scripts.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class PaddockUIController : ContentUi
{
    [SerializeField]
    private PaddockModel _model;

    [SerializeField]
    private List<Slot> _slots;

    [SerializeField]
    private UnityEvent<int> _onSlotClicked;

    private void OnEnable()
    {
        if (_model != null)
            _model.SlotChanged += OnSlotChanged;

        if (_slots == null)
            return;

        for (int i = 0; i < _slots.Count; i++)
        {
            int index = i;
            Slot slot = _slots[i];
            if (slot == null)
                continue;

            Button btn = slot.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => OnSlotClick(index));
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (_model != null)
            _model.SlotChanged -= OnSlotChanged;

        if (_slots == null)
            return;

        for (int i = 0; i < _slots.Count; i++)
        {
            Slot slot = _slots[i];
            if (slot == null)
                continue;

            Button btn = slot.GetComponent<Button>();
            if (btn != null)
                btn.onClick.RemoveAllListeners();
        }
    }

    private void OnSlotChanged(int index)
    {
        UpdateSlot(index);
    }

    public void SetModel(PaddockModel model)
    {
        if (_model != null)
            _model.SlotChanged -= OnSlotChanged;

        _model = model;

        if (_model != null)
            _model.SlotChanged += OnSlotChanged;

        Refresh();
    }

    public void Refresh()
    {
        if (_model == null || _slots == null)
            return;

        for (int i = 0; i < _slots.Count && i < _model.Count; i++)
            UpdateSlot(i);
    }

    private void UpdateSlot(int index)
    {
        if (_model == null || _slots == null || index >= _slots.Count)
            return;

        Slot slot = _slots[index];
        if (slot == null)
            return;

        Dino dino = _model.GetSlot(index);

        if (slot.Root != null)
            slot.Root.SetActive(dino != null);

        if (slot.Icon != null)
        {
            if (dino != null && dino.Sprite != null)
            {
                slot.Icon.texture = dino.Sprite.texture;
                slot.Icon.color = Color.white;
            }
            else
            {
                slot.Icon.texture = null;
                slot.Icon.color = Color.clear;
            }
        }

        if (slot.Label != null)
            slot.Label.text = dino != null ? dino.Name : "";
    }

    private void OnSlotClick(int index)
    {
        _onSlotClicked?.Invoke(index);
    }
}