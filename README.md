# dotnet-macos-cpuusage
Min-repro example for sustained high CPU usage w/ dotnet 3 on macOS 10.15 Catalina. References https://github.com/dotnet/corefx/issues/42634.

## Program details
This program executes a network availability check every second using the .NET NetworkInterface API, programmatically switching Wi-Fi networks every 10 seconds. This is an exaggerated, simplified version of our actual use case, where switching networks triggers a network check in the application. This version forces more rapid reproduction of the high CPU usage issue.

## Context
We are running into an issue with .NET Core 3.0 on macOS 10.15 Catalina where running this example program for several minutes eventually causes the CPU usage to shoot up above 50% and remain there consistently until the app is terminated/restarted.

## How to run
### Requirements: 
- macOS 10.15 Catalina
- .NET Core 3.0
- 2 different known Wi-Fi networks that are known to the machine - this is necessary so we can programmatically switch between them

### Steps to reproduce:
1. Edit the Program.cs file's `main()` method, assigning `network1` and `network2` to the names of the two Wi-Fi networks to switch between.
2. Run the program using `dotnet run Program.cs`.
3. Monitor the app in Activity Monitor. It will run under the name `mac-high-cpu`.
4. After a few minutes, you should see the CPU usage spike and stay consistently high.
