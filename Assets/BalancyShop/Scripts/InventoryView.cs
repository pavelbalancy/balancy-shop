using System;
using System.Collections.Generic;
using Balancy;
using Balancy.Example;
using Balancy.Models.SmartObjects;
using UnityEngine;

namespace BalancyShop
{
    public class InventoryView : MonoBehaviour
    {
        private const string ITEM_GOLD = "608";
        private const string ITEM_GEM = "609";
        
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private int minInventorySize = 2;
        [SerializeField] private bool resourcesBag;

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
            SynchInventoryData();
            PrepareInitialSlots();
            Subscribe();
        }

        private void PrepareInitialSlots()
        {
            CleanUpInventorySlots();
            CreateEmptySlotViews();
            FillSlotsWithContent();
        }

        private void SynchInventoryData()
        {
            if (_inventory.GetSlotsCount() < minInventorySize)
                _inventory.SetInventorySize(minInventorySize);

            if (resourcesBag)
            {
                _inventory.SetAcceptableItem(DataEditor.GetModelByUnnyId<Item>(ITEM_GOLD), 0);
                _inventory.SetAcceptableItem(DataEditor.GetModelByUnnyId<Item>(ITEM_GEM), 1);
            }
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

            var slot = _inventory.GetCopyOfSlot(index);
            _inventorySlots[index].SlotView.UpdateData(slot);
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
