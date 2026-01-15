# cssh Shell v0.2 (Rev.3) Specification

# 1. Overview
Windows provides two major shells: PowerShell and the Command Prompt.
PowerShell is powerful but geared toward advanced users, while the Command Prompt is a relic from the MS‑DOS era—low‑function and outdated.
Therefore, I wanted a shell that is more modern and capable than the Command Prompt, without being as complex as PowerShell.
The cssh v0.x.x series is a prototype version. The name cssh stands for C# Script Shell.
The supported OS is Windows only, with plans to support other .NET‑capable operating systems in the future.

## cssh 0.x System Requirements
- OS: Windows only
- Requires .NET 10.0 or later
(Because .NET 8 will be outdated by the time cssh is completed)
- Launched from Windows Terminal (via PowerShell or CMD.exe)

# 2. Screen Layout
## 2.1 Prompt
The prompt follows a PowerShell‑like style.
Example:
```
cssh: C:\workspace\Cssh>
```

## 2.2 Modes
cssh has two modes: normal mode and edit mode.
- edit enters edit mode
- quit (in edit mode) returns to normal mode
In edit mode, the bottom line of the screen is the command input line, showing the prompt > .

## 2.3 Startup Screen and Window Title
When cssh starts, it enters normal mode, clears the screen, and displays the prompt at the top.
The window title is shown as:
cssh v0.2.n
(n = build version)

# 3. Built‑in Commands (Normal Mode)
The syntax and behavior of built‑in commands follow Bash conventions, though functionality is more limited.
cssh does not support shell variables.
Command behavior and output follow Bash as closely as possible.

## 3.1 ls (alias: dir)
Displays the contents of a directory.
Options:
- -l long format
- -a show hidden files

## 3.2 cd
Changes the current directory.
- With a parameter: move to the specified directory
- Without a parameter: move to the home directory (C:\Users\<user> on Windows)
- Option: - (i.e., cd -) moves to the previous directory

## 3.3 pwd
Displays the current directory.

## 3.4 cat (alias: type)
Displays the contents of a UTF‑8 text file.
Option:
- -n show line numbers

## 3.5 rm (alias: del)
Deletes files or directories.
Options:
- -r recursive
- -f force
Options may be combined (e.g., rm -rf ...).

## 3.6 mkdir
Creates a directory (absolute or relative path).

## 3.7 cp (alias: copy)
Copies a file.
If the destination is a directory, a new file is created under it.
Overwrites existing files; read‑only files can be overwritten with -f.

## 3.8 mv (alias: move, ren)
Moves files or directories.
If the destination already exists, behavior follows Bash.
Option:
- -f force

## 3.9 echo
Displays a string or environment variable.
Strings do not require quotes.

## 3.10 which (alias: where)
Indicates whether a command is built‑in or external.
For external commands, displays the full path.

## 3.11 clear (alias: cls)
Clears the screen.

## 3.12 help
Displays help for commands.
- help → list of available commands
- help <command> → help for that command
Language follows the OS UI language (English or Japanese).

## 3.13 exit (alias: quit)
Exits cssh.

## 3.14 history (alias: h)
- Displays command history (Bash‑style).
- History is per‑session only (not persisted).
- Supports !number to re‑execute a command.

## 3.15 touch
Updates a file’s timestamp or creates an empty file if it does not exist.

##3.16 edit
Enters edit mode.
edit filename: loads the file into the edit buffer.

## 3.17 sudo (v0.2 Rev.2)
- Used when accessing protected disk areas or running privileged commands.
- On Windows, triggers a UAC confirmation dialog.

## 3.18 run (v0.2 Rev.2)
Executes the script currently in the edit buffer.
- Parameters after run are passed to the script
- If a filename is given (run script.csx), the file is loaded into the buffer and executed
- Unsaved buffer content triggers overwrite confirmation
- On script error: switches to edit mode and highlights the error line
Supports Ctrl+C to interrupt execution.

## 3.19 alias (v0.2 Rev.2)
- Bash‑style alias support.
- Example: alias ll='ls -l'
- alias alone shows the alias list.

## 3.20 External Commands (v0.2 Rev.2)
- External commands can be executed normally.
- Privileged commands require sudo.

# 4. Edit Mode (Added in v0.2)
## 4.1 About Edit Mode (Rev.3)
### 4.1.1 Editing and Command Input
- When entering edit mode via edit, the cursor moves to the bottom command line.
- If the edit buffer contains text, it is displayed; otherwise, the screen is empty.
- Commands are entered after the >  prompt.
- quit returns to normal mode; other commands remain in edit mode.
- ESC cancels command input and restores the cursor to its previous position.
- Errors are shown on the last line and cleared with ESC.
- Editing text directly inside the edit area was planned in Rev.2 but removed due to complexity; external editors are used instead.

### 4.1.2 External Editor Integration (Rev.3)
Editor detection order:
- Notepad++
- Notepad
Notepad++ is detected via its standard installation path.
- The edit area only displays the current buffer (no inline editing)
- Scrolling is allowed
- New command: np (launch external editor)
- np saves the buffer to a temp file and opens it in the editor
- After closing the editor, the temp file is loaded into the backup buffer
- User is asked whether to apply the backup buffer to the main buffer
- If accepted, the buffers are swapped
- undo swaps them back once

## 4.2 Edit Mode Built‑in Commands (Rev.3)
### 4.2.1 write (alias: w)
- Saves the buffer to a file.
- Prompts before overwriting unless w! is used.

### 4.2.2 read (alias: r)
- Loads a file into the buffer (overwriting existing content).
- Prompts if unsaved content exists unless r! is used.

### 4.2.3 quit (alias: q)
- Returns to normal mode.
- The buffer is preserved for the next edit session.

