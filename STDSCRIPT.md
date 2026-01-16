# cssh Standard Function Specification (Rev.2)

# 1. Overview
## 1.1 Purpose
-	cssh uses C# scripts as its standard scripting format.
-	However, because C# script syntax is very close to regular C#, it lacks simple, script‑friendly utility functions.
- This document defines the standard functions provided by cssh to address that issue.
(Example)
```
println("Hello World!");
```

## 1.2 Implementation
- 	Create a project  inside the solution .
- 	The source file is .
- 	The compiled DLL will be generated as  under the lowest-level  directory.

1.3 Usage
- Use using static ScriptStd; to access the functions.
- Add the following line inside the <ItemGroup> of your project (.csproj):
```
<ProjectReference Include="..\cssh.Std\cssh.Std.csproj" />
```
# 2. Output Functions
*print(object value)*
- Description: Writes the value to standard output without a newline.
- Return value: none
- Signature: print(object)
- Example: print("hello")
*println(object value)*
- Description: Writes the value to standard output followed by a newline.
- Return value: none
- Signature: println(object)
- Example: println("hello")
*printf(string format, params object[] args)*
- Description: Outputs text using a C‑style format string. Supports %d, %s, %f, and # formats only.
- Return value: none
- Example: printf("name=%s age=%d", name, age)
*debug(object value)*
- Description: Writes debug output to standard error (stderr).
- Return value: none
- Example: debug("x=" + x);

# 3. String Functions
*format(string format, params object[] args)*
- 	Description: Generates and returns a formatted string.
- 	Return value: string
- 	Example:
```
var msg = format("Hello {0}, age {1}", name, age);
println(msg);
```

*merge(string[] arr, string separator = "")*
- 	Description: Concatenates all elements of a string array into a single string.
If  is specified, it is inserted between elements.
- 	Return value: string
- 	Example:
```
string[] arr = ["two", "dogs"];
var merged = merge(arr);
```

*split(string str, string separator = ",")*
- 	Description: Splits the string using the specified separator and returns a string array.
- 	Return value: string[]
- 	Example: var input = "A,B,C"; var arr = split(input);

*index(string input, string str)*
- Description: Returns the index of str within input.
Returns -1 if not found.
- Return value: int
- Example: var p = index("C#;C++", ";");

*substr(string input, int start, int length)*
- Description: Returns a substring of input starting at start with the specified length.
(Wrapper of String.Substring())
- Return value: string
- Example: var s = substr("0123456789", 3, 2);

*startsWith(string input, string str)*
- Description: Returns true if input starts with str. Case‑sensitive.
(Wrapper of String.StartsWith())
- Return value: bool
- Example: var b = startsWith("C#;C++", "C");

*endsWith(string input, string str)*
- Description: Returns true if input ends with str. Case‑sensitive.
(Wrapper of String.EndsWith())
- Return value: bool
- Example: var b = endsWith("C#;C++", "++");

*trim(string input)*
- Description: Returns the string with leading and trailing whitespace removed.
(Wrapper of String.Trim())
- Return value: string
- Example: var str = trim("\tABC\n")

# 4. Input Functions
*input(string prompt)*
- Description: Displays a prompt and reads one line of input from the user.
The prompt is displayed using Console.Write(prompt).
The returned string does not include the trailing newline.
- Return value: string?
- Example: var name = input("name > ");

*gets()*
- Description: Reads one line of input without displaying a prompt.
- Return value: string?
- Example: var line = gets();

# 5. Date and Time Functions
*today()*
- Description: Returns today’s date in "yyyy-MM-dd" format.
- Return value: string
- Example: println(today());

*now()*
- Description: Returns the current date and time in ISO 8601 format.
- Return value: string
- Example: println(now());

*datetime(DateTime? dt, string format)*
- Description: Returns a formatted date/time string.
- Signature:

*string datetime(DateTime? dt = null, string format = "yyyy-MM-ddTHH:mm:ss")*
- Return value: string
- Example:
```
println(datetime(null, "yyyy/MM/dd HH:mm"));
println(datetime(new DateTime(2000, 1, 1), "yyyy-MM-dd"));
```

6. File Functions
*read(string path)*
- Description: Reads the entire contents of a file and returns it.
- Return value: string
- Example: var text = read("data.txt");
*write(string path, string content)*
- Description: Writes content to a file (overwrites existing content).
- Return value: none
- Example: write("log.txt", "hello");
*append(string path, string content)*
- Description: Appends content to a file.
- Return value: none
- Example: append("log.txt", "more");
*exists(string path)*
- Description: Returns true if the file exists.
- Return value: bool
- Example:
```
if (exists("config.json")) { ... }
```

# 7. Process Execution Functions
**ystem(string command)
- Description: Executes an external command and returns its standard output as a string.
(Output is not displayed on screen.)
- Return value: string
- Example: var result = system("ls -l");
*run(string command)*
- Description: Executes an external command and streams its standard output directly to the console.
- Return value: none
- Example: run("echo hello");

# 8. Control Functions
*exit(int code = 0)*
- Description: Terminates the process with the specified exit code.
- Return value: none (does not return)
- Example: exit(1);
*abort(string message = "")*
- Description: Immediately terminates the process.
If message is provided, it is written to standard error before exiting.
- Return value: none
- Example: abort("fatal error");

# 9. Regular Expression Functions
*match(string text, string pattern)*
- Description: Returns true if text matches the regular expression pattern.
- Return value: bool
- Signature: bool match(string text, string pattern)
- Example:
```
if (match(name, "^[A-Z].*")) { ... }
```
*search(string text, string pattern)*
- Description: Returns the first substring of text that matches pattern.
Returns null if no match is found.
- Return value: string?
- Signature: string? search(string text, string pattern)
- Example: var str = search(line, "[0-9]+");
*replace(string text, string pattern, string replacement)*
- Description: Replaces all matches of pattern in text with replacement.
- Return value: string
- Signature: string replace(string text, string pattern, string replacement)
- Example: var s = replace(line, "[0-9]+", "###");

# 10. Command-Line Argument Functions
*argc()*
- Description: Returns the number of command-line arguments.
- Return value: int
- Signature: int argc()
- Example: println(argc());
*args(int index)*
- Description: Returns the command-line argument at the specified index.
Returns null if the index is out of range.
- Return value: string?
- Signature: string? args(int index)
- Example:

