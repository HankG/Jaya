﻿using Jaya.Shared;
using Jaya.Shared.Base;
using Jaya.Shared.Models;
using Jaya.Ui.Models;
using Jaya.Ui.Services;
using System;
using System.Windows.Input;

namespace Jaya.Ui.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        readonly SharedService _shared;
        ICommand _populateCommand;
        TreeNodeModel _selectedNode;

        public NavigationViewModel()
        {
            _shared = GetService<SharedService>();

            Node = new TreeNodeModel(null, null);
            PopulateCommand.Execute(Node);
        }

        #region properties

        public ICommand PopulateCommand
        {
            get
            {
                if (_populateCommand == null)
                    _populateCommand = new RelayCommand<TreeNodeModel>(PopulateAction);

                return _populateCommand;
            }
        }

        public PaneConfigModel PaneConfig => _shared.PaneConfiguration;

        public ApplicationConfigModel ApplicationConfig => _shared.ApplicationConfiguration;

        public TreeNodeModel Node { get; }

        public TreeNodeModel SelectedNode
        {
            get => _selectedNode;
            set
            {
                _selectedNode = value;

                if (value == null)
                    return;

                if (value.FileSystemObject != null)
                {
                    var args = new DirectoryChangedEventArgs(value.Service, value.Account, value.FileSystemObject as DirectoryModel);
                    EventAggregator.Publish(args);
                }
            }
        }

        #endregion

        void OnNodeExpanded(TreeNodeModel node, bool isExpaded)
        {
            if (!isExpaded)
                return;

            if (node.IsHavingDummyChild)
                PopulateCommand.Execute(node);
        }

        void AddChildNode(TreeNodeModel node, TreeNodeModel childNode)
        {
            Invoke(() => node.Children.Add(childNode));
        }

        void RemoveChildNode(TreeNodeModel node, TreeNodeModel childNode)
        {
            Invoke(() => node.Children.Remove(childNode));
        }

        async void PopulateAction(TreeNodeModel node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.IsExpanded && !node.IsHavingDummyChild)
                return;

            if (node.Service == null)
            {
                foreach (var service in GetService<ProviderService>().Providers)
                {
                    var serviceInstance = service as ProviderServiceBase;

                    var serviceNode = new TreeNodeModel(service as ProviderServiceBase, null);
                    serviceNode.Label = service.Name;
                    serviceNode.ImagePath = service.ImagePath;
                    serviceNode.NodeExpanded += OnNodeExpanded;
                    serviceNode.AddDummyChild();
                    AddChildNode(node, serviceNode);

                    serviceInstance.AccountAdded += (AccountModelBase account) => OnAccountAction(account, AccountAction.Added, serviceNode);
                    serviceInstance.AccountRemoved += (AccountModelBase account) => OnAccountAction(account, AccountAction.Removed, serviceNode);
                }
            }
            else if (node.Account == null)
            {
                var accounts = await node.Service.GetAccountsAsync();
                foreach (var account in accounts)
                {
                    var accountNode = new TreeNodeModel(node.Service, account);
                    accountNode.Label = account.Name;
                    accountNode.FileSystemObject = new DirectoryModel();
                    accountNode.ImagePath = account.ImagePath;
                    accountNode.NodeExpanded += OnNodeExpanded;
                    accountNode.AddDummyChild();
                    AddChildNode(node, accountNode);
                }
            }
            else
            {
                var currentDirectory = await node.Service.GetDirectoryAsync(node.Account, node.FileSystemObject as DirectoryModel);
                foreach (var directory in currentDirectory.Directories)
                {
                    var fileSystemObjectNode = new TreeNodeModel(node.Service, node.Account);
                    fileSystemObjectNode.Label = directory.Name;
                    fileSystemObjectNode.FileSystemObject = directory;
                    fileSystemObjectNode.NodeExpanded += OnNodeExpanded;
                    if (directory.Type == FileSystemObjectType.Drive)
                        fileSystemObjectNode.ImagePath = "Hdd-16.png".GetImageUrl();
                    fileSystemObjectNode.AddDummyChild();
                    AddChildNode(node, fileSystemObjectNode);
                }
            }

            node.RemoveDummyChild();
        }

        void OnAccountAction(AccountModelBase account, AccountAction action, TreeNodeModel node)
        {
            if (action == AccountAction.Added)
            {
                var accountNode = new TreeNodeModel(node.Service, account);
                accountNode.Label = account.Name;
                accountNode.FileSystemObject = new DirectoryModel();
                accountNode.ImagePath = account.ImagePath;
                accountNode.NodeExpanded += OnNodeExpanded;
                accountNode.AddDummyChild();
                AddChildNode(node, accountNode);
            }
            else if (action == AccountAction.Removed)
            {
                foreach (var accountNode in node.Children)
                {
                    if (!accountNode.Account.Equals(account))
                        continue;

                    RemoveChildNode(node, accountNode);
                    break;
                }
            }
        }
    }
}
