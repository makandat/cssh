# cssh v0.2 (Rev.2) Specification

# 1. Overview
Windows currently provides two major shells: PowerShell and Command Prompt.
PowerShell is powerful but geared toward advanced users, while Command Prompt is a relic from the MS‑DOS era—low‑function and outdated.
Therefore, I wanted a shell that is more modern and capable than Command Prompt, without the complexity of PowerShell.
The cssh version 0.x.x series is a prototype.
The name cssh stands for C# Script Shell.
Supported OS: Windows only.
Future versions will support other operating systems where .NET runs.
Requirements for cssh 0.x
- OS: Windows only
- Requires .NET 10.0 or later
(.NET 8 will be too old by the time cssh is completed)
- Must be launched from Windows Terminal (PowerShell or CMD.exe)

# 2. Screen
## 2.1 Prompt
The prompt follows a PowerShell‑like style.
Example:
```
cssh: C:\workspace\Cssh>
```

## 2.2 Modes
cssh has two modes: normal mode and edit mode.
- Use the edit command to enter edit mode
- Use the quit command (in edit mode) to return to normal mode
- In edit mode, the bottom line of the screen is the command input line, showing the prompt > 
2.3 Startup Screen and Window
When cssh starts, it enters normal mode, clears the screen, and displays the prompt at the top.
The window title is shown as:
```
cssh v0.2.n
```
where 2 is the minor version and n is the build version.

# 3. Built‑in Commands (Normal Mode)
- Built‑in commands follow Bash syntax and behavior.
- Their functionality is limited compared to Bash.
- cssh does not support shell variables.
- Command behavior and output follow Bash conventions.

## 3.1 ls (Alias: dir)
Displays the contents of a directory.
Options:
- -l detailed view
- -a show hidden files

## 3.2 cd
Changes the current directory.
Option:
- '-' (cd -) — returns to the previous directory

## 3.3 pwd
Displays the current directory.
No options.

## 3.4 cat (Alias: type)
Displays the contents of a text file (UTF‑8 only).
Option:
- -n show line numbers

## 3.5 rm (Alias: del)
Deletes files or directories.
Options:
- -r recursive
- -f force
Options may be combined (e.g., rm -rf ...).

## 3.6 mkdir
Creates a directory.
Accepts both absolute and relative paths.

## 3.7 cp (Alias: copy)
Copies a file.
If the destination is a directory, a new file is created under it.
If a file with the same name exists, it is overwritten.
If the file is read‑only, use -f to force overwrite.

## 3.8 mv (Alias: move, ren)
Moves files or directories.
If the destination already exists, behavior follows Bash.
Option:
- -f force

## 3.9 echo
Displays a string or the value of an environment variable.
Strings do not require quotes.

## 3.10 which (Alias: where)
- If the command is built‑in, displays that it is a built‑in command
- If it is an external command, displays its full path

## 3.11 clear (Alias: cls)
Clears the screen.

## 3.12 help
- Displays help for commands
- help alone shows a list of available commands
- help command shows help for that command
- Language follows the OS locale (English or Japanese)

## 3.13 exit (Alias: quit)
Exits cssh.

## 3.14 history (Alias: h)
- Displays command history (Bash‑style)
- History is per session (not persisted)
- !number re‑executes a command from history

## 3.15 touch
- Updates the timestamp of a file
- Creates an empty file if it does not exist

## 3.16 edit
- Enters edit mode
- edit filename loads the file into the edit buffer

## 3.17 sudo (v0.2 Rev.2)
- Required when accessing protected disk areas
- On Windows, shows a confirmation dialog
- (Linux password input is out of scope for this version)

## 3.18 run (v0.2 Rev.2)
Executes the script currently in the edit buffer.
- Arguments may follow run
- If the buffer is empty or contains errors, show an error
- run scriptfile loads the file and executes it
- If unsaved changes exist, ask for confirmation
- On success: remain in normal mode
- On error: return to edit mode, underline the error line, and show the message
- Script output is shown in the normal screen
- Ctrl+C interrupts execution
- sudo run script is allowed

