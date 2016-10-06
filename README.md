# ATS/ETS 2 Engine, Sounds and Transmission Editor
The ATSEngineTool is an open source desktop application that is used to create/modify engines, sounds and transmissions for SCS Software's American Truck Simulator and Euro Truck Simulator 2. This app is able to generate the required SII files that the game uses for its engines, transmissions and sounds. It can automatically merge the generated SII files right into my Real Engines and Sounds mod, or in a serpeate compile directory.

__Please bear with me as I complete this Readme, it is still a work in progress__.

## Table of Contents

- [Requirements](#requirements)
- [Creating, Modifying and Removing Engines](#creating-modifying-and-removing-engines)
- [Modifying Sound Packages](#modifying-sound-packages)
- [Creating Sound Packages](#creating-sound-packages-espack)
- [Program Updates](#updates)
- [FAQ](#faq)

## Requirements
 * Windows 7 SP1 or newer
 * Microsoft .NET 4.6.1 or newer

## Creating, Modifying and Removing Engines
Creating and adding new engines or transmissions to your game **can** break your save game if you ever decide to **delete** these newly created items. The game by default uses a fallback file in case an accessory goes missing, but I cannot guarantee that it always works. Make sure that any engines you do add to the game are not installed in **any** of your trucks before **deleting them**. The default SCS engines are left in the program database, and have "(SCS)" appended to their names so they are easy to find in game.

## Modifying Sound Packages
Any time you change an engine's sound package, you need to make sure that your current truck does **NOT** have that engine series installed. For example, if you have an ISX15 605 in your current truck, and you modify the ISx15 sound package to something else (and merging that change with the mod), you will have **NO** truck sounds when you start your game back up. It is best to install an SCS engine before modifying the sound packages to any of the engines to prevent the no sound bug. If you still encouter the sound bug, Please create a topic in this discussion forum and I will get back to you ASAP.

## Creating Sound Packages (.espack)
Sound data is now stored in the AppData.db database file (as of 2.8.0). You are now able to edit your engine sounds data, such as volume and pitch references within the app itself. You will need to create an Engine Sounds Package (.espack) for your custom sounds that you may have imported previously. This is pretty simple to do, and you can use the included sound packages in the release section as a reference. An **E**_ngine_ **S**_ounds_ **Pack**_age_ is basically a ZIP file with a custom file extension. 

There are some changes to the interior and exterior sound files from before as well:

1. You will need to remove the {{{NAME}}} and {{{SUITABLE}}} tags.
2. The sound files now use a few directives for the compiler such as (@EP (engine sound path) and @CP (common sound path)). What was once "/sound/truck/engine/N14/int/sound.ogg" is now "@EP/int/sound.ogg". You can still used long paths, but the with the users ability to modify the package install folder, your hard paths could get broken.
3. You will also need to create a manifest.sii file which can be copied from and modifed from the included sound packages.

Again, use the included sound packages as a reference to update your own custom sound packages! When all is said and done, your sound package should look like so:

![Whoops, Image is missing!](http://puu.sh/ryGyk/e7f53b552a.png "Example .espack")
## Updates
Whenever an update is released for this app, it is completly safe to just drag and drop the updates files on top of your existing files (Move and Replace). Your data is stored in the "/data/AppData.db" database file, and a release package will never have this file included (thus, no data loss when upgrading). Whenever an update is applied to your database file, a backup will be created in the "/data/backups/" folder with the version number. If you ever need to roll back to a previous version, just copy the database from the backups folder and rename it back to "AppData.db".

## FAQ
* **How do I reset my data?** You can reset your data by deleting your "/data/AppData.db" database file. The next time you launch the program, the default data will be copied over from the "/data/Default.db" database.
* **My Truck has no sounds, now what?** If you end up removing the compiled engines and/or sound packages, you may find yourself in a truck with no engine sounds at all. The only for sure way to fix this is to sell or trade in your truck.
