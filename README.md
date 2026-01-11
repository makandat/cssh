# cssh v0.1 (Rev.2) Specification

# 1. Overview

Windows provides two major shells: PowerShell and the traditional Command Prompt.  
PowerShell is powerful but geared toward advanced users, while Command Prompt is a legacy tool from the MS-DOS era and lacks modern functionality.

cssh aims to fill the gap by providing a shell that is more capable and modern than Command Prompt, without the complexity of PowerShell.

This version of cssh is an experimental prototype that supports only built-in commands.

The supported OS is Windows only, but future versions will support other platforms where .NET is available.

---

# 2. Screen Layout

## 2.1 Prompt
The prompt follows a PowerShell-like style.

Example:
```
cssh: C:\workspace\Cssh>
```

## 2.2 Modes
cssh has two modes: **interactive mode** and **edit mode**.

- The `edit` command enters edit mode.
- The `quit` command (within edit mode) returns to interactive mode.

In **v0.1.0**, edit mode only switches the screen; script editing is not yet supported.  
Only the `quit` command is available in edit mode; all other commands are ignored.

In edit mode, the command input line appears at the bottom of the screen with the prompt:

```
>
```

## 2.3 Startup Screen and Window Title
When cssh starts, it enters interactive mode, clears the screen, and displays the prompt at the top.

The window title displays:

```
cssh v0.1.n
```

(where *n* is the build version)

---

# 3. Built-in Commands (Interactive Mode)

The syntax and behavior of built-in commands follow **Bash conventions**, though functionality is limited compared to Bash.

Shell variables are **not supported** in v0.1.x.

Command behavior and output follow Bash as closely as possible.

---

## 3.1 `ls` (Alias: `dir`)
Displays the contents of a directory.

Options:
- `-l` : long format  
- `-a` : show hidden files

## 3.2 `cd`
Changes the current directory.

Options:
- `-` : equivalent to `cd -`, returns to the previous directory.

## 3.3 `pwd`
Displays the current directory.  
No options.

## 3.4 `cat` (Alias: `type`)
Displays the contents of a text file (UTF‑8 only).

Options:
- `-n` : show line numbers

## 3.5 `rm` (Alias: `del`)
Deletes files or directories.

Options:
- `-r` : recursive  
- `-f` : force  

Options may be combined (e.g., `rm -rf ...`).

## 3.6 `mkdir`
Creates a directory.  
Accepts both absolute and relative paths.

## 3.7 `cp` (Alias: `copy`)
Copies a file.

- If the destination is a directory, the file is copied into it.  
- If a file with the same name exists, it is overwritten.  
- If the destination file is read-only, use `-f` to force overwrite.

## 3.8 `mv` (Alias: `move`, `ren`)
Moves files or directories.

Options:
- `-f` : force

Behavior follows Bash when the destination already exists.

## 3.9 `echo`
Displays a string.  
Quotation marks are not required.

## 3.10 `which` (Alias: `where`)
Indicates whether a command is a built-in command.

External commands are not supported in v0.1, so the result is always “builtin”.

## 3.11 `clear` (Alias: `cls`)
Clears the screen.

## 3.12 `help`
Displays help for commands.

- `help` : shows a list of available commands  
- `help <command>` : shows help for the specified command  
- Display language is English on English Windows, Japanese on Japanese Windows  
  (other languages are not supported)

## 3.13 `exit` (Alias: `quit`)
Exits cssh.

## 3.14 `history` (Alias: `h`)
Displays the command history.

- Behavior follows Bash  
- The `!` operator is not implemented in v0.1.x  
- History is session‑local only

## 3.15 `touch`
Updates a file’s timestamp to the current time.

## 3.16 `edit`
Enters script edit mode.

---

# 4. Built-in Commands (Edit Mode)

In v0.1.x, **only the `quit` command is supported**.  
All other input is ignored.

---

# 5. Platform Considerations (Windows, macOS, Linux)

v0.1.x supports **Windows only**, but future versions will support macOS, Linux, and any platform where .NET runs.

## 5.1 Path Differences
- Windows paths are case-insensitive (`c:\` and `C:\` are equivalent).  
- cssh accepts both `\` and `/` as path separators.

## Paths with Spaces
Paths containing spaces may be enclosed in either `"` or `'`.  
You may enclose the entire path or only the parts containing spaces.

Mixed quoting such as `"...'` is invalid.

## Environment Variables
- Windows environment variables are configured through system settings.  
- The `export` command is not supported.  
- Creating or modifying environment variables is not supported.

---

# 6. Additional Notes (v0.1.x)

## 6.1 Pipelines and Redirection
The following features will be introduced gradually in v0.2 and later:

- `|` (pipeline)  
- `>`, `>>`, `<` (redirection)  
- `>>` (value pipe), `>!` (error pipe)

## 6.2 Command Execution Rules
- Only one command per line  
- Command chaining with `;` is not supported  
- Conditional execution (`&&`, `||`) is not supported  

## 6.3 External Commands
External commands (e.g., `python`, `git`, `dotnet`) cannot be executed.  
Support will be added in v0.2.x along with the `run` command.

## 6.4 Shell Variables
Shell variables are not supported.  
Assignments like `VAR=value` are not allowed.

## 6.5 Environment Variables
Environment variable expansion (`%VAR%`, `$VAR`) is not supported.  
May be considered for future versions.
