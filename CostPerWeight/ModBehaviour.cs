using Duckov.UI;
using Duckov.Utilities;
using ItemStatsSystem;
using SodaCraft.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CostPerWeight
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        GameObject? _row = null;
        TextMeshProUGUI? _labelText = null;
        TextMeshProUGUI? _valueText = null;

        string GetLabel()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return "性价比";
                case SystemLanguage.Korean:
                    return "무게 대비 가치";
                case SystemLanguage.Japanese:
                    return "重量単価";
                default:
                    return "Value Ratio";
            }
        }

        void EnsureRow()
        {
            if (_row != null) return;

            _row = new GameObject("CostPerWeightRow");
            var hlg = _row.AddComponent<HorizontalLayoutGroup>();
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = false;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;

            _labelText = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI, _row.transform);
            _labelText.fontSize = 18f;
            _labelText.alignment = TextAlignmentOptions.Left;
            _labelText.text = GetLabel();

            _valueText = Instantiate(GameplayDataSettings.UIStyle.TemplateTextUGUI, _row.transform);
            _valueText.fontSize = 18f;
            _valueText.alignment = TextAlignmentOptions.Right;
        }

        void Awake()
        {
            Debug.Log("CostPerWeight Loaded!");
        }

        void OnDestroy()
        {
            if (_row != null)
                Destroy(_row);
        }

        void OnEnable()
        {
            ItemHoveringUI.onSetupItem += OnSetupItemHoveringUI;
            ItemHoveringUI.onSetupMeta += OnSetupMeta;
            LocalizationManager.OnSetLanguage += OnSetLanguage;
        }

        void OnDisable()
        {
            ItemHoveringUI.onSetupItem -= OnSetupItemHoveringUI;
            ItemHoveringUI.onSetupMeta -= OnSetupMeta;
            LocalizationManager.OnSetLanguage -= OnSetLanguage;
        }

        private void OnSetLanguage(SystemLanguage lang)
        {
            if (_labelText != null)
                _labelText.text = GetLabel();
        }

        private void OnSetupMeta(ItemHoveringUI uI, ItemMetaData data)
        {
            if (_row != null)
                _row.SetActive(false);
        }

        private void OnSetupItemHoveringUI(ItemHoveringUI uiInstance, Item item)
        {
            EnsureRow();

            if (item == null)
            {
                _row!.SetActive(false);
                return;
            }

            float sellPrice = item.GetTotalRawValue() / 2f;
            float weight = item.TotalWeight;

            _row!.SetActive(true);
            _row.transform.SetParent(uiInstance.LayoutParent);
            _row.transform.localScale = Vector3.one;

            if (weight <= 0f)
            {
                _valueText!.text = "N/A";
                _valueText.color = Color.gray;
                return;
            }

            float ratio = sellPrice / weight;
            _valueText!.text = $"{ratio:F0} $/kg";
            _valueText.color = GetRatioColor(ratio);
        }

        static readonly Color White = new Color(1f, 1f, 1f);
        static readonly Color Green = new Color(0.486f, 1f, 0.486f);
        static readonly Color Blue = new Color(0.486f, 0.835f, 1f);
        static readonly Color Purple = new Color(0.816f, 0.675f, 1f);
        static readonly Color Orange = new Color(1f, 0.863f, 0.141f);
        static readonly Color LightRed = new Color(1f, 0.345f, 0.345f);
        static readonly Color Red = new Color(0.733f, 0f, 0f);

        static Color GetRatioColor(float ratio)
        {
            if (ratio >= 10000f) return Red;
            if (ratio >= 5000f) return LightRed;
            if (ratio >= 2500f) return Orange;
            if (ratio >= 1200f) return Purple;
            if (ratio >= 600f) return Blue;
            if (ratio >= 200f) return Green;
            return White;
        }
    }
}
