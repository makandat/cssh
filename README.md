cssh v0.1.0 — A Minimal, Modern Shell for Windows
1. Overview
Windows currently provides two major shells: PowerShell and Command Prompt.
PowerShell is powerful but geared toward advanced users, while Command Prompt is a legacy tool from the MS-DOS era and lacks modern features.
cssh aims to fill this gap by providing a lightweight, modern, and easy‑to‑use shell—more capable than Command Prompt, but simpler than PowerShell.
Version v0.1.0 is an early prototype that supports only built‑in commands.
The initial release supports Windows only, with plans to support macOS, Linux, and any environment where .NET runs.

2. User Interface
2.1 Prompt
cssh uses a Bash‑style prompt.
Examples:
root@site:~#
user@site:~$


The ~ represents the current directory.
Its display depth follows the environment variable PROMPT_DIRTRIM.
If the variable is not defined, the default value is 2.

2.2 Modes
cssh has two modes:
- Interactive Mode
- Edit Mode
Use the edit command to enter Edit Mode, and the quit command (inside Edit Mode) to return to Interactive Mode.
In v0.1.0, Edit Mode does not support script editing.
Only the quit command is available.
In Edit Mode, the command input line appears at the bottom of the screen with the prompt:
>



2.3 Startup Screen and Window Title
When cssh starts:
- It enters Interactive Mode.
- The screen is cleared.
- The prompt appears at the top of the window.
The window title is set to:
cssh v0.1.0



3. Built‑in Commands (Interactive Mode)
The syntax and behavior of built‑in commands follow Bash conventions, but functionality is intentionally limited.
Shell variables are not supported in v0.1.x.
3.1 ls (alias: dir)
Displays directory contents.
Options:
- -l : long format
- -a : show hidden files
3.2 cd
Changes the current directory.
Option:
- - : return to the previous directory
3.3 pwd
Displays the current directory.
3.4 cat (alias: type)
Displays the contents of a text file (UTF‑8 only).
Option:
- -n : show line numbers
3.5 rm (alias: del)
Deletes files or directories.
Options:
- -r : recursive
- -f : force
Options may be combined (e.g., rm -rf ...).
3.6 mkdir
Creates a directory (absolute or relative path).
3.7 cp (alias: copy)
Copies a file.
- If the destination is a directory, the file is copied into it.
- Existing files are overwritten.
- Read‑only files require the -f option.
3.8 mv (alias: move, ren)
Moves files or directories.
Option:
- -f : force
3.9 echo
Displays text. Quotation marks are optional.
3.10 which (alias: where)
Indicates whether the specified command is a built‑in command.
External commands are not supported in v0.1, so this command always returns:
- “built‑in command” (Japanese)
- “builtin” (English)
3.11 clear (alias: cls)
Clears the screen.
3.12 help (alias: ?)
Displays help information.
- help : list all available commands
- help <command> : show help for a specific command
- Language follows the OS (English or Japanese)
3.13 exit (alias: quit)
Exits cssh.
3.14 history (alias: h)
Displays command history (Bash‑style).
Execution via ! is not supported in v0.1.
3.15 touch
Updates the timestamp of a file to the current time.
3.16 edit
Enters Edit Mode.

4. Built‑in Commands (Edit Mode)
In v0.1.x, only the quit command is supported.
All other commands are ignored.

5. Platform Differences: Windows, macOS, Linux
v0.1.x supports Windows only, but future versions will support macOS, Linux, and all .NET‑compatible environments.
5.1 Path Handling
- Windows paths are case‑insensitive (c:\ = C:\).
- cssh accepts both \ and / as path separators.
Paths with Spaces
Paths containing spaces may be enclosed in either:
- "..."
- '...'
You may quote the entire path or only the part containing spaces.
Mixed quoting such as "...' is an error.
Environment Variables
- Environment variables cannot be created or modified.
- The export command is not supported.
- Environment variable expansion is not supported in v0.1.x.

6. Limitations in v0.1.x
6.1 Pipelines and Redirection
Not supported:
- |
- >, >>, <
- >> (value pipe)
- >! (error pipe)
These will be introduced in v0.2 or later.
6.2 Command Execution Rules
- Only one command per line is allowed.
- Command chaining with ; is not supported.
- Conditional execution (&&, ||) is not supported.
6.3 External Commands
- External commands (e.g., python, git, dotnet) cannot be executed.
- External command support will be added in v0.2.x along with the run command.
6.4 Shell Variables
- Shell variables are not supported.
- Assignments like VAR=value are not allowed.
6.5 Environment Variables
- Environment variable expansion is not supported.
- %VAR% and $VAR are not recognized.
- Support will be considered in v0.2 or later.


もし追加したい項目があれば、続けて作っていきましょう。

