using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WlanChannelInfo.Test
{
    [TestClass]
    public class WhenUsingInterfaceService
    {
        InterfaceService interfaceService = new InterfaceService();
        [TestMethod]
        public void GetWifiDataReturnsData()
        {
            var infos = (IEnumerable<object>)interfaceService.GetWifiInfo();
            Assert.IsTrue(infos.Count() > 0);
            foreach (var item in infos)
            {
                Console.WriteLine(item);
            }
        }
        [TestMethod]
        public void NiceLinq_nice()
        {
            var client = new NativeWifi.WlanClient();
            System.Diagnostics.Debug.WriteLine("GetWifiInfo : " + client);
            Assert.IsTrue(client.Interfaces.Count() > 0);
            var things = 
                from wlanIface in client.Interfaces
                from bssentry in wlanIface.GetNetworkBssList()
                from network in wlanIface.GetAvailableNetworkList(NativeWifi.Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles)
                where InterfaceService.GetStringForSSID(network.dot11Ssid) == InterfaceService.GetStringForSSID(bssentry.dot11Ssid)
                select new
                {
                    bssentry = InterfaceService.GetStringForSSID(bssentry.dot11Ssid),
                    channel = Wifi.FrequencyChannelMap[bssentry.chCenterFrequency],
                    frequency = bssentry.chCenterFrequency,
                    strength = bssentry.rssi,
                    signalQuality = network.wlanSignalQuality,
                    WifiType = network.dot11BssType,
                };
            Assert.IsTrue(things.Count() > 0);
            foreach (var item in things)
            {
                Console.WriteLine("bssentry {0} channel {1} freq {2} strength {4} wifitype {3}", item.bssentry,
                             item.channel,
                             item.frequency,
                             item.WifiType,
                             item.signalQuality
                             );
            }
        }
    }
}
