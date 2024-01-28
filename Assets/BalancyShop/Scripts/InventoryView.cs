using System;
using System.Collections.Generic;
using Balancy.Example;
using Balancy.Models.SmartObjects;
using UnityEngine;

namespace BalancyShop
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private int minInventorySize = 2;

        private Balancy.Data.SmartObjects.Inventory _inventory;
        private bool _subscribedForTheInventory;
        
        private class InventorySlot
        {
            public InventorySlotView SlotView;
        }

        private readonly List<InventorySlot> _inventorySlots = new List<InventorySlot>();
        
        public void Init(Balancy.Data.SmartObjects.Inventory inventory)
        {
            if (_inventory == inventory)
                return;
            
            Unsubscribe();
            _inventory = inventory;
            SynchSlotsCount();
            PrepareInitialSlots();
            Subscribe();
        }

        private void PrepareInitialSlots()
        {
            CleanUpInventorySlots();
            CreateEmptySlotViews();
            FillSlotsWithContent();
        }

        private void SynchSlotsCount()
        {
            if (_inventory.GetSlotsCount() < minInventorySize)
                _inventory.SetInventorySize(minInventorySize);
        }

        private void CreateEmptySlotViews()
        {
            var inventorySize = _inventory.GetSlotsCount();
            for (int i = 0; i < inventorySize; i++)
                AddEmptySlotView();
        }

        private void AddEmptySlotView()
        {
            var newItem = Instantiate(slotPrefab, content);
            var slotView = newItem.GetComponent<InventorySlotView>();
            _inventorySlots.Add(new InventorySlot
            {
                SlotView = slotView
            });
        }

        private void FillSlotsWithContent()
        {
            for (int i = 0; i < _inventorySlots.Count; i++)
                UpdateSlotView(i);
        }

        private void OnItemsChanged(Item item, int count, int slotIndex)
        {
            UpdateSlotView(slotIndex);
        }

        private void UpdateSlotView(int index)
        {
            while (_inventorySlots.Count <= index)
                AddEmptySlotView();

            var itemInstance = _inventory.GetCopyOfItemInSlot(index);
            _inventorySlots[index].SlotView.UpdateData(itemInstance);
        }

        private void OnDestroy()
        {
            Unsubscribe();
            CleanUpInventorySlots();
        }

        private void CleanUpInventorySlots()
        {
            content.RemoveChildren();
            _inventorySlots.Clear();
        }

        private void Subscribe()
        {
            if (_inventory == null || _subscribedForTheInventory)
                return;

            _subscribedForTheInventory = true;

            _inventory.OnItemWasRemoved += OnItemsChanged;
            _inventory.OnNewItemWasAdded += OnItemsChanged;
        }

        private void Unsubscribe()
        {
            if (_inventory == null || !_subscribedForTheInventory)
                return;
            
            _subscribedForTheInventory = false;

            _inventory.OnItemWasRemoved -= OnItemsChanged;
            _inventory.OnNewItemWasAdded -= OnItemsChanged;
        }
    }
}
