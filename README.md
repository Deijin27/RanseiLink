# RanseiLink

WIP Pokemon Conquest Rom Editor. A WPF app, as well as a netcore console app.

![](https://i.imgur.com/0WpMBxe.png)

See the Wiki for explanations of file formats and data structures used within the game.

## WPF App

![](https://i.imgur.com/m78JNff.png)

How to get it running on your computer:

1. Go to https://github.com/Deijin27/RanseiLink/releases
2. Find the latest release
3. On the page of the latest release, you will see a section called "Assets", open that and download `RanseiLink.Editor.<version>.zip`
4. Extract from the zip using an application like 7zip
5. In the extracted folder, double click `RanseiWpf.exe` to run the app (may not have the `.exe` extension, the one that says it's an application)
6. You may be prompted do download the .NET Core 3.1 Runtime at this [page](https://dotnet.microsoft.com/download/dotnet/3.1/runtime?utm_source=getdotnetcore&utm_medium=referral). If so, download and install the "Run Destktop Apps x64".
7. Once that's done you should be able to double click `RanseiWpf.exe` an run the application.

How to use:

1. Open the app, on the top bar choose the load rom option (tooltips will appear when you hover over the icons) and choose a rom to be loaded into application data
2. Apply your desired edits. Save the changes with the save button and they will stick around if you close the application and come back later.
3. From the top bar, choose the commit to rom option, and commit the changes to a copy of your original rom

## Console App

Within the console, navigate to the folder containing `RanseiConsole.dll`.

Run `dotnet RanseiConsole.dll -h` to view the command options.

The console app supports scripting using Lua, see: https://github.com/Deijin27/RanseiLink/wiki/Sample-Lua-Scripts

## Preview of console app and scripting

![](https://i.imgur.com/RpHVEgM.png)
