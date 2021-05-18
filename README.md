# RanseiLink

WIP Pokemon Conquest Rom Editor. A WPF app, as well as a netcore console app.

![](https://i.imgur.com/0WpMBxe.png)

See the Wiki for explanations of file formats and data structures used within the game.

## WPF App

![](https://i.imgur.com/m78JNff.png)

Walkthrough:

1. Open the app, on the top bar choose the load rom option (tooltips will appear when you hover over the icons) and choose a rom to be loaded into application data
2. Apply your desired edits. Save the changes with the save button and they will stick around if you close the application and come back later.
3. From the top bar, choose the commit to rom option, and commit the changes to a copy of your original rom

## Console App

Within the console, navigate to the folder containing `RanseiConsole.dll`.

Run `dotnet RanseiConsole.dll -h` to view the command options.

The console app supports scripting using Lua, see: https://github.com/Deijin27/RanseiLink/wiki/Sample-Lua-Scripts

## Preview of console app and scripting

![](https://i.imgur.com/RpHVEgM.png)
