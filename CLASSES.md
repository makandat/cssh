# Classes

## AliasAndHistoryTests
Represents the AliasAndHistoryTests class.

**Source:** D:\workspace\Cssh\cssh.Tests\AliasAndHistoryTests.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Field** `private readonly CommandRunner _runner;` — The _runner field.
- **Field** `private readonly ShellState _state;` — The _state field.
- **Field** `private readonly string _testDir;` — The _testDir field.
- **Constructor** `public AliasAndHistoryTests()` — 
- **Method** `public void Alias_CanCreateAndInvoke()` — Executes the Alias_CanCreateAndInvoke method.
- **Method** `public void Alias_WithQuotedExpansion_PreservesSpaces()` — Executes the Alias_WithQuotedExpansion_PreservesSpaces method.
- **Method** `public void Alias_List_ShowsDefined()` — Executes the Alias_List_ShowsDefined method.
- **Method** `public void Unalias_IsNotRegistered()` — Executes the Unalias_IsNotRegistered method.
- **Method** `public void History_RecordsAndLists()` — Executes the History_RecordsAndLists method.
- **Method** `public void Bang_ExecutesHistoryEntry()` — Executes the Bang_ExecutesHistoryEntry method.
- **Method** `public void BangPrefix_ExecutesMostRecentMatchingHistoryEntry()` — Executes the BangPrefix_ExecutesMostRecentMatchingHistoryEntry method.
- **Method** `public void BangPrefix_NotFoundReturnsError()` — Executes the BangPrefix_NotFoundReturnsError method.
- **Method** `public void Dispose()` — Executes the Dispose method.

## ClearCommand
Represents the ClearCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\ClearCommand.cs

### Members

