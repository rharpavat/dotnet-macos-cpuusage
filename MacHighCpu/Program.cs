namespace MacHighCpu
{
    using System;
    using System.Threading;
    using System.Net.NetworkInformation;

    class Program
    {
        
        private static void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {   
            CheckNetworkAvailability();   
        }

        private static int ctr = 0;

        // Switch between the given two networks
        private static void SwitchBetweenNetworks(string network1, string network2) {
            ctr++;
            Console.WriteLine($"Connecting to {network1}...ctr=" + ctr);
            BashShell.RunCommand($"networksetup -setairportnetwork en0 {network1}");
            Thread.Sleep(10000);
            Console.WriteLine($"Connecting to {network2}...ctr=" + ctr);
            BashShell.RunCommand($"networksetup -setairportnetwork en0 {network2}");
        }
 
        private static void CheckNetworkAvailability()
        {
            NetworkInterface.GetAllNetworkInterfaces();
        }

        public static void Main(string[] args)
        {
            // TODO: Change these to the two available WiFi networks to switch between
            string network1 = "Guest";
            string network2 = "wpa2";

            // switch wi-fi networks every 10 seconds.
            Timer _networkSwitcher = new Timer(new TimerCallback((state) =>
            {
                SwitchBetweenNetworks(network1, network2);
            }), "", 20000, 20000);

            // Call the network detection code every second.
            Timer _caller = new Timer(new TimerCallback((state) =>
            {
                NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
            }), "", 1000, 1000);

            Console.WriteLine("App started!");
            //Wait still Sigterm or Ctrl+C shut down
            ConsoleUtility.WaitForShutdown();
            _networkSwitcher.Dispose();
            _caller.Dispose();

            Console.WriteLine("Shutdown!");
        }
    }
}
