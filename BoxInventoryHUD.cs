using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiBoxCarry
{
    internal sealed class BoxInventoryHUD : MonoBehaviour
    {
        private bool _created;

        private GameObject _textGO;
        private GameObject _bgGO;
        private TextMeshProUGUI _text;

        private PlayerObjectHolder holder;

        public BoxInventoryHUD(IntPtr ptr) : base(ptr) { }

        private void Update()
        {
            if (!_created)
                TryCreateHud();

            if (_text != null)
                UpdateHudText();
        }

        private void TryCreateHud()
        {
            GameObject uiRoot = GameObject.Find("---UI---");
            if (uiRoot == null)
                return;

            Transform ingameCanvas = uiRoot.transform.Find("Ingame Canvas");
            if (ingameCanvas == null)
            {
                Plugin.Log.LogWarning("[HUD] Ingame Canvas not found");
                return;
            }

            Transform timeTransform = ingameCanvas.Find("Time");
            Transform timeBgTransform = ingameCanvas.Find("Time BG");

            if (timeTransform == null || timeBgTransform == null)
            {
                Plugin.Log.LogWarning("[HUD] Time or Time BG not found");
                return;
            }

            holder = GameObject.FindObjectOfType<PlayerObjectHolder>();
            if (holder == null)
                return;

            // Find the vanilla TMP so we can copy its look only
            TextMeshProUGUI sourceText =
                timeTransform.GetComponent<TextMeshProUGUI>() ??
                timeTransform.GetComponentInChildren<TextMeshProUGUI>(true);

            if (sourceText == null)
            {
                Plugin.Log.LogWarning("[HUD] Could not find source TMP on Time");
                return;
            }

            // Create fresh text object
            _textGO = new GameObject("InventoryHUD");
            _textGO.transform.SetParent(ingameCanvas, false);

            RectTransform textRect = _textGO.AddComponent<RectTransform>();

            RectTransform sourceRect = sourceText.GetComponent<RectTransform>();

            // Copy anchor/pivot/size from vanilla time text
            textRect.anchorMin = sourceRect.anchorMin;
            textRect.anchorMax = sourceRect.anchorMax;
            textRect.pivot = sourceRect.pivot;
            textRect.sizeDelta = sourceRect.sizeDelta;
            textRect.anchoredPosition = sourceRect.anchoredPosition + new Vector2(0f, -40f);
            textRect.localScale = new Vector3(1.2f, 1.2f, 1f);

            _text = _textGO.AddComponent<TextMeshProUGUI>();

            CopyTmpStyle(sourceText, _text);
            _text.text = "0";
            _text.alignment = sourceText.alignment;

            // Background can still be cloned if you want,
            // but we remove scripts by only keeping Image hierarchy.
            _bgGO = GameObject.Instantiate(timeBgTransform.gameObject, ingameCanvas);
            _bgGO.name = "InventoryHUD_BG";

            RectTransform bgRect = _bgGO.GetComponent<RectTransform>();
            if (bgRect != null)
            {
                bgRect.anchoredPosition = textRect.anchoredPosition + new Vector2(20f, 0f);
                bgRect.localScale = new Vector3(0.4f, 0.4f, 1f);
            }

            _bgGO.transform.SetSiblingIndex(_textGO.transform.GetSiblingIndex());

            _textGO.SetActive(false);
            _bgGO.SetActive(false);

            _created = true;
            Plugin.Log.LogInfo("[HUD] Fresh TMP HUD created");
        }

        private void CopyTmpStyle(TextMeshProUGUI source, TextMeshProUGUI dest)
        {
            dest.font = source.font;
            dest.fontSharedMaterial = source.fontSharedMaterial;
            dest.fontSize = source.fontSize;
            dest.color = source.color;
            dest.alpha = source.alpha;
            dest.enableWordWrapping = source.enableWordWrapping;
            dest.overflowMode = source.overflowMode;
            dest.richText = source.richText;
            dest.raycastTarget = false;
            dest.horizontalAlignment = source.horizontalAlignment;
            dest.verticalAlignment = source.verticalAlignment;
            dest.margin = source.margin;
        }


        private void UpdateHudText()
        {
            BoxInventory inventory = PlayerInventoryManager.Inventory;
            int queueCount = inventory != null ? inventory.Count : 0;

            int heldCount = (holder != null && holder.CurrentObject != null) ? 1 : 0;
            int totalCount = queueCount + heldCount;

            bool shouldShow = totalCount > 0;

            if (_textGO != null)
                _textGO.SetActive(shouldShow);

            if (_bgGO != null)
                _bgGO.SetActive(shouldShow);

            string newText = totalCount.ToString();
            if (_text.text != newText)
                _text.text = newText;
        }
    }
}