- **Field** `public string Name => "clear";` — The Name field.
- **Field** `public string Description => "Clear the screen.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## CommandParserTests
Tests for CommandParser covering: - Basic command parsing - Quoted arguments (single and double quotes) - Pipeline operators (|) - Redirection operators (>, >>, <) - Multiple pipelines and redirects Per specification section 5: "Shell operators"

**Source:** D:\workspace\Cssh\cssh.Tests\CommandParserTests.cs

### Members

- **Field** `private readonly CommandParser _parser = new();` — The _parser field.
- **Method** `public void Parse_ShouldReturnCommandName_WhenOnlyCommandGiven()` — Executes the Parse_ShouldReturnCommandName_WhenOnlyCommandGiven method.
- **Method** `public void Parse_ShouldSplitCommandAndArguments()` — Executes the Parse_ShouldSplitCommandAndArguments method.
- **Method** `public void Parse_ShouldHandleMultipleArguments()` — Executes the Parse_ShouldHandleMultipleArguments method.
- **Method** `public void Parse_ShouldHandleDoubleQuotedArguments_WithSpaces()` — Executes the Parse_ShouldHandleDoubleQuotedArguments_WithSpaces method.
- **Method** `public void Parse_ShouldHandleSingleQuotedArguments_WithSpaces()` — Executes the Parse_ShouldHandleSingleQuotedArguments_WithSpaces method.
- **Method** `public void Parse_ShouldHandleMixedQuotes()` — Executes the Parse_ShouldHandleMixedQuotes method.
- **Method** `public void Parse_ShouldHandleQuotedPathsWithBackslash()` — Executes the Parse_ShouldHandleQuotedPathsWithBackslash method.
- **Method** `public void Parse_ShouldHandleQuotedPathsWithForwardSlash()` — Executes the Parse_ShouldHandleQuotedPathsWithForwardSlash method.
- **Method** `public void ParseSequence_ShouldParseOutputRedirect_Truncate()` — Executes the ParseSequence_ShouldParseOutputRedirect_Truncate method.
- **Method** `public void ParseSequence_ShouldParseOutputRedirect_Append()` — Executes the ParseSequence_ShouldParseOutputRedirect_Append method.
- **Method** `public void ParseSequence_ShouldParseInputRedirect()` — Executes the ParseSequence_ShouldParseInputRedirect method.
- **Method** `public void ParseSequence_ShouldParseRedirectWithQuotedPath()` — Executes the ParseSequence_ShouldParseRedirectWithQuotedPath method.
- **Method** `public void ParseSequence_ShouldParseTwoCommandPipeline()` — Executes the ParseSequence_ShouldParseTwoCommandPipeline method.
- **Method** `public void ParseSequence_ShouldHaveEmptyArgsOnSecondCommandForEchoPipe()` — Executes the ParseSequence_ShouldHaveEmptyArgsOnSecondCommandForEchoPipe method.
- **Method** `public void ParseSequence_ShouldParseMultipleCommandPipeline()` — Executes the ParseSequence_ShouldParseMultipleCommandPipeline method.
- **Method** `public void ParseSequence_ShouldParsePipelineWithArguments()` — Executes the ParseSequence_ShouldParsePipelineWithArguments method.
- **Method** `public void ParseSequence_ShouldParsePipelineWithFinalRedirect()` — Executes the ParseSequence_ShouldParsePipelineWithFinalRedirect method.
- **Method** `public void ParseSequence_ShouldSplitBySemicolon()` — Executes the ParseSequence_ShouldSplitBySemicolon method.
- **Method** `public void ParseSequence_ShouldNotSplitSemicolonInsideQuotes()` — Executes the ParseSequence_ShouldNotSplitSemicolonInsideQuotes method.
- **Method** `public void ParseSequence_ShouldHandleCommandWithMultipleArguments_AndRedirect()` — Executes the ParseSequence_ShouldHandleCommandWithMultipleArguments_AndRedirect method.
- **Method** `public void ParseSequence_ShouldPreservePathsWithSpaces()` — Executes the ParseSequence_ShouldPreservePathsWithSpaces method.
- **Method** `public void ParseSequence_ShouldHandleWindowsPaths()` — Executes the ParseSequence_ShouldHandleWindowsPaths method.
- **Method** `public void ParseSequence_ShouldHandleUnixStylePaths()` — Executes the ParseSequence_ShouldHandleUnixStylePaths method.
- **Method** `public void ParseSequence_ShouldHandleOptionsWithDash()` — Executes the ParseSequence_ShouldHandleOptionsWithDash method.
- **Method** `public void ParseSequence_ShouldHandleEmptyInput()` — Executes the ParseSequence_ShouldHandleEmptyInput method.
- **Method** `public void ParseSequence_ShouldHandleWhitespaceOnlyInput()` — Executes the ParseSequence_ShouldHandleWhitespaceOnlyInput method.
- **Method** `public void Parse_ShouldReturnEmptyCommand_WhenInputIsEmpty()` — Executes the Parse_ShouldReturnEmptyCommand_WhenInputIsEmpty method.
- **Method** `public void ParseSequence_ShouldHandleConsecutiveSpaces()` — Executes the ParseSequence_ShouldHandleConsecutiveSpaces method.

## CommandRunnerTests
Integration tests for CommandRunner covering: - Pipeline execution - Redirect execution (>, >>, <) - Error handling - Command execution with state Per specification section 5: "Shell operators and execution rules"

**Source:** D:\workspace\Cssh\cssh.Tests\CommandRunnerTests.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Field** `private readonly CommandRunner _runner;` — The _runner field.
- **Field** `private readonly ShellState _state;` — The _state field.
- **Field** `private readonly string _testDir;` — The _testDir field.
- **Constructor** `public CommandRunnerTests()` — 
- **Method** `public void Run_Echo_ShouldReturnText()` — Executes the Run_Echo_ShouldReturnText method.
- **Method** `public void Run_Echo_ShouldHandleQuotedText()` — Executes the Run_Echo_ShouldHandleQuotedText method.
- **Method** `public void Run_Echo_ShouldExpandEnvironmentVariable()` — Executes the Run_Echo_ShouldExpandEnvironmentVariable method.
- **Method** `public void Run_Echo_ShouldNotExpandInSingleQuotes()` — Executes the Run_Echo_ShouldNotExpandInSingleQuotes method.
- **Method** `public void Run_Echo_ShouldExpandInDoubleQuotes()` — Executes the Run_Echo_ShouldExpandInDoubleQuotes method.
- **Method** `public void Run_Echo_ShouldExpandInlineVariables()` — Executes the Run_Echo_ShouldExpandInlineVariables method.
- **Method** `public void Run_Echo_WithOutputRedirectTruncate_ShouldCreateFile()` — Executes the Run_Echo_WithOutputRedirectTruncate_ShouldCreateFile method.
- **Method** `public void Run_Echo_WithOutputRedirectAppend_ShouldAppendToFile()` — Executes the Run_Echo_WithOutputRedirectAppend_ShouldAppendToFile method.
- **Method** `public void Run_Echo_WithMultipleRedirects_ShouldUseLastRedirect()` — Executes the Run_Echo_WithMultipleRedirects_ShouldUseLastRedirect method.
- **Method** `public void Run_Cat_WithInputRedirect_ShouldReadFromFile()` — Executes the Run_Cat_WithInputRedirect_ShouldReadFromFile method.
- **Method** `public void Run_WithInputRedirect_NonExistentFile_ShouldReturnError()` — Executes the Run_WithInputRedirect_NonExistentFile_ShouldReturnError method.
- **Method** `public void Run_Pipeline_EchoToCat_ShouldTransmitData()` — Executes the Run_Pipeline_EchoToCat_ShouldTransmitData method.
- **Method** `public void Run_Pipeline_MultipleCommands_ShouldChain()` — Executes the Run_Pipeline_MultipleCommands_ShouldChain method.
- **Method** `public void Run_Pipeline_WithFinalRedirect_ShouldRedirectPipeOutput()` — Executes the Run_Pipeline_WithFinalRedirect_ShouldRedirectPipeOutput method.
- **Method** `public void Run_Semicolon_Commands_ShouldExecuteSequentially()` — Executes the Run_Semicolon_Commands_ShouldExecuteSequentially method.
- **Method** `public void Run_UnknownCommand_ShouldReturnError()` — Executes the Run_UnknownCommand_ShouldReturnError method.
- **Method** `public void Run_Touch_ShouldCreateFile()` — Executes the Run_Touch_ShouldCreateFile method.
- **Method** `public void Run_Mkdir_ShouldCreateDirectory()` — Executes the Run_Mkdir_ShouldCreateDirectory method.
- **Method** `public void Run_ExternalCommand_FromPath_ShouldExecute()` — Executes the Run_ExternalCommand_FromPath_ShouldExecute method.
- **Method** `public void Run_CommandWithMultipleArgs_AndRedirect()` — Executes the Run_CommandWithMultipleArgs_AndRedirect method.
- **Method** `public void Run_EmptyInput_ShouldHandleGracefully()` — Executes the Run_EmptyInput_ShouldHandleGracefully method.
- **Method** `public void Run_WhitespaceOnlyInput_ShouldHandleGracefully()` — Executes the Run_WhitespaceOnlyInput_ShouldHandleGracefully method.
- **Method** `public void Dispose()` — Executes the Dispose method.

## cssh.Core;.CommandParser
Parses raw input into AST (Sequence of Pipelines with Redirects). Supports quoted arguments using both "..." and '...' and operators (>, >>, <, |).

**Source:** D:\workspace\Cssh\cssh.Core\CommandParser.cs

### Members

- **Method** `public Sequence ParseSequence(string input)` — Parses input into a Sequence (for pipeline execution). Supports multiple command groups separated by ';' (outside quotes).
- **Method** `private List<CommandNode> ParsePipeline(string input)` — Parses a single pipeline (one or more commands separated by |).
- **Method** `private List<string> TokenizeWithRedirects(string input)` — Tokenizes input while preserving redirect operators and quoted strings.
- **Method** `private List<string> SplitByPipes(string input)` — Splits input by pipes (outside quotes).
- **Method** `private List<string> SplitBySemicolons(string input)` — Splits input by semicolons (outside quotes).
- **Method** `private bool IsRedirectOperator(string token)` — 
- **Method** `private bool IsRedirectChar(char c)` — 
- **Method** `public ParsedCommand Parse(string input)` — Legacy Parse method for backward compatibility.

## cssh.Core;.CommandRegistry
Represents the CommandRegistry class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\CommandRegistry.cs

### Members

- **Field** `private readonly Dictionary<string, ICommand> _commands = new();` — The _commands field.
- **Field** `private readonly Dictionary<string, string> _descriptions = new();` — The _descriptions field.
- **Field** `private readonly Dictionary<string, string> _dynamicAliases = new();` — The _dynamicAliases field.
- **Method** `public void Register(ICommand command, string description)` — コマンドを登録する（説明文付き）
- **Method** `public IEnumerable<string> GetAllCommandNames()` — コマンド名から ICommand を取得する コマンドの説明文を取得する 登録されているすべてのコマンド名を返す
- **Method** `public void AddAlias(string name, string expansion)` — 動的 alias を追加する
- **Method** `public bool RemoveAlias(string name)` — 動的 alias を削除する
- **Method** `public IReadOnlyDictionary<string,string> GetAliases()` — 現在の alias を取得する

## cssh.Core;.CommandRunner
Executes commands by coordinating the parser, registry, and shell state. Supports pipelines (|) and redirects (>, >>, <).

**Source:** D:\workspace\Cssh\cssh.Core\CommandRunner.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Constructor** `public CommandRunner(CommandParser parser, CommandRegistry registry)` — Initializes a new instance of the <see cref="CommandRunner"/> class.
- **Method** `public string Run(ShellState state, string input)` — Parses and executes a command string with support for pipelines and redirects.
- **Method** `private string HandleEditModeInput(ShellState state, string input)` — Handles inputs while in edit mode. Supported editor commands: - read|r <file> : load file into buffer - write|w [file] : write buffer to file (optional file to save as) - run [file] : run buffer as simple script (supports only lines starting with 'echo ') - quit|q : exit edit mode Any other input is appended as a line to the edit buffer.
- **Method** `private string ExecutePipeline(ShellState state, Pipeline pipeline)` — Executes a pipeline of one or more commands.
- **Method** `private string ExecuteCommandWithInputRedirect(ShellState state, CommandNode cmd)` — Executes a command (first in pipeline or standalone) with input redirect support.
- **Method** `private string EscapeArgs(string[] args)` — Attempts to execute an external command using PATH lookup and Process. Returns the command output, or null if command not found/run failed.
- **Field** `private const string StdinPrefix = "\u0001";` — Executes a piped command (receives input from previous command).
- **Method** `private string ExecutePipedCommand(ShellState state, CommandNode cmd, string pipeInput)` — 
- **Method** `private string ExecuteCommandWithRedirect(ShellState state, CommandNode cmd)` — Executes a command with output redirect (>, >>).
- **Method** `private void HandleRedirect(RedirectInfo redirect, string output)` — Handles output redirection (>, >>).

## cssh.Core;.ICommand
Represents the ICommand interface.

**Source:** D:\workspace\Cssh\cssh.Core\ICommand.cs

_No documented members_

## cssh.Core;.PathNormalizer
Provides utilities for normalizing paths so that both '/' and '\' can be used as directory separators.

**Source:** D:\workspace\Cssh\cssh.Core\PathNormalizer.cs

### Members

- **Method** `public static string Normalize(string path)` — Normalizes a path by converting '/' to '\' on Windows.

## cssh.Core;.ShellMode
Represents the ShellMode enum.

**Source:** D:\workspace\Cssh\cssh.Core\ShellMode.cs

_No documented members_

## cssh.Core;.ShellState
Represents the ShellState class.

**Source:** D:\workspace\Cssh\cssh.Core\ShellState.cs

### Members

- **Field** `private string _currentDirectory = Directory.GetCurrentDirectory();` — The _currentDirectory field.
- **Field** `private string _previousDirectory = Directory.GetCurrentDirectory();` — The _previousDirectory field.
- **Property** `public ShellMode Mode { get; set; } = ShellMode.Normal;` — Gets or sets the Mode.
- **Property** `public List<string> History { get; } = new List<string>();` — Gets or sets the History.
- **Property** `public string EditBuffer { get; set; } = string.Empty;` — Gets or sets the EditBuffer.
- **Property** `public string EditFileName { get; set; } = string.Empty;` — Gets or sets the EditFileName.
- **Property** `public bool EditDirty { get; set; } = false;` — Gets or sets the EditDirty.
- **Property** `public CommandRegistry Registry { get; }` — Gets or sets the Registry.
- **Constructor** `public ShellState(CommandRegistry registry)` — 

## Cssh.Core.Ast;.CommandNode
Represents the CommandNode class.

**Source:** D:\workspace\Cssh\cssh.Core\AST.cs

### Members

- **Property** `public string Name { get; }` — Gets or sets the Name.
- **Property** `public IReadOnlyList<string> Args { get; }` — Gets or sets the Args.
- **Property** `public RedirectInfo Redirect { get; }` — Gets or sets the Redirect.
- **Constructor** `public CommandNode(string name, IReadOnlyList<string> args, RedirectInfo redirect)` — 

## Cssh.Core.Ast;.Pipeline
Represents the Pipeline class.

**Source:** D:\workspace\Cssh\cssh.Core\AST.cs

### Members

- **Property** `public IReadOnlyList<CommandNode> Commands { get; }` — Gets or sets the Commands.
- **Constructor** `public Pipeline(IReadOnlyList<CommandNode> commands)` — 

## Cssh.Core.Ast;.RedirectInfo
Represents the RedirectInfo class.

**Source:** D:\workspace\Cssh\cssh.Core\AST.cs

### Members

- **Property** `public RedirectType Type { get; }` — Gets or sets the Type.
- **Constructor** `public RedirectInfo(RedirectType type, string? filePath)` — 
- **Method** `public static RedirectInfo None() => new(RedirectType.None, null);` — 

## Cssh.Core.Ast;.RedirectType
Represents the RedirectType enum.

**Source:** D:\workspace\Cssh\cssh.Core\AST.cs

_No documented members_

## Cssh.Core.Ast;.Sequence
Represents the Sequence class.

**Source:** D:\workspace\Cssh\cssh.Core\AST.cs

### Members

- **Property** `public IReadOnlyList<Pipeline> Pipelines { get; }` — Gets or sets the Pipelines.
- **Constructor** `public Sequence(IReadOnlyList<Pipeline> pipelines)` — 

## cssh.Core.Commands;.AliasBuiltinCommand
Builtin command 'alias' Usage: - alias -> list aliases - alias name expr -> set alias 'name' to expansion 'expr' (expr may contain spaces)

**Source:** D:\workspace\Cssh\cssh.Core\Commands\AliasBuiltinCommand.cs

### Members

- **Field** `public string Name => "alias";` — The Name field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.AliasCommand
Represents the AliasCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\AliasCommands.cs

### Members

- **Field** `private readonly string _target;` — The _target field.
- **Constructor** `public AliasCommand(string name, string target)` — 
- **Property** `public string Name { get; }` — Gets or sets the Name.
- **Field** `public string Description => $"Alias for '{_target}'.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.CatCommand
Outputs the contents of a file.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\CatCommand.cs

