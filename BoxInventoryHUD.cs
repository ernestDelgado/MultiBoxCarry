using System;
using TMPro;
using UnityEngine;

namespace MultiBoxCarry
{
    internal sealed class BoxInventoryHUD : MonoBehaviour
    {
        private bool _created;

        private GameObject _textGO;
        private GameObject _bgGO;
        private TextMeshProUGUI _text;

        private PlayerObjectHolder holder;

        public BoxInventoryHUD(IntPtr ptr) : base(ptr)
        {
        }

        private void Update()
        {
            if (!_created)
            {
                TryCreateHud();
            }

            if (_text != null)
            {
                UpdateHudText();
            }
        }

        private void TryCreateHud()
        {
            //CHECKS:
            GameObject uiRoot = GameObject.Find("---UI---");
            if (uiRoot == null)
            {
                return;
            }

            Transform ingameCanvas = uiRoot.transform.Find("Ingame Canvas");
            if (ingameCanvas == null)
            {
                Plugin.Log.LogWarning("[HUD] Ingame Canvas not found");
                return;
            }

            Transform timeTransform = ingameCanvas.Find("Time");
            if (timeTransform == null)
            {
                Plugin.Log.LogWarning("[HUD] Time not found");
                return;
            }

            Transform timeBgTransform = ingameCanvas.Find("Time BG");
            if (timeBgTransform == null)
            {
                Plugin.Log.LogWarning("[HUD] Time BG not found");
                return;
            }

            holder = GameObject.FindObjectOfType<PlayerObjectHolder>();
            if (holder == null)
                return;


            //HUD Creation:
            GameObject textClone = GameObject.Instantiate(timeTransform.gameObject, ingameCanvas);
            textClone.name = "InventoryHUD";
            _textGO = textClone;

            RectTransform textRect = textClone.GetComponent<RectTransform>();
            if (textRect != null)
            {
                textRect.anchoredPosition = new Vector2(
                    textRect.anchoredPosition.x,
                    textRect.anchoredPosition.y - 40f
                );

                textRect.localScale = new Vector3(1.2f, 1.2f, 1f);
            }

            GameObject bgClone = GameObject.Instantiate(timeBgTransform.gameObject, ingameCanvas);
            bgClone.name = "InventoryHUD_BG";
            _bgGO = bgClone;

            RectTransform bgRect = bgClone.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.localScale = new Vector3(0.4f, 0.4f, 1f);

                if (textRect != null)
                {
                    bgRect.anchoredPosition = textRect.anchoredPosition + new Vector2(20f, 0f);
                }
            }

            // Put background behind text
            bgClone.transform.SetSiblingIndex(textClone.transform.GetSiblingIndex());

            _text = textClone.GetComponent<TextMeshProUGUI>();
            if (_text == null)
            {
                _text = textClone.GetComponentInChildren<TextMeshProUGUI>(true);
            }

            if (_text == null)
            {
                Plugin.Log.LogWarning("[HUD] No TextMeshProUGUI found on cloned Time");
                return;
            }

            // Start hidden until count > 0
            if (_textGO != null)
                _textGO.SetActive(false);

            if (_bgGO != null)
                _bgGO.SetActive(false);

            Plugin.Log.LogInfo("[HUD] InventoryHUD created");
            _created = true;
        }

        private void UpdateHudText()
        {
            BoxInventory inventory = PlayerInventoryManager.Inventory;
            int queueCount = inventory != null ? inventory.Count : 0;

            int heldCount = 0;

            if (holder != null && holder.CurrentObject != null)
            {
                heldCount = 1;
            }

            int totalCount = queueCount + heldCount;
            bool shouldShow = totalCount > 0;

            if (_textGO != null)
                _textGO.SetActive(shouldShow);

            if (_bgGO != null)
                _bgGO.SetActive(shouldShow);

            _text.text = totalCount + "";
        }
    }
}