## 3.19 alias (v0.2 Rev.2)
- Bash‑style alias command
- Example: alias ll='ls -l'
- alias alone shows all aliases

## 3.20 External Command Execution (v0.2 Rev.2)
- Use sudo for administrator commands
- Commands follow their own help specifications

# 4. Built‑in Commands in Edit Mode (Added in v0.2)
## 4.1 write (Alias: w)
- Saves the contents of the edit buffer to the file specified as a parameter.
- If the file already exists, a confirmation message is shown before overwriting.
- If w! is used, the file is saved immediately without confirmation.

## 4.2 read (Alias: r)
- Loads the specified file into the edit buffer, replacing its current contents.
- The loaded file name becomes the “current editing file.”
- If the buffer contains unsaved changes, a confirmation message is shown.
- If r! is used, the file is loaded immediately without confirmation.
- The file does not have to be a script; any text file may be loaded.

## 4.3 quit (Alias: q)
- Exits edit mode and returns to normal mode.
- The edit buffer is not cleared; when returning to edit mode later, the previous contents and cursor position remain.

## 4.4 clear
- Clears the edit buffer.
- Clears the display and moves the prompt to the top-left corner.

## 4.5 run
### 4.5.1 Basic Behavior
Executes the contents of the edit buffer as a script.

### 4.5.2 Determining Whether the Buffer Contains a Script
- If the buffer was loaded from a file, the extension is used to determine whether it is a script.
- If the buffer has never been saved, cssh cannot determine whether it is a script or data, so it asks for confirmation:
“Execute as script? (y/n)”
- Only C# scripts (.csx) are supported in this version.

### 4.5.3 Error Handling
- If the script contains errors, the error line is highlighted (underlined).
- The error message is displayed below the error line.
- If execution hangs or becomes unresponsive, Ctrl+C can be used to interrupt it.

### 4.5.4 Privileged Execution
If the script accesses protected disk areas or uses administrator commands internally, prefix the command with sudo (e.g., sudo run).

### 4.5.5 Output and Mode Transition
- Script output (stdout and stderr) is displayed in the normal screen.
- The run command itself is also echoed in the normal screen.
- To return to edit mode after execution, run the edit command.

### 4.5.6 Behavior of run <script>
- run filename.csx loads the file into the edit buffer and executes it.
- If the buffer contains unsaved changes, confirmation is required.
- If execution succeeds, cssh remains in normal mode.
- If errors occur, cssh switches to edit mode and highlights the error line.

## 4.6 Editing and Command Input (Rev.2)
- When edit is executed in normal mode, the cursor moves to the top-left corner.
If the edit buffer contains text, it is displayed; otherwise, the screen is blank.
- The bottom line of the screen is reserved for command input in edit mode.
- To enter command input mode, press ESC.
- Pressing ESC moves the cursor to the bottom prompt (> ).
- Commands are entered after the prompt and executed with Enter.
- If the command is quit, cssh returns to normal mode.
- For other commands, cssh stays in edit mode and restores the cursor to its previous position.
- Pressing ESC again cancels command input and restores the cursor.
- If a command fails, the error message is shown on the bottom line.
Pressing ESC clears the error and restores the prompt and command text.
- After clearing, the cursor moves to the end of the command.

## 4.7 Editing Key Operations
- Basic editing behavior follows simple text editors like Notepad.
- The Insert key has no effect (always insert mode).
- ESC switches to command input mode (cursor moves to the bottom line).

# 5. Shell Operators (Added in v0.2)
## 5.1 Pipelines and Redirection
- | pipeline operator is supported
(stdout of the previous command becomes stdin of the next command)
- Redirection operators are supported:
- > overwrite
- >> append
- < input redirection

## 5.2 Command Execution Rules
- Sequential execution using ; is supported.
- Conditional execution (&&, ||) is not supported in this version.
- Background execution (&) is not supported.

# 6. Windows, macOS, and Linux Behavior
v0.2.x supports Windows only.
Future versions will support macOS, Linux, and all .NET‑compatible environments.

