using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agricultura {
    public class DynamicUIListPoolable<T> {
        private List<GameObject> items;
        private Queue<GameObject> pool;

        private Transform parent;
        private GameObject prefab;
        private Action<GameObject, T> updateItemFunction;

        public DynamicUIListPoolable(Transform parent, GameObject prefab, Action<GameObject, T> updateItemFunction) {
            this.parent = parent;
            this.prefab = prefab;
            this.updateItemFunction = updateItemFunction;

            items = new List<GameObject>();
            pool = new Queue<GameObject>();
        }

        public void ReplaceAllItems(IEnumerable<T> dataList) {
            int itemCount = 0;
            if (dataList != null) {
                foreach (T data in dataList) {
                    if (itemCount < items.Count) {
                        updateItemFunction(items[itemCount], data);
                    } else {
                        items.Add(CreateOrGetItemFromPool(data));
                    }
                    itemCount++;
                }
            }
            while (itemCount < items.Count) {
                items[itemCount].SetActive(false);
                pool.Enqueue(items[itemCount]);
                items.RemoveAt(itemCount);
            }
        }

        public void ClearItems() {
            foreach (GameObject item in items) {
                item.SetActive(false);
                pool.Enqueue(item);
            }
            items.Clear();
        }

        public void AddItem(T data) {
            items.Add(CreateOrGetItemFromPool(data));
        }

        private GameObject CreateOrGetItemFromPool(T data) {
            GameObject item;
            if (pool.Count > 0) {
                item = pool.Dequeue();
                item.SetActive(true);
            } else {
                item = GameObject.Instantiate(prefab, parent);
            }

            updateItemFunction(item, data);

            return item;
        }
    }
}