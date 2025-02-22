﻿using Jaya.Shared;
using Jaya.Shared.Base;
using Jaya.Ui.Models;
using Jaya.Ui.Services;

namespace Jaya.Ui.ViewModels.Windows
{
    public class MainViewModel : ViewModelBase
    {
        readonly Subscription<DirectoryChangedEventArgs> _onDirectoryChanged;
        readonly SharedService _shared;

        public MainViewModel()
        {
            WindowTitle = Constants.APP_NAME;
            _onDirectoryChanged = EventAggregator.Subscribe<DirectoryChangedEventArgs>(DirectoryChanged);
            _shared = GetService<SharedService>();
        }

        ~MainViewModel()
        {
            EventAggregator.UnSubscribe(_onDirectoryChanged);
        }

        public ToolbarConfigModel ToolbarConfig => _shared.ToolbarConfiguration;

        public PaneConfigModel PaneConfig => _shared.PaneConfiguration;

        public ApplicationConfigModel ApplicationConfig => _shared.ApplicationConfiguration;

        public string WindowTitle
        {
            get => Get<string>();
            private set => Set(value);
        }

        void DirectoryChanged(DirectoryChangedEventArgs args)
        {
            if (args.Provider == null)
                WindowTitle = Constants.APP_NAME;
            else if (args.Directory == null || string.IsNullOrEmpty(args.Directory.Path))
                WindowTitle = args.Provider.Name;
            else
                WindowTitle = args.Directory.Name;
        }
    }
}