### Members

- **Field** `public string Name => "cat";` — The Name field.
- **Field** `public string Description => "Print the contents of a file.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.CdCommand
Implements the <c>cd</c> command, changing the current working directory. Follows Bash behavior for basic directory navigation.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\CdCommand.cs

### Members

- **Field** `public string Name => "cd";` — 
- **Method** `public string Execute(ShellState state, string[] args)` — Changes the current working directory. Supports: cd, cd -, cd .., cd <path>.

## cssh.Core.Commands;.DynamicAliasInvoker
Invokes a dynamic alias. The expansion string is parsed using the shell parser so quoting and env expansion behave consistently with normal input. The alias expansion is executed as: <expansion-command> + appended args

**Source:** D:\workspace\Cssh\cssh.Core\Commands\DynamicAliasInvoker.cs

### Members

- **Field** `private readonly string _name;` — The _name field.
- **Field** `private readonly string _expansion;` — The _expansion field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Constructor** `public DynamicAliasInvoker(string name, string expansion, CommandRegistry registry)` — 
- **Field** `public string Name => _name;` — The Name field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.EchoCommand
Outputs the given arguments as a single line of text. No variable expansion or special processing is performed.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\EchoCommand.cs

### Members

- **Field** `public string Name => "echo";` — The Name field.
- **Field** `public string Description => "Print the given arguments.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.EditCommand
Represents the EditCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\EditCommand.cs

