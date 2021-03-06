﻿using CustomUI.BeatSaber;
using CustomUI.MenuButton;
using CustomUI.Utilities;
using IPA.Loader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VRUI;

namespace BSIPA_ModList.UI
{
    internal class ModInfoViewController : VRUIViewController
    {
        internal Sprite Icon;
        internal string Name;
        internal string Version;
        internal string Author;
        internal string Description;
        internal PluginLoader.PluginMetadata UpdateInfo;

        private static RectTransform rowTransformOriginal;

        private ModInfoView view;
        private RectTransform rowTransform;
        private Button linkHomeButton;
        private Button linkSourceButton;
        private Button linkDonateButton;

        public void Init(Sprite icon, string name, string version, string author, string description, PluginLoader.PluginMetadata updateInfo, PluginManifest.LinksObject links = null)
        {
            Icon = icon;
            Name = name;
            Version = version;
            Author = author;
            Description = description;
            UpdateInfo = updateInfo;

            if (rowTransformOriginal == null)
                rowTransformOriginal = MenuButtonUI.Instance.GetPrivateField<RectTransform>("menuButtonsOriginal");

            // i also have no clue why this is necessary
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);

            var go = new GameObject("Info View", typeof(RectTransform));
            go.SetActive(false);
            go.AddComponent<RectMask2D>();
            view = go.AddComponent<ModInfoView>();
            var rt = view.transform as RectTransform;
            rt.SetParent(transform);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(1f, 1f);
            rt.anchoredPosition = new Vector2(0f, 0);
            view.Init(this);
            go.SetActive(true);

            if (links != null)
            {
                rowTransform = Instantiate(rowTransformOriginal, rectTransform);
                rowTransform.anchorMin = new Vector2(0f, 0f);
                rowTransform.anchorMax = new Vector2(1f, .15f);
                rowTransform.anchoredPosition = new Vector2(-3.5f, -2f);

                foreach (Transform child in rowTransform)
                {
                    child.name = string.Empty;
                    Destroy(child.gameObject);
                }

                if (links.ProjectHome != null)
                {
                    linkHomeButton = BeatSaberUI.CreateUIButton(rowTransform, "QuitButton", buttonText: "Home", 
                        onClick: () => Process.Start(links.ProjectHome.ToString()));
                    linkHomeButton.GetComponentInChildren<HorizontalLayoutGroup>().padding = new RectOffset(6, 6, 0, 0);
                }
                if (links.ProjectSource != null)
                {
                    linkSourceButton = BeatSaberUI.CreateUIButton(rowTransform, "QuitButton", buttonText: "Source", 
                        onClick: () => Process.Start(links.ProjectSource.ToString()));
                    linkSourceButton.GetComponentInChildren<HorizontalLayoutGroup>().padding = new RectOffset(6, 6, 0, 0);
                }
                if (links.Donate != null)
                {
                    linkDonateButton = BeatSaberUI.CreateUIButton(rowTransform, "QuitButton", buttonText: "Donate", 
                        onClick: () => Process.Start(links.Donate.ToString()));
                    linkDonateButton.GetComponentInChildren<HorizontalLayoutGroup>().padding = new RectOffset(6, 6, 0, 0);
                }
            }
        }

#if DEBUG
        public void Update()
        {
#if ADJUST_INFO_BUTTON_UI_LINKS
            RectTransform rt = rowTransform;

            if (rt == null) return;

            var cpos = rt.anchoredPosition;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x - .1f, cpos.y);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x + .1f, cpos.y);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x, cpos.y + .1f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x, cpos.y - .1f);
            }
            else
                return;

            Logger.log.Debug($"Position now at {rt.anchoredPosition}");
#endif
        }