## 6.1 Path Differences
- Windows paths are case‑insensitive (c:\ = C:\).
- cssh accepts both \ and / as path separators.

## 6.2 Paths Containing Spaces
- Paths containing spaces may be enclosed in either " " or ' '.
- The entire path or only the part containing spaces may be quoted.
- Mixed quoting such as "...' is an error.

## 6.3 Environment Variables (v0.m.n Specification)
- Windows environment variables are configured via system settings.
- export is not supported.
- Creating or modifying environment variables is not supported.

# 7. C# Script Details (Rev.3)
## 7.1 Overview of C# Scripts
- Syntax is mostly identical to C#.
- Extension: .csx
- Executed using the dotnet-script command.
- Scripts are compiled to .NET IL before execution.
- No boilerplate (no class or Main method required).
- Many namespaces are imported by default:
System
System.IO
System.Collections.Generic
System.Diagnostics
System.Dynamic
System.Linq
System.Linq.Expressions
System.Text
System.Threading.Tasks

- Standard assemblies available:
System.Runtime.dll
System.Core.dll
System.Data.dll
System.Xml.dll
System.Xml.Linq.dll

-Importing additional packages
Use #r:
```
#r "nuget: System.Text.Json, 8.0.0"
```
using System.Text.Json;

Loading other scripts
```
#load "other_script.csx"
```

## 7.2 cssh Standard Library
cssh provides a custom library to make scripting more convenient.
See STDSCRIPT.md for details.
Library file: cssh.Std.dll
### Contents
Output functions
- print, println, printf, debug
String formatting
- format
Input
- input, gets
Date/time
- today, now, datetime
File operations
- read, write, append, exists
Process execution
- system, run
Control
- exit, abort
Regular expressions
- match, search, replace
Command‑line arguments
- argc, args

# 8. Additional Notes (v0.2.x)
## 8.1 External Commands
- External commands (python, git, dotnet, etc.) can be executed.
- Commands in PATH can be executed without specifying full paths.
- Commands in the current directory require .\ or ./.
- Extensions that may be omitted: .exe, .bat, .cmd, .ps1
- Windows is case‑insensitive.
- Argument parsing follows the command’s own rules.
- stdout and stderr are not distinguished.
- Exit codes are not used.

## 8.2 run Command in Normal and Edit Modes (Rev.2)
- run executes scripts using dotnet-script.
- Only C# script files (.csx) are supported.

## 8.3 Shell Variables
- Shell variables are not supported.
- Assignments like VAR=value are not allowed.

## 8.4 Environment Variables
- $VAR expansion is supported.

## 8.5 Clipboard
- Windows Terminal’s selection, copy, and paste features are supported.

## 9. Roadmap (Rev.2)
v0.2.0
- Shell operators: |, >, >>, <, ;
- Environment variable expansion (echo $PATH)
v0.2.1 (Implemented)
- External command execution
- Dynamic alias system (alias)
- Command history (history, h, !n, !prefix)
- Unit tests added
v0.2.2
- Edit mode implementation
- Clipboard support in edit mode
- cssh standard functions (STDSCRIPT.md)
- run command (C# script execution)
- .csshrc configuration file (Bash’s .bashrc equivalent)
v0.2.3
T.B.D.
v0.2.4
T.B.D.

# 10. For Developers: API Documentation Generation
- CLASSES.md is auto‑generated from XML documentation comments (///).
Script: scripts/generate_classes_md.ps1
Example (PowerShell):
pwsh -NoProfile -NoLogo -File scripts/generate_classes_md.ps1


Output: CLASSES.md at the repository root.

Validation Script
scripts/validate_classes_md.ps1
Example:
pwsh -NoProfile -NoLogo -File scripts/validate_classes_md.ps1


Checks for placeholders like $sig or unnatural summaries.

Helper Scripts
- scripts/add_xml_docs.ps1 — inserts XML docs (review diffs afterward)
- scripts/reindent.ps1 — fixes indentation

CI Recommendation
Integrate generation and validation into CI so that PRs modifying public APIs automatically update CLASSES.md.