### Members

- **Field** `public string Name => "edit";` — The Name field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.HelpCommand
Represents the HelpCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\HelpCommand.cs

### Members

- **Field** `public string Name => "help";` — The Name field.
- **Method** `private bool IsJapanese()` — 
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.HistoryCommand
Builtin command 'history' (alias 'h')

**Source:** D:\workspace\Cssh\cssh.Core\Commands\HistoryCommand.cs

### Members

- **Field** `public string Name => "history";` — The Name field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.LsCommand
Implements the <c>ls</c> command with Bash-like behavior, supporting <c>-a</c> and <c>-l</c> options.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\LsCommand.cs

### Members

- **Field** `public string Name => "ls";` — The Name field.
- **Method** `public string Execute(ShellState state, string[] args)` — 
- **Method** `private static string FormatShort(IEnumerable<string> entries)` — 
- **Method** `private static string FormatLong(IEnumerable<string> entries)` — 
- **Method** `private static string FormatEntry(string path)` — 
- **Method** `private static string GetPseudoPermissions(string path)` — 
- **Method** `private static bool IsExecutable(string path)` — 
- **Method** `private static bool IsSymlink(string path)` — 

## cssh.Core.Commands;.MkdirCommand
Represents the MkdirCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\MkdirCommand.cs

### Members