#endif
    }

    internal class ModInfoView : MonoBehaviour
    {
        private ModInfoViewController controller;

        private TextMeshProUGUI titleText;
        private TextMeshProUGUI authorText;
        private TextMeshProUGUI descText;
        private Image icon;

        public void Init(ModInfoViewController controller)
        {
            this.controller = controller;

            var rectTransform = transform as RectTransform;
            rectTransform.sizeDelta = new Vector2(60f, 10f);

            titleText = BeatSaberUI.CreateText(rectTransform, $"{controller.Name} <size=60%>{controller.Version}", new Vector2(11f, 27.5f));
            titleText.fontSize = 6f;
            authorText = BeatSaberUI.CreateText(rectTransform, controller.Author, new Vector2(11f, 22f));
            authorText.fontSize = 4.5f;
            descText = BeatSaberUI.CreateText(rectTransform, controller.Description, new Vector2(-4.5f, 12f));
            descText.fontSize = 3.5f;
            descText.enableWordWrapping = true;
            descText.overflowMode = TextOverflowModes.ScrollRect;

            icon = new GameObject("Mod Info View Icon", typeof(RectTransform)).AddComponent<Image>();
            icon.gameObject.SetActive(false);
            icon.rectTransform.SetParent(rectTransform, false);
            icon.rectTransform.anchorMin = new Vector2(0.5f, 0.44f);
            icon.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            icon.rectTransform.sizeDelta = new Vector2(60f, 10f);
            icon.rectTransform.anchoredPosition = new Vector2(-27.8f, 27.3f);
            icon.sprite = controller.Icon;
            icon.preserveAspect = true;
            icon.useSpriteMesh = true;
            icon.material = UIUtilities.NoGlowMaterial;
            icon.gameObject.SetActive(true);
        }

#if DEBUG
#if ADJUST_INFO_TEXT_UI_KEYS
        private int currentItem = 0;
#endif
        public void Update()
        {
#if ADJUST_INFO_TEXT_UI_KEYS
            RectTransform rt;
            switch (currentItem)
            {
                default:
                    currentItem = 0;
                    goto case 0; // idk why this is needed tbh
                case 0:
                    rt = titleText.rectTransform;
                    break;
                case 1:
                    rt = authorText.rectTransform;
                    break;
                case 2:
                    rt = descText.rectTransform;
                    break;

            }

            if (Input.GetKeyDown(KeyCode.N))
                currentItem++;

            var cpos = rt.anchoredPosition;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x - .1f, cpos.y);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x + .1f, cpos.y);
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x, cpos.y + .1f);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rt.anchoredPosition = new Vector2(cpos.x, cpos.y - .1f);
            }
            else
                return;

            Logger.log.Debug($"Position now at {rt.anchoredPosition}");
#endif
#if ADJUST_INFO_ICON_UI_KEYS
            var rt = icon.rectTransform;
            if (Input.GetKey(KeyCode.Z))
            { // adjust anchormin
                var cpos = rt.anchorMin;
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rt.anchorMin = new Vector2(cpos.x - .001f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    rt.anchorMin = new Vector2(cpos.x + .001f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    rt.anchorMin = new Vector2(cpos.x, cpos.y + .001f);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    rt.anchorMin = new Vector2(cpos.x, cpos.y - .001f);
                }
                else
                    return;

                Logger.log.Debug($"Anchor min now at {rt.anchorMin}");
            }
            else if(Input.GetKey(KeyCode.X))
            { // adjust anchorMax
                var cpos = rt.anchorMax;
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rt.anchorMax = new Vector2(cpos.x - .001f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    rt.anchorMax = new Vector2(cpos.x + .001f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    rt.anchorMax = new Vector2(cpos.x, cpos.y + .001f);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    rt.anchorMax = new Vector2(cpos.x, cpos.y - .001f);
                }
                else
                    return;

                Logger.log.Debug($"Anchor max now at {rt.anchorMax}");
            }
            else
            {
                var cpos = rt.anchoredPosition;
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    rt.anchoredPosition = new Vector2(cpos.x - .1f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.RightArrow))
                {
                    rt.anchoredPosition = new Vector2(cpos.x + .1f, cpos.y);
                }
                else if (Input.GetKey(KeyCode.UpArrow))
                {
                    rt.anchoredPosition = new Vector2(cpos.x, cpos.y + .1f);
                }
                else if (Input.GetKey(KeyCode.DownArrow))
                {
                    rt.anchoredPosition = new Vector2(cpos.x, cpos.y - .1f);
                }
                else
                    return;

                Logger.log.Debug($"Position now at {rt.anchoredPosition}");
            }
#endif
        }
#endif
        }
}