### 4.2.4 clear
- Clears the buffer and the display.

### 4.2.5 run
Executes the buffer as a script.
- If loaded from a file, the extension determines whether it is a script
- If unsaved, asks for confirmation
- Only .csx scripts are supported
- Errors highlight the line and show the message

#### 4.2.5.1 Basic Operation
- Executes the contents of the edit buffer as a script.

#### 4.2.5.2 Determining Edit Buffer Contents
- If the contents were loaded from a file, determines whether it is a script based on the file extension and displays an error message if necessary.
- If the edit buffer contains unsaved file data, it cannot be determined whether it is a script or data, so a confirmation message is displayed. (Example) “Execute as script? (y/n)” ← Follows the OS UI language
- (Note) This version only supports C# scripts, so the script file extension is .csx.

#### 4.2.5.3 Error Handling
- If an error occurs in the script, highlight the error line (display an underline).
- Simultaneously display the error message below the error line.
- If execution stalls due to an error, press Ctrl+C to cancel.

#### 4.2.5.4 When Privileges Are Required for Execution
- When accessing protected disk areas or using internal administrator commands, execute with sudo prefixed. (Example) sudo run

#### 4.2.5.5 Displaying and Navigating Execution Results
- Output from command execution is normally displayed on the screen. (Copying the run command itself also displays it on the screen as if the run command were entered.)
- To return to the edit screen, execute only the edit command.

#### 4.2.5.6 run script Behavior
- When specifying run filename.csx, it loads that file into the edit buffer and executes it. If there are unsaved buffers, it prompts for confirmation to overwrite.
- After execution, if there are no errors, it remains in normal mode.
- If there are errors, it transitions to edit mode and highlights the error line (by displaying an underline).

### 4.2.6 np
Launches the external editor.
After editing, loads the result into the backup buffer and asks whether to apply it.

### 4.2.7 undo
Swaps main and backup buffers once.

### 4.2.8 /string
- Searches from the top of the buffer and jumps to the first match.
- Subsequent / repeats the search.
- If not found, a message is shown.

# 5. Shell Operators (Added in v0.2)
## 5.1 Pipeline and Redirection
Supported:
- | pipeline
- > overwrite redirect
- >> append redirect
- < input redirect

## 5.2 Command Execution Rules
- Sequential execution with ;
- && and || not supported
- Background execution & not supported

# 6. Windows, macOS, and Linux Support
v0.2.x supports Windows only.
Future versions will support macOS, Linux, and all .NET‑capable environments.

## 6.1 Path Differences
- Windows paths are case‑insensitive.
- Both \ and / are accepted as separators.

## 6.2 Paths with Spaces
Paths containing spaces may be enclosed in either " " or ' '.
Mixing quotes (e.g., "...') is an error.

## 6.3 Environment Variables
Environment variables are managed by Windows Settings.
export is not supported.
Variables cannot be created or modified.

# 7. C# Script Details (Rev.3)
## 7.1 Overview
- Syntax is essentially C#
- Extension: .csx
- Executed via dotnet-script
- Compiled to IL before execution
- No boilerplate required (no class or Main)
- Many namespaces are imported by default
Default namespaces include:
- System
- System.IO
- System.Collections.Generic
- System.Diagnostics
- System.Dynamic
- System.Linq
- System.Linq.Expressions
- System.Text
- System.Threading.Tasks
Standard assemblies available:
- System.Runtime.dll
- System.Core.dll
- System.Data.dll
- System.Xml.dll
- System.Xml.Linq.dll

External packages can be imported using #r:
```
#r "nuget: System.Text.Json, 8.0.0"
```
Other scripts can be loaded with:
```
#load "other_script.csx"
```

## 7.2 cssh Standard Library
- cssh provides a custom library to simplify scripting.
- File: cssh.Std.dll
Usage:
```
#r "cssh.Std.dll"
```
### Library Contents
- Output functions
print, println, printf, debug
- String functions
format
- Input functions
input, gets
- Date/time functions
today, now, datetime
- File functions
read, write, append, exists
- Process execution
system, run
- Control functions
exit, abort
- Regex functions
match, search, replace
- Command‑line argument functions
argc, args

# 8. Additional Notes (v0.2.x)
## 8.1 External Commands
- Commands in PATH can be executed without full paths
- Commands in the current directory require .\ or ./
- Extensions may be omitted for: .exe, .bat, .cmd, .ps1
- Output and error streams are not distinguished
- Exit codes are not used
## 8.2 run Command Notes
- Executes scripts via dotnet-script
- Only C#‑compatible scripts are supported
## 8.3 Shell Variables
- Not supported.
- Assignments like VAR=value are invalid.

## 8.4 Environment Variables
- Variable expansion ($VAR) is supported.

## 8.5 Clipboard
- Windows Terminal clipboard features (copy/paste) are available.

# 9. Roadmap (Rev.2)
## v0.2.0
- Added operators: |, >, >>, <, ;
- Environment variable expansion (echo $PATH)
## v0.2.1 (Implemented)
- External command execution
- Dynamic alias support
- Command history (history, h, !n, !prefix)
- Added unit tests
## v0.2.2
- Edit mode implementation
- Clipboard support in edit mode
- cssh standard functions
- Script execution (run)
- .csshrc support
## v0.2.3
- T.B.D.
##v0.2.4
- T.B.D.

# 10. For Developers: API Documentation Generation
- CLASSES.md is auto‑generated from XML comments using scripts/generate_classes_md.ps1.
- Validation script: scripts/validate_classes_md.ps1
- Helper scripts:
- add_xml_docs.ps1
- reindent.ps1
- Recommended: integrate generation/validation into CI.