- **Field** `public string Name => "mkdir";` — The Name field.
- **Field** `public string Description => "Create a directory.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.PwdCommand
Implements the <c>pwd</c> command, printing the current working directory.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\PwdCommand.cs

### Members

- **Field** `public string Name => "pwd";` — 
- **Method** `public string Execute(ShellState state, string[] args)` — Returns the current working directory, following Bash behavior.

## cssh.Core.Commands;.RmCommand
Represents the RmCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\RmCommand.cs

### Members

- **Field** `public string Name => "rm";` — The Name field.
- **Field** `public string Description => "Remove a file.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.RmdirCommand
Represents the RmdirCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\RmdirCommand.cs

### Members

- **Field** `public string Name => "rmdir";` — The Name field.
- **Field** `public string Description => "Remove an empty directory.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.TouchCommand
Creates an empty file if it does not exist. If the file exists, updates its last write time.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\TouchCommand.cs

### Members

- **Field** `public string Name => "touch";` — The Name field.
- **Field** `public string Description => "Create a file or update its modification time.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Commands;.WhichCommand
Represents the WhichCommand class.

**Source:** D:\workspace\Cssh\cssh.Core\Commands\WhichCommand.cs

### Members

- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Constructor** `public WhichCommand(CommandRegistry registry)` — 
- **Field** `public string Name => "which";` — The Name field.
- **Field** `public string Description => "Show the implementation of a command.";` — The Description field.
- **Method** `public string Execute(ShellState state, string[] args)` — 

## cssh.Core.Constants;.CsshConstants
Provides constant values used throughout the cssh shell.

**Source:** D:\workspace\Cssh\cssh.Core\cssh.Constants.cs

### Members

