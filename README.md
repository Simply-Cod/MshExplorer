# Msh Explorer

**Msh Explorer** is a terminal file explorer made in C#.
Designed to make file and directory navigation in the terminal fast and efficient

It allows users to browse, create, edit (from a list of editors) and delete files and directories.
![Screenshot](https://github.com/Simply-Cod/MshExplorer/blob/master/media/Msh-ExplorerScreenshot1.png)

## Features
**Navigate**        - Navigate in and out of directories fast.

**Create/Delete**   - Create or delete  files and directories in the terminal.

**Copy/Paste**      - Copy and paste files and directories.

**Search**          - Search for files and folder in the current directory.

**Commands**        - In explorer commandline (more features planned).

**Style**           - Style can be turned off if you wish using the 'config' command.


## Known Issues
**Gnome Terminal Compatibility:**
- The Gnome terminal may struggle with large amounts of escape codes.

**Workarounds:**
- Disable list window styling with the config command, **or**
- Run Msh Explorer inside tmux, which handles escape codes more efficiently.

## Roadmap
-[ ] File preview panel
-[ ] Bookmarks - create a list of bookmarks that is stored in a saved file.

