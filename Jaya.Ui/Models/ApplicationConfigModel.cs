﻿using Avalonia.ThemeManager;
using Jaya.Shared.Base;
using Newtonsoft.Json;
using System;

namespace Jaya.Ui.Models
{
    public class ApplicationConfigModel : ConfigModelBase
    {
        [JsonProperty]
        public bool IsItemCheckBoxVisible
        {
            get => Get<bool>();
            set => Set(value);
        }

        [JsonProperty]
        public bool IsFileNameExtensionVisible
        {
            get => Get<bool>();
            set => Set(value);
        }

        [JsonProperty]
        public bool IsHiddenItemVisible
        {
            get => Get<bool>();
            set => Set(value);
        }

        [JsonProperty]
        public double WidthPx
        {
            get => Get<double>();
            set => Set(value);
        }

        [JsonProperty]
        public double HeightPx
        {
            get => Get<double>();
            set => Set(value);
        }


        public ITheme Theme
        {
            get => Get<ITheme>();
            set
            {
                if (Set(value))
                    App.ThemeSelector.SelectedTheme = value;
            }
        }

        [JsonProperty]
        string ThemeName
        {
            get => Get<string>();
            set
            {
                Set(value);
                SetTheme(value);
            }
        }

        void SetTheme(string name)
        {
            foreach (var theme in App.ThemeSelector.Themes)
            {
                if (theme.Name.Equals(name, StringComparison.InvariantCulture))
                {
                    Theme = theme;
                    break;
                }
            }
        }

        protected override ConfigModelBase Empty()
        {
            return new ApplicationConfigModel
            {
                WidthPx = 800,
                HeightPx = 600,
                ThemeName = "Dark"
            };
        }
    }
}
