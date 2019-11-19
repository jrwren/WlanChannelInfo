// Copyright 2010 Jay R. Wren - http://jrwren.wrenfam.com/blog/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WlanChannelInfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnInitialized(EventArgs e)
        {            
            base.OnInitialized(e);
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            this.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
        }

        void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var gridView = new GridView();
            foreach (System.Reflection.PropertyInfo property in (dataGrid.ItemsSource as System.Collections.IList)[0].GetType().GetProperties())
            {
                var col = new GridViewColumn { Header = property.Name, DisplayMemberBinding = new Binding(property.Name) };
                gridView.Columns.Add(col);
            }
            dataGrid.View = gridView;
        }

        System.ComponentModel.ListSortDirection _lastDirection;
        GridViewColumnHeader _lastHeaderClicked;
        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            System.ComponentModel.ListSortDirection direction;

            if (headerClicked != null &&
                headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                if (headerClicked != _lastHeaderClicked)
                {
                    direction = System.ComponentModel.ListSortDirection.Ascending;
                }
                else
                {
                    if (_lastDirection == System.ComponentModel.ListSortDirection.Ascending)
                    {
                        direction = System.ComponentModel.ListSortDirection.Descending;
                    }
                    else
                    {
                        direction = System.ComponentModel.ListSortDirection.Ascending;
                    }
                }

                // see if we have an attached SortPropertyName value
                string sortBy = (headerClicked.Column.DisplayMemberBinding as Binding).Path.Path;
                if (string.IsNullOrEmpty(sortBy))
                {
                    // otherwise use the column header name
                    sortBy = headerClicked.Column.Header as string;
                }
                sort(sortBy, direction);

                _lastHeaderClicked = headerClicked;
                _lastDirection = direction;
            }
        }

        void sort(string sortby, System.ComponentModel.ListSortDirection listSortDirection)
        {
            System.ComponentModel.ICollectionView dataView = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

            if (dataView != null)
            {
                dataView.SortDescriptions.Clear();
                System.ComponentModel.SortDescription sd = new System.ComponentModel.SortDescription(sortby, listSortDirection);
                dataView.SortDescriptions.Add(sd);
                dataView.Refresh();
            }
        }
        
        private void RefreshCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.dataGrid.ItemsSource = new InterfaceService().GetWifiInfo();
        }
        void NewCommandExecuted(object sender, ExecutedRoutedEventArgs e) { }
        void OpenCommandExecuted(object sender, ExecutedRoutedEventArgs e) { }
        void ExitCommandExecuted(object sender, ExecutedRoutedEventArgs e) { this.Close(); }

        private void AboutCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var aboutmessage =
@"
WlanChannelInfo is a dead simple UI to the Channel and Strength 
values in the Wlan* API newly exposed in Windows 7. 

Copyright 2010 Jay R. Wren - http://jrwren.wrenfam.com/blog/

Licensed under the Apache License, Version 2.0 (the ""License"");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an ""AS IS"" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
";
            MessageBox.Show(aboutmessage, "About");
        }
    }
    
}
