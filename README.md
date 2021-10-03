# RanseiLink

WIP Pokemon Conquest Rom Editor. A netcore windows app, as well as a netcore console app.

![](https://i.imgur.com/0WpMBxe.png)

See the Wiki for explanations of file formats and data structures used within the game.

## Windows App

![](https://i.imgur.com/4F25Bqv.png)

How to get it running on your computer:

1. Go to https://github.com/Deijin27/RanseiLink/releases
2. Find the latest release
3. On the page of the latest release, you will see a section called "Assets", open that and download `RanseiLink.<version>.zip`
4. Extract from the zip using an application like 7zip
5. Run the extracted exe.
6. You may be prompted to download the .NET Core 3.1 Runtime at this [page](https://dotnet.microsoft.com/download/dotnet/3.1/runtime?utm_source=getdotnetcore&utm_medium=referral). If so, download and install the "Run Destktop Apps x64".
7. Once that's done you should be able to run the application.

How to use:

- To get started with a mod, choose the "Create Mod" option. You will have to provide it with an unchanged rom to base the mod on. From there click on the mod that appears in the list and start editing.
- Once you have finished editing, you can write the mod to a rom. The button to do this is both on the editing page, and also if you right click on the mod in the list.

## Console App

Within the console, navigate to the folder containing `RanseiConsole.dll`.

Run `dotnet RanseiConsole.dll -h` to view the command options.

The console app supports scripting using Lua, see: https://github.com/Deijin27/RanseiLink/wiki/Sample-Lua-Scripts

## Preview of console app and scripting

![](https://i.imgur.com/JSPIMkU.png)
