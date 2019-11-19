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
using System.Windows.Input;

namespace WlanChannelInfo
{
    public class CustomCommands
    {
        static CustomCommands()
        {
            Exit = new RoutedUICommand("E_xit", "Exit", typeof(CustomCommands));
            Import = new RoutedUICommand("Import", "Import", typeof(CustomCommands));
            ToggleStatusBar = new RoutedUICommand("StatusBar", "StatusBar", typeof(CustomCommands));
            Accept = new RoutedUICommand("Accept", "Accept", typeof(CustomCommands));
            About = new RoutedUICommand("About", "About", typeof(CustomCommands));
        }
        public static RoutedUICommand Exit { get; private set; }
        public static RoutedUICommand Import { get; private set; }
        public static RoutedUICommand ToggleStatusBar { get; private set; }
        public static RoutedUICommand Accept { get; private set; }
        public static RoutedUICommand About { get; private set; }
    }
}