- **Field** `public const string Version = "v0.2.1";` — Shell version string.
- **Field** `public const int Revision = 1;` — The Revision field.
- **Field** `public const int DefaultPromptDirTrim = 2;` — Default directory trim length for prompt display.
- **Field** `public const string ModeInteractive = "interactive";` — Interactive mode identifier.
- **Field** `public const string ModeEdit = "edit";` — Edit mode identifier.
- **Field** `public const string TitlePrefix = "cssh ";` — Prefix used for shell title display.
- **Field** `public const string CmdLs = "ls";` — 
- **Field** `public const string CmdCd = "cd";` — 
- **Field** `public const string CmdPwd = "pwd";` — 

## cssh.Core.Execution;.CommandExecutor
Executes parsed AST (Sequence / Pipeline / CommandNode) using the existing ICommand (string-based) command model.

**Source:** D:\workspace\Cssh\cssh.Core\Execution\CommandExecuter.cs

### Members

- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Constructor** `public CommandExecutor(CommandRegistry registry)` — 
- **Method** `public string ExecuteSequence(ShellState state, Sequence sequence)` — Executes a sequence of pipelines separated by ';'. Returns the output of the last pipeline (for REPL display).
- **Method** `private string ExecutePipeline(ShellState state, Pipeline pipeline)` — Executes a single pipeline, e.g. cmd1 | cmd2 | cmd3. The output of each command becomes the "piped input" for the next one.
- **Method** `private static string ApplyOutputRedirects(string output, RedirectInfo? redirect)` — Executes a single command node with optional redirects and piped input. Builds the argument array for a command, combining: - original parsed arguments - piped input (if present) - input redirection content (&lt; file) if configured NOTE: この挙動は「最小構成」の暫定方針です。 - 元の args をそのまま維持 - pipedInput があれば末尾に追加 - &lt; file があればさらに末尾に追加 実運用で不自然に感じたら、ここを差し替えれば済みます（他の構造には影響しません）。 Applies output redirects (&gt;, &gt;&gt;) if configured. When redirected, the text is written to file and an empty string is returned so that REPL does not duplicate the output.

## cssh.Tests;.CsshConstantsTests
Unit tests for <see cref="CsshConstants"/> ensuring that versioning and constant values remain stable across releases.

**Source:** D:\workspace\Cssh\cssh.Tests\csshConstantsTests.cs

### Members

- **Method** `public void Version_ShouldBe_v021()` — Ensures that the shell version matches v0.2.1.
- **Method** `public void DefaultPromptDirTrim_ShouldBe_2()` — Ensures that the default prompt directory trim length is 2.
- **Method** `public void ModeInteractive_ShouldBe_interactive()` — Ensures that the interactive mode identifier is correct.
- **Method** `public void ModeEdit_ShouldBe_edit()` — Ensures that the edit mode identifier is correct.

## EditModeRunCsxTests
Represents the EditModeRunCsxTests class.

**Source:** D:\workspace\Cssh\cssh.Tests\EditModeRunCsxTests.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Field** `private readonly CommandRunner _runner;` — The _runner field.
- **Field** `private readonly ShellState _state;` — The _state field.
- **Field** `private readonly string _testDir;` — The _testDir field.
- **Constructor** `public EditModeRunCsxTests()` — 
- **Method** `public void Run_Csx_ReturnsConsoleWriteLineOutput()` — Executes the Run_Csx_ReturnsConsoleWriteLineOutput method.
- **Method** `public void Run_Csx_CompilationErrorReportsLine()` — Executes the Run_Csx_CompilationErrorReportsLine method.
- **Method** `public void Dispose()` — Executes the Dispose method.

## EditModeTests
Represents the EditModeTests class.

**Source:** D:\workspace\Cssh\cssh.Tests\EditModeTests.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Field** `private readonly CommandRunner _runner;` — The _runner field.
- **Field** `private readonly ShellState _state;` — The _state field.
- **Field** `private readonly string _testDir;` — The _testDir field.
- **Constructor** `public EditModeTests()` — 
- **Method** `public void EditCommand_EntersEditMode()` — Executes the EditCommand_EntersEditMode method.
- **Method** `public void ReadCommand_LoadsFileIntoBuffer()` — Executes the ReadCommand_LoadsFileIntoBuffer method.
- **Method** `public void AppendLines_AppendsToBufferAndMarksDirty()` — Executes the AppendLines_AppendsToBufferAndMarksDirty method.
- **Method** `public void ClearCommand_ClearsBufferAndResetsDirty()` — Executes the ClearCommand_ClearsBufferAndResetsDirty method.
- **Method** `public void WriteCommand_SavesBufferToFile()` — Executes the WriteCommand_SavesBufferToFile method.
- **Method** `public void WriteCommand_UsesExistingFileName_WhenNoArgProvided()` — Executes the WriteCommand_UsesExistingFileName_WhenNoArgProvided method.
- **Method** `public void RunCommand_ExecutesEchoLines()` — Executes the RunCommand_ExecutesEchoLines method.
- **Method** `public void Quit_ExitsEditMode()` — Executes the Quit_ExitsEditMode method.
- **Method** `public void Dispose()` — Executes the Dispose method.

