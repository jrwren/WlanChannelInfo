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
using System.Collections.ObjectModel;
using ManagedWifi;

namespace WlanChannelInfo
{
    public class Wifi
    {
        public readonly static Dictionary<uint, int> FrequencyChannelMap = new Dictionary<uint, int>
        {
            { 2412000, 1},
            { 2417000, 2},
            { 2422000, 3},
            { 2427000, 4},
            { 2432000, 5},
            { 2437000, 6},
            { 2442000, 7},
            { 2447000, 8},
            { 2452000, 9},
            { 2457000, 10},
            { 2462000, 11},
            { 2467000, 12},
            { 2472000, 13},
            { 2484000, 14}, // japan
        };
        public static int GetChannelFromFrequency(uint frequency)
        {
            if (FrequencyChannelMap.ContainsKey(frequency))
                return FrequencyChannelMap[frequency];
            return -1;
        }
    }
    public class InterfaceService
    {
        public static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString( ssid.SSID, 0, (int) ssid.SSIDLength );
        }
        
        internal static string output()
        {
            StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
             var client = new WlanClient();

             try
             {
                 
                 foreach (var wlanIface in client.Interfaces)
                 {
                     foreach (var bssentry in wlanIface.GetNetworkBssList())
                     {
                         sw.WriteLine("bssentry {0} channel {1} freq {2} strength {4} link quality {3}",
                             GetStringForSSID(bssentry.BaseEntry.dot11Ssid), 
                             Wifi.FrequencyChannelMap[bssentry.BaseEntry.chCenterFrequency],
                             bssentry.BaseEntry.chCenterFrequency,
                             bssentry.BaseEntry.linkQuality,
                             bssentry.BaseEntry.rssi
                             );
                     }
                     // Lists all networks with WEP security
                     var networks = wlanIface.GetAvailableNetworkList(0); // no flags
                     foreach (Wlan.WlanAvailableNetwork network in networks)
                     {
                         //if ( network.dot11DefaultCipherAlgorithm == Wlan.Dot11CipherAlgorithm.WEP )
                         {
                             sw.WriteLine("Found WEP network with SSID {0}. with strength {1} and {2}", 
                                 GetStringForSSID(network.dot11Ssid), 
                                 network.wlanSignalQuality, 
                                 //network.dot11BssType
                                 network.Dot11PhyTypes
                                 );
                         }
                     }
                     
                 }
                 return sw.ToString();
             }
             catch (Exception ex)
             {
                 sw.WriteLine(ex);
                 return sw.ToString();
             }
        }
        public System.Collections.IEnumerable GetWifiInfo()
        {
            var client = new WlanClient();
            System.Diagnostics.Debug.WriteLine("GetWifiInfo : "+client);
            var retval =  
                from wlanIface in client.Interfaces
                from bssentry in wlanIface.GetNetworkBssList()
                from network in wlanIface.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles)
                where 
                GetStringForSSID(network.dot11Ssid) == GetStringForSSID(bssentry.BaseEntry.dot11Ssid)
                select new WifiInfo
                         {
                             bssentry = GetStringForSSID(bssentry.BaseEntry.dot11Ssid),
                             channel = Wifi.FrequencyChannelMap.ContainsKey(bssentry.BaseEntry.chCenterFrequency) ? 
                                Wifi.FrequencyChannelMap[bssentry.BaseEntry.chCenterFrequency]: -1,
                             frequency = bssentry.BaseEntry.chCenterFrequency,
                             linqQuality = bssentry.BaseEntry.linkQuality,
                             strength = bssentry.BaseEntry.rssi,
                             signalQuality = network.wlanSignalQuality,
                             // wifitype = network.dot11Bsstype
                             wifitype = network.Dot11PhyTypes
                         };
            retval = retval.ToList();
            System.Diagnostics.Debug.Assert(retval.Count() > 0);
            return retval;
        }
        
    }
    public class WifiInfo
    {
        public string bssentry { get; set; }
        public int channel { get; set; }
        public uint frequency { get; set; }
        public uint linqQuality { get; set; }
        public int strength { get; set; }
        public uint signalQuality { get; set; }
        //public Wlan.Dot11BssType wifitype { get; set; }
        public Wlan.Dot11PhyType[] wifitype {get;set;}
    }

    public static class EnumerableExt
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
    }
}