## LsCommandTests
Represents the LsCommandTests class.

**Source:** D:\workspace\Cssh\cssh.Tests\LsCommandTests.cs

### Members

- **Method** `public void Ls_ShouldListFilesAndDirectories_InCurrentDirectory()` — Executes the Ls_ShouldListFilesAndDirectories_InCurrentDirectory method.

## PathAndQuotingTests
Tests for path handling and quoting per specification sections 6.1, 6.2 - Windows path support (\ and /) - Paths with spaces - Single and double quotes - Mixed quote styles

**Source:** D:\workspace\Cssh\cssh.Tests\PathAndQuotingTests.cs

### Members

- **Field** `private readonly CommandParser _parser = new();` — The _parser field.
- **Method** `public void Path_WindowsStyle_ShouldAcceptBackslash()` — Executes the Path_WindowsStyle_ShouldAcceptBackslash method.
- **Method** `public void Path_UnixStyle_ShouldAcceptForwardSlash()` — Executes the Path_UnixStyle_ShouldAcceptForwardSlash method.
- **Method** `public void Path_MixedStyle_ShouldAcceptMixedSlashes()` — Executes the Path_MixedStyle_ShouldAcceptMixedSlashes method.
- **Method** `public void Path_DriveLetterLowercase_ShouldBePreserved()` — Executes the Path_DriveLetterLowercase_ShouldBePreserved method.
- **Method** `public void Path_DriveLetterUppercase_ShouldBePreserved()` — Executes the Path_DriveLetterUppercase_ShouldBePreserved method.
- **Method** `public void QuotedPath_DoubleQuotes_ShouldPreserveSpaces()` — Executes the QuotedPath_DoubleQuotes_ShouldPreserveSpaces method.
- **Method** `public void QuotedPath_SingleQuotes_ShouldPreserveSpaces()` — Executes the QuotedPath_SingleQuotes_ShouldPreserveSpaces method.
- **Method** `public void QuotedPath_PartialQuote_SpaceInMiddle()` — Executes the QuotedPath_PartialQuote_SpaceInMiddle method.
- **Method** `public void QuotedPath_OutputRedirect_WithSpaces()` — Executes the QuotedPath_OutputRedirect_WithSpaces method.
- **Method** `public void QuotedPath_InputRedirect_WithSpaces()` — Executes the QuotedPath_InputRedirect_WithSpaces method.
- **Method** `public void Quotes_DoubleQuoted_StringWithSpaces()` — Executes the Quotes_DoubleQuoted_StringWithSpaces method.
- **Method** `public void Quotes_SingleQuoted_StringWithSpaces()` — Executes the Quotes_SingleQuoted_StringWithSpaces method.
- **Method** `public void Quotes_MixedQuotes_InSameCommand()` — Executes the Quotes_MixedQuotes_InSameCommand method.
- **Method** `public void Quotes_NestedOppositeQuotes_ShouldWork()` — Executes the Quotes_NestedOppositeQuotes_ShouldWork method.
- **Method** `public void Quotes_DoubleQuoteInsideSingle()` — Executes the Quotes_DoubleQuoteInsideSingle method.
- **Method** `public void Path_WithDots_ShouldBePreserved()` — Executes the Path_WithDots_ShouldBePreserved method.
- **Method** `public void Path_WithDots_RelativePath()` — Executes the Path_WithDots_RelativePath method.
- **Method** `public void Path_WithHome_RelativeToHome()` — Executes the Path_WithHome_RelativeToHome method.
- **Method** `public void Path_UNCPath_Windows()` — Executes the Path_UNCPath_Windows method.
- **Method** `public void Path_LongPath_WindowsStyle()` — Executes the Path_LongPath_WindowsStyle method.
- **Method** `public void Spec_QuoteMismatch_DoubleInside()` — Executes the Spec_QuoteMismatch_DoubleInside method.
- **Method** `public void Redirect_OutputPath_WithBackslash()` — Executes the Redirect_OutputPath_WithBackslash method.
- **Method** `public void Redirect_OutputPath_WithForwardSlash()` — Executes the Redirect_OutputPath_WithForwardSlash method.
- **Method** `public void Redirect_RelativePath_DotSlash()` — Executes the Redirect_RelativePath_DotSlash method.

## ShellOperatorsTests
Focused tests for shell operators per specification section 5 - Pipes (|) - Output redirection (>, >>) - Input redirection (<) - Command sequencing

**Source:** D:\workspace\Cssh\cssh.Tests\ShellOperatorsTests.cs

### Members

- **Field** `private readonly CommandParser _parser;` — The _parser field.
- **Field** `private readonly CommandRegistry _registry;` — The _registry field.
- **Field** `private readonly CommandRunner _runner;` — The _runner field.
- **Field** `private readonly ShellState _state;` — The _state field.
- **Field** `private readonly string _testDir;` — The _testDir field.
- **Constructor** `public ShellOperatorsTests()` — 
- **Method** `public void Parser_ShouldRecognizeOutputRedirectTruncate()` — Executes the Parser_ShouldRecognizeOutputRedirectTruncate method.
- **Method** `public void Parser_ShouldRecognizeOutputRedirectAppend()` — Executes the Parser_ShouldRecognizeOutputRedirectAppend method.
- **Method** `public void Parser_ShouldRecognizeInputRedirect()` — Executes the Parser_ShouldRecognizeInputRedirect method.
- **Method** `public void Parser_ShouldRecognizePipe()` — Executes the Parser_ShouldRecognizePipe method.
- **Method** `public void Operator_OutputRedirect_ShouldTruncateExistingFile()` — Executes the Operator_OutputRedirect_ShouldTruncateExistingFile method.
- **Method** `public void Operator_OutputRedirect_ShouldCreateNewFile()` — Executes the Operator_OutputRedirect_ShouldCreateNewFile method.
- **Method** `public void Operator_OutputRedirect_ShouldHandleSpecialCharacters()` — Executes the Operator_OutputRedirect_ShouldHandleSpecialCharacters method.
- **Method** `public void Operator_AppendRedirect_ShouldAppendToExistingFile()` — Executes the Operator_AppendRedirect_ShouldAppendToExistingFile method.
- **Method** `public void Operator_AppendRedirect_ShouldCreateFileIfNotExists()` — Executes the Operator_AppendRedirect_ShouldCreateFileIfNotExists method.
- **Method** `public void Operator_AppendRedirect_MultipleAppends_ShouldPreserveAllLines()` — Executes the Operator_AppendRedirect_MultipleAppends_ShouldPreserveAllLines method.
- **Method** `public void Operator_InputRedirect_ShouldReadFileContent()` — Executes the Operator_InputRedirect_ShouldReadFileContent method.
- **Method** `public void Operator_InputRedirect_NonExistentFile_ShouldReturnError()` — Executes the Operator_InputRedirect_NonExistentFile_ShouldReturnError method.
- **Method** `public void Operator_Pipe_ShouldTransmitOutput()` — Executes the Operator_Pipe_ShouldTransmitOutput method.
- **Method** `public void Operator_Pipe_TwoStages_ShouldChainCorrectly()` — Executes the Operator_Pipe_TwoStages_ShouldChainCorrectly method.
- **Method** `public void Operator_Pipe_WithFinalRedirect_ShouldApplyRedirectToLastCommand()` — Executes the Operator_Pipe_WithFinalRedirect_ShouldApplyRedirectToLastCommand method.
- **Method** `public void Operator_Pipe_MultipleStages_ShouldChainAll()` — Executes the Operator_Pipe_MultipleStages_ShouldChainAll method.
- **Method** `public void Operators_Combined_PipeAndRedirect()` — Executes the Operators_Combined_PipeAndRedirect method.
- **Method** `public void Operators_Combined_InputAndPipe()` — Executes the Operators_Combined_InputAndPipe method.
- **Method** `public void Operator_InvalidRedirectPath_ShouldHandleError()` — Executes the Operator_InvalidRedirectPath_ShouldHandleError method.
- **Method** `public void Spec_ShouldNotSupportConditionalExecution()` — Executes the Spec_ShouldNotSupportConditionalExecution method.
- **Method** `public void Spec_ShouldNotSupportBackgroundExecution()` — Executes the Spec_ShouldNotSupportBackgroundExecution method.
- **Method** `public void Dispose()` — Executes the Dispose method.

## ShellStateTests
Represents the ShellStateTests class.

**Source:** D:\workspace\Cssh\cssh.Tests\csshShellStateTests.cs

### Members

- **Method** `public void Initial_CurrentDirectory_ShouldBe_EnvironmentCurrentDirectory()` — Executes the Initial_CurrentDirectory_ShouldBe_EnvironmentCurrentDirectory method.
- **Method** `public void Initial_PreviousDirectory_ShouldBe_EnvironmentCurrentDirectory()` — Executes the Initial_PreviousDirectory_ShouldBe_EnvironmentCurrentDirectory method.
- **Method** `public void Updating_CurrentDirectory_ShouldNotChange_PreviousDirectory()` — Executes the Updating_CurrentDirectory_ShouldNotChange_PreviousDirectory method.
- **Method** `public void Updating_PreviousDirectory_ShouldNotChange_CurrentDirectory()` — Executes the Updating_PreviousDirectory_ShouldNotChange_CurrentDirectory method.


