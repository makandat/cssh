# cssh: C# スクリプト拡張言語 (C# Script Plus)

# １ 概要
- cssh 0.2 ではシェルの標準言語として C# スクリプトを簡単に実行できるようにしたが、C# スクリプトの拡張は行わず、cssh 標準関数 (Script.Std) を導入するにとどまった。
- cssh 0.3 では 、さらに C# スクリプトに「スクリプトらしさ」を与えるために、言語拡張を行った C# Script Plus (拡張子は .csp) を導入する。
- csp は拡張構文を使わなければ、.csx (C# script) と全く同じとする。つまり、C# script の特殊構文 #r, #load をそのまま使える。
- csp はインタプリタでもコンパイラでもない。つまり、後述する拡張構文を C# スクリプト構文に変換するトランスレータである。
- cssh 0.3 の csp はプロトタイプ版であり、仕様検討や実装検討のために導入する。
- csp は本格的なプログラムを作ることを前提にしない。つまり、手軽な短いプログラムを対象とする。(本格的なものは C# に任せる)
- csp は言語拡張を行うが、独自のインタプリタを持つものではなく、一度、C# スクリプトに変換し、dotnet-script コマンドで実行する。
- 基本的な目標は、「儀式的な構文の排除」、「Ruby や Python のような既存言語の簡易な構文表現を取り込む」、「データ中心の構文」、「シェル言語としての使いやすさ」などとする。
- cssh 0.2 で導入した標準関数はそのまま利用できるし、必要なら便利な関数を追加する。
- csp は構文糖を C# スクリプトに変換するだけであり、名前解決・型解決・オーバーロード解決などの意味解析は行わない。(Roslyn に任せる）
- cssh とは独立して csp 単独で実行することができるものとする。(dotnet-script のような dotnet-csp コマンドも追加)
- sqlite3 の標準サポート (Python のような)
- csp の最初のバージョンは cssh のバージョンに合わせて v0.3.0 から始める。

## 1.1 C# Script Plus (csp) のポリシー
- C# はちょっとしたコードを書くには大きすぎるし仰々しい。-> 簡易構文、スクリプト風構文
- スクリプト風構文の例： Console.WriteLine(x) -> println(x)
- Java 的な儀式構文はできるだけ排除。
- C# のすばらしさ (堅牢性、信頼性など) をそのまま生かしたい。
- Python, JavaScript, Ruby のような人気スクリプト言語のいいとこどり。
- 直感性、読みやすさ重視。
- 長く複雑あるいは重いプログラムは作らない。-> そういうのは C# に任せる。
- C# の構文がそのまま使えるので、複雑や重い処理も書くことはできる。(推奨はしないが）


# ２ 言語拡張
- csp は C# script (.csx) を基盤とし、構文糖を C# script に変換するトランスレータである。
- 名前解決・型解決・オーバーロード解決などの意味解析は行わず、
- 最終的な解釈は Roslyn に任せる。
- 以下の構文は csp 独自の拡張であり、すべて C# script に変換されて実行される。
- 標準関数 (cssh.Std.dll) をデフォルトで使用できる(#r は不要)。ただし、cssh.Std.dll は csp DLL (csp.Csplus.dll) と同じ場所にコピーしておく必要がある。

## 2.1 def キーワード
- Ruby や Python で使われる def を導入する。
- 変数宣言の場合は、dynamic 型の変数を意味する。
- 関数の場合は、パラメータはすべて dynamic 型、戻り値も dynamic 型とする。
- def 型の値の挙動は C# の仕様通りとする。
- 長いスクリプトの場合は、エラーが発生しやすくなるので、非推奨とする。(行数がある制限を超えた場合、警告を表示。警告する最大行 const int NDEF_MAX = 200 とする)
### 変数宣言
変換前
```
def x = expr
```
変換後
```
dynamic x = expr;
```
### 関数定義
変換前
```
def f(a, b) { ... }
```
変換後
```
dynamic f(dynamic a, dynamic b) {
    ...
}
```
### ラムダ式
変換前
```
def f = x => x + 1
```
変換後
```
dynamic f = (Func<dynamic, dynamic>)(x => x + 1);
```


## 2.2 三項演算子の拡張
- 従来の ? : (三項演算子) は必ず値を返す必要があるが、? と :  の後には式でなくブロックも書けるようにする。
- : の後のブロックを省略する場合は、必ず文末を示す ; が必要である。
- 例は 3.3 も参照
（例）
```
a > 0 ? a -= 2 : println(“end”)
a > 0 ? a -= 2;
```
変換前 (? : 場合)
```
cond ? { A } : { B }
```
変換後
```
if (cond) {
    A
} else {
    B
}
```
変換前 (? ; (else がない場合))
 ```
 cond ? { A };
 ```
変換後
 ```
 if (cond) {
    A
}
```

## 2.3 C 言語風のキャスト
- C# では様々な値の変換方法があるが、その代わり分かりづらさがある。
- csp ではすべて C 言語風のキャスト演算子 (type) により値の変換を可能とする。<br>
(例)
```
double x = 10.5; var s = (string)x
```
- 特別な変換ではなく、C# のキャストを利用する。(type)expr


## 2.4 繰り返し構文
### 2.4.1 パラメータ付きブロック
- Ruby 風のパラメータ付きブロックを導入する。例: {|key, value| …. }
- |..| 部を省略したときは、暗黙のパラメータ _, _i, _k, _v が使える。
- 例は 3.5 および 3.6 参照
- C# への変換は以下のようにする。
変換前
```
{ |a, b| ... }
```
変換後
```
(a, b) => { ... }
```

### 2.4.2  loop（無限ループ）
- ブロックを無限回繰り返す。
- ブロックから抜け出すには until 文を使う。 (「2.13 until 文」も参照)
- 例は 3.4 参照
- C# への変換は以下のようにする。
変換前
```
loop { ... }
```
変換後
```
while (true) {
    ...
}
```
- ループから抜け出す (until expr) 
変換前
```
until expr
```
変換後
```
if (expr) break;
```

### 2.4.3 times（for 文に変換）
- Ruby の times 風の構文で for 文の代わりに使用できる。
- ブロック内で until 文が使用可能 (「2.13 until 文」も参照)
- 例は 3.5 参照
- C# への変換は以下のようにする。
変換前
```
n.times { |i| ... }
```
変換後
```
for (var i = 0; i < n; i++) {
    ...
}
```
- |i| を省略したとき
変換前
```
n.times { ... }
```
変換後
```
for (var _ = 0; _ < n; _++) {
    ...
}
```

### 2.4.4 each（foreach に変換）
- Ruby の each 風構文で foreach 文の代わりに使用できる。
- スカラーのコレクションである配列、リスト、集合に対しては each を使う。
- Key:Value 型のコレクションに対しては each_k, each_v, each_kv を使う。ただし、k はキー、v は値を意味する。
- each はコレクションの要素を列挙するが、同時にインデックスも取得する場合は each_iv を使用する。ただし、i はインデックス、v は値を意味する。
- ブロック内で until 文が使用可能 (「2.13 until 文」も参照)
- 例は 3.5 参照

#### スカラーコレクション each
変換前（明示パラメータ）
```
list.each { |v| ... }
```
変換後
```
foreach (var v in list) {
    ...
}
```
変換前（暗黙パラメータ）
```
list.each { ... }
```
変換後
```
foreach (var _v in list) {
    var v = _v; // ブロック内での暗黙パラメータ _v / v 相当
    ...
}
```
#### インデックス付き each_iv
変換前（明示パラメータ）
```
list.each_iv { |i, v| ... }
```
変換後
```
for (var i = 0; i < list.Count; i++) {
    var v = list[i];
    ...
}
```
変換前（暗黙パラメータ）
```
list.each_iv { ... }
```
変換後
```
list.each_iv { ... }
```

#### Key:Value コレクション each_k / each_v / each_kv
変換前（each_k, 明示パラメータ）
```
dict.each_k { |k| ... }
```
変換後
```
foreach (var k in dict.Keys) {
    ...
}
```
変換前（each_k, 暗黙パラメータ）
```
dict.each_k { ... }
```
変換後
```
foreach (var _k in dict.Keys) {
    ...
}
```
変換前（each_v, 明示パラメータ）
```
dict.each_v { |v| ... }
```
変換後
```
foreach (var _v in dict.Values) {
    var v = _v;
    ...
}
```
変換前（each_v, 暗黙パラメータ）
```
dict.each_v { ... }
```
変換後
```
foreach (var _v in dict.Values) {
    ...
}
```
変換前（each_kv, 明示パラメータ）
```
dict.each_kv { |k, v| ... }
```
変換後
```
foreach (var kv in dict) {
    var k = kv.Key;
    var v = kv.Value;
    ...
}
```
変換前（each_kv, 暗黙パラメータ）
```
dict.each_kv { ... }
```
変換後
```
foreach (var kv in dict) {
    var _k = kv.Key;
    var _v = kv.Value;
    ...
}
```

## 2.5 例外処理構文
- JavaScript の Promise のような .then{...).catch {...}.finally {} も使用可能。
- 致命的エラーがあった場合、raise 文で例外を投げることができる。(Exception をインスタンス化不要)
- 例は 3.7 参照

then 変換前
```
expr.then { |x| ... }
```
変換後
```
try {
    var x = expr;
    ...
}
```

catch 変換前
```
expr.then { ... }.catch { |e| ... }
```
変換後
```
try {
    ...
} catch (Exception e) {
    ...
}
```
finally 変換前
```
expr.then { ... }.catch { ... }.finally { ... }
```
変換後
```
try {
    ...
} catch (Exception e) {
    ...
} finally {
    ...
}
```
raise 変換前
```
raise "error message"
```
変換後
```
throw new Exception("error message");
```

## 2.6 シェル風演算子
- | (パイプライン)
- >, >>, < (リダイレクト)

| 変換前
```
cmd1 | cmd2
```
変換後
```
ScriptStd.Pipe(cmd1, cmd2);
```
リダイレクト(>) 変換前
```
cmd > "out.txt"
```
変換後
```
ScriptStd.Redirect(cmd, "out.txt", append: false);
```

追加リダイレクト(>>) 変換前
```
cmd >> "out.txt"
```
変換後
```
ScriptStd.Redirect(cmd, "out.txt", append: true);
```
入力リダイレクト(<) 変換前
```
cmd < "input.txt"
```
変換後
```
ScriptStd.InputRedirect(cmd, "input.txt");
```


## 2.7 フロー演算子
- オブジェクトのメソッドは . でつないで流れるような処理ができる。
- 演算子 => はメソッドの代わりに式をつないで、式の値を流れるように処理できる演算子である。
- 直前の式の値は特殊な変数 _ で取得できる。
- 最後に実行された式の値が下流へ _ として送られる。
- => の式の後には改行を入れて読みやすくできる。
- 式の代わりにブロックを書くことができる。その場合、ブロックの最後の式の値が下流へ送られる。

変換前 (式だけの場合)
  x + 10.0 => radians(_) => cos(_) => y;
変換後
  y = cos(radians(x + 10.0));
変換前 (オブジェクトのメソッドを含む場合)
```
 str.ToLower() => _.StartssWith("xyz") => $stdout
```
変換後
```
  Console.Write(str.ToLower().StartsWith("xyz"));
```
変換前 (長いフロー)
input
  => _.Trim()
  => _.ToLower()
  => parse(_)
  => radians(_)
  => cos(_)
  => normalize(_)
  => y
変換後
y = normalize(cos(radians(parse(input.Trim().ToLower()))));
変換前 (ブロックがある場合)
expr
  => {
       a = _ + 1
       b = a * 2
       b + 10   # ← これが返り値
     }
  => f(_)
  => y
  変換後
  y = f((() => {
        var __tmp = expr;
        var a = __tmp + 1;
        var b = a * 2;
        return b + 10;
     })());

*将来の機能強化(案)*
- => の途中（節と呼ぶ）は独立して動く。つまり、データごとに並行処理が可能。
- 節は式とブロックが可能。
- 節の先頭には待ち行列 (Queue) があり、前の処理が終わるまで自動的に次のデータが待ち状態になる。
- 節の中にフローを書くのは禁止。(動作が複雑になるため)
- ラベルを途中に挟み、ループ処理ができる。(節の途中で back LABEL のような文が使える)
- 節の中で leave 文により途中でフローから抜けることができる。
- フロート中で例外が発生したときの、catch {...}, finally { …  } が節として使える。
- catch { … } のなかでは redo 文で再試行可能。

## 2.8 グローバル定数・変数
- $stdout, $stdin, $stderr: 標準入出力
- $env.KEY: 環境変数
- $settings: 設定ファイルの KV 表現 (設定ファイルは settings.json とする)
- $path_separator: ファイルパスのセパレータで、動作環境が Windows かどうかの判別にも使用できる。

## 2.9 import 文と include 文
- #r の代わりに import 文を使える。
- 環境変数 PYTHONPATH のような CSPPATH をサポートする。つまり、CSPPATH が定義されていれば、その内容を DLL が存在するディレクトリとみなす。
- CSPPATH の内容は１つのディレクトリとする。（終端の / や \ は不要)
- #load の代わりに include 文を使える。include でも CSPPATH が有効とする。
- import, include とも DLL ファイルの拡張子 .dll は省略できる。
- CSPPATH が未定義の場合、import, include は DLL の絶対パスを指定する必要がある。

(例)
CSPPATH = "C:\lib" のとき、import "scriptStd" は C:\lib\scriptStd.dll とみなされる。

## 2.10 文末のセミコロンと文の継続
- 1 行に1つの文しかない場合、文末の ; は省略可能。
- 複数行にまたがる文の場合、文の最後を明示するため ; が必要。
- 複数行にまたがる文の場合、文末に ‘\’ を置くことで文の継続を明示する。(‘\’の前には1つ以上の空白が必要)
- ただし、括弧 ()、ブロック {}, あるいは do … end、配列 [] が開いている場合は、C# と同様に文が継続しているとみなし、‘\’ は不要とする。

## 2.11 ブロック { } の使用制限 (do … end)
辞書リテラル { key: value } とブロック { ... } の区別が曖昧になる場合、ブロックは do … end を用いること。
```
{ x: 1, y: 2 }      // 辞書
{ |x| println(x) }  // ブロック
```

## 2.12 with 文
- C# の using には2つの意味がある。
- ファイルオブジェクトの自動解放の場合の using は with と書いてもよいものとする。

## 2.13 until 文
- loop, times, each のブロック内でのみ使用できる。（旧来の C# 構文の中では使えない）
- ブロック内での場所は制限しない。(どの位置でもループから脱出可能)

## 2.14 new 演算子の省略
- クラスをインスタンス化するとき、Python のように new を使わず クラス名だけを書くことができる。
- これはクラスのコンストラクタを関数のように扱うことを意味する。
- Generic 型のクラスも new を単純に省略して書くことができる。
例:
```
    List<int>()                     → new List<int>()
    Dictionary<string, int>(10)     → new Dictionary<string, int>(10)
    Dictionary<string, List<int>>() → new Dictionary<string, List<int>>()
```
これは Python のクラス呼び出しと同様に、
コンストラクタ呼び出しの糖衣構文として扱う。

## 2.15 正規表現マッチ演算子
- Perl 風の =~ 演算子をサポートする。(!~ も同様に)
- 正規表現文字列も / …  / と書くことができるようにする。ただし、マッチ演算子を使うときのみ限定。
- ただし、置換演算子 s/../../ などはサポートしない。
- 拡張した ? : 演算子とともに使えること。
変換前 =~ 演算子
```
expr =~ /pattern/
```
変換後
```
System.Text.RegularExpressions.Regex.IsMatch(expr, "pattern")
```

変換前 !~ 演算子
```
expr !~ /pattern/
```
変換後
```
!System.Text.RegularExpressions.Regex.IsMatch(expr, "pattern")
```



## 2.16 バッククォート演算子
- Perl 風の \` … \` (バッククォート演算子) もサポートする。これは、\` \` 内部のコマンドを実行し、結果を文字列で返す。
変換前
```
\`command arg1 arg2\`
```
変換後
```
__csp_exec("command arg1 arg2")
```
ただし、__csp_exec は次のように実装する。
```
string __csp_exec(string cmd) {
    var psi = new ProcessStartInfo() {
        FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash",
        Arguments = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? $"/C {cmd}"
            : $"-c \"{cmd}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };
    using var p = Process.Start(psi);
    var output = p.StandardOutput.ReadToEnd();
    p.WaitForExit();
    return output;
}
```

## 2.17 sqlite3 標準サポート
- データの永続化が簡単にできる。しかも、検索や管理が容易である。
- 大量データのSQL による複雑な検索ができる。
- 一方、簡単化のため機能は限定する。(よく使う機能に限定する)
- SQL は完全なもの (sqlite3 コマンドで実行できるような) のみをサポートする。
- トランザクションはサポートしない。
- バッチ処理はサポートしない。
- データ型は数(Integer/Real)、文字列(Text)、日付時刻(DateTime)のみで、BLOB はサポートしない。
- テーブルは必ず整数型 autoincrement の主キーを持つこと。その主キーは先頭フィールドとする。
- プロバイダは Microsoft.Data.Sqlite を使う。(dotnet add package Microsoft.Data.Sqlite でインストールできる)

### 2.17.1 データベースオブジェクト
データベースオブジェクトの作成は sqlite3(path=":MEMORY:") ファクトリー関数で行う。path を省略したときは、オンメモリのDBが作成される。
```
def db = sqlite3(DBPATH)
def memdb = sqlite3()
```

### 2.17.2 クエリー
以下のメソッドがある。
- query(SQL) : SELECT を実行し結果セット (辞書型の行のコレクション) を返す。
- getRow(SQL) : １行を返す SELECT を実行し行 (辞書型の１つの行) を返す。
- getValue(SQL) : 1つの値を返す SELECT を実行しスカラーを返す。
- execute(SQL) : INSERT のような結果セットを返さないSQLを実行する。
- execute("INSERT INTO ...") の場合は、挿入したデータの主キーを返す。失敗したときは、-1 を返す。
- execute("UPDATE ...") の場合は、更新した行数を返す。失敗したときは、-1 を返す。
- execute("DELETE FROM ...") の場合は、削除した行数を返す。失敗したときは、-1 を返す。
- execute("CREATE TABLE ...") の場合は、成功なら 1, 失敗したときは 0 を返す。
- execute("DROP TABLE ...") の場合は、成功なら 1, 失敗したときは 0 を返す。
- SQL のデータに ' が含まれていたら実行前に '' に置換する。


## 2.18 設定ファイル
- 設定ファイルを起動時に自動で読み取り、グローバル変数 $settings に格納する。これは、$settings.Key でアクセスできる。
- 設定ファイルの更新はできない。（ユーザが直接、エディタで変更する）
- 設定ファイルは、JSON 形式。ただし、ネストなしとする。
- ファイル名は、settings.json で、カレントディレクトリのみを検索対象とする。
変換前
```
$settings.Key
```
変換後
```
__csp_settings["Key"]
```
__csp_settings() は次のように実装する。
```
var __csp_settings = new Dictionary<string, object>();

var path = Path.Combine(Environment.CurrentDirectory, "settings.json");
if (File.Exists(path)) {
    var json = File.ReadAllText(path);
    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    if (dict != null) {
        __csp_settings = dict;
    }
}
```

## 2.19 オブジェクトのアクセス
- KV 形式のオブジェクトは、JavaScript のように obj.key 形式でもアクセスできる。
変換前
```
obj.Key
```
変換後
```
obj["Key"]
```

## 2.20 readonly
- C# の readonly 修飾子は使えるシーンが限られている。
- csp では readonly は JavaScript の const の機能に近く、どこでも使用できる。
- これは変換ではなく、C# Script Plus がソースを検索し、readonky 変数に代入後に再び代入しようとするコードを見つるとエラーメッセージを表示する。

## 2.21 AI 向けコメント
- #<sp> に続くコメントは AI への指示コメントである。ただし、<sp>は空白を意味する。
- これは AI がコメントにしたがってコード追加、補間、修正などに使う。
- （例）#<sp>に続く文を理解し、コードのコード追加、補間、修正を行う。コード修正後、# は //# に置き換えること。
- もし、実行時に AI向けコメントが含まれていた場合、エラーが出てしまう。よって、# を //# で置き換えてエラーが出ないようにする。

## 2.22 特殊な文字列表現
- Ruby 風の %Q[] や %q() などが使える。これは、" や ' を含む文字列をそのままの形で書けるので便利である。
- %Q[...]	ダブルクォート文字列	**式展開（#{}）**やエスケープ文字が有効。
- %q[...]	シングルクォート文字列	式展開を行わない。純粋な文字列として扱う。
- [] は、() や {} でもよい。
- [] の中では改行してもよい。
- ${var} という書式を埋め込み変数とみなす。
- \${...}, \] は単なる文字とみなす。
- \ は、この場合を除いて単なる文字とみなす。また、\\ は C# 変換時のため1個の\とみなす。
変換前 %Q 
```
def s = %Q[Hello "#{name}", today is #{date}]
```
変換後
```
var s = "Hello \"" + name + "\", today is " + date;
```
変換前 %q
```
def s = %q(It's a "raw" string)
```
変換後
```
var s = "It's a \"raw\" string";
```
変換前
```
def name = "Noname"
def s = %Q[Hello "${name}"]
```
変換後
```
"This is not embed: ${value}"
```

## 2.23 Action, Func の簡易記法
- Action, Func は .NET であらかじめ定義されているデリゲートである。
- この２つのデリゲートを使った無名関数を簡単に定義できる記法を提供する。

変換前 (Action)
```
var subr = void (string s, object x) { Console.WriteLine(s + " = " + x.ToString() + "\n")}
```
変換後
```
var subr = Action<string, object> (s, x) => { Console.WriteLine(s + " = " + x.ToString() + "\n")};
```
変換前 (Func)
```
var fun = string (string s, object x) { s + " = " + x.ToString() + "\n" }  // ブロック内の最後の式の値を戻り値とする。
```
変換後
```
var subr = Func<string, object, string> (s, x) => s + " = " + x.ToString() + "\n";
```
- 最後のパラメータが無名関数だった場合、ブロックとして書くことができる。

変換前
```
for(1, 10, 2) {|i| action(i)} // for(1, 10, 2, i => action(i)) と同じ
```
変換後
```
var @for = void (int begin, int end, int step, Action<int> action)
{
  for (int i = begin; i <= end; i += step)
  {
    action(i);
  }
}
```

## 2.24 文字列引用符
- C# では ' は文字型リテラルに使用される。
- csp では JavaScript のように文字列リテラルの引用符として " と同じように使用する。
- 文字列リテラルは ord(code) 関数を使う。
変換前
```
"y = 'A'";'c = "A"'
変換後
"y = \"A\"";"c = \"A\"";


# ３ 拡張構文などの使用例

## 3.1 標準関数
- #r や using static なしで csp 標準関数を使用できる。(STDSCRIPT_JA.md 参照)
- 標準関数とは直接関係ないが、下の例のように1 行に1つ文ではセミコロンは省略できる。
```
println(“Hello, World!”)
```
- Python の組み込み関数の一部や Math 関数の一部は標準関数として使える。

```
string s = chr(0x41)
println(ord(s))
double y = log10(10.0)
println(y)
```

## 3.2 def キーワード
def キーワードは C# の dynamic と深く関係する。
var は型推論に関連する(型が決まる)が、def は dynamic 型である。
```
def x = 1  // dynamic
var y = 2 // 型推論により決まった int
```
関数定義にdef を使うと、関数値とパラメータすべてが dynamic 型である。
```
def add(x, y)
{
  return x + y
}
def y = add(3, 1.5)  # y = 4.5
```
- C# ではラムダ式を直接 dyncmic 型に代入できないが、csp ではそれを許す。
```
def mylambda = x => 2 *x
// これは以下と同じ
// var mylamba = (Func<dynamic, dynamic>)( x => 2 * x);
```

## 3.3 三項演算子の拡張
- csp では ? : (三項演算子) は式の値を返さなくてもよく、中身が複数行のブロックも使用できる。
```
def y = 10
data is int or long or float or double or decimal ? {
  y = data - 1
  println(y)
}
: {
  println(data)
}
```
- else 節がない場合は、文の終わりを示す ; が必要。
```
def y = 10
data is int or long or float or double or decimal ? {
  y = data - 1
  println(y)
};
```

## 3.4 loop と until
無限ループからの脱出は until 文が必要。until の場所はブロック内のどこでもよい。
```
string s
loop
{
   s = getNext()
  until String.IsNullOrEmpty(s)
  println(s)
}
```

## 3.5 times
- times は for 文に代わるスマートな書き方。
- n.times {|i| …. }のような書き方の他、n.times { … }のように|i| を省略する書き方もできる。
- (注意) i は繰り返し変数、n は int または int に変換できること。
- |i| (ブロックパラメータ) を省略したときは、ブロック内でのみ有効な特殊変数 _ が繰り返し変数として使える。
- 繰り返し変数は 0 から始まり n-1 で終わる。
- 上記のルールに従わない場合は、ブロック内で変換するか、for 文を使用する。
```
100.times { |i| println(i) }
100.times { println(_) }
```
- 開始の値 (0) とステップの値 (1) は固定なので、もし、そうでない場合は for 文を使用する。
```
for (var i = 1; i < 100; i += 2)
{
    ...
}
```

## 3.6 each
- each は配列やリスト、辞書、集合などの要素に対して繰り返し処理を行う場合に使用し、foreach 文の代わりとして使用できる。 
- 要素とともにインデックスも取得したいときは、each_iv を使用する。
- key:value コレクション構造を持つオブジェクトに対しては、each_k, each_v, each_kv を使用する。
```
var data = {name:"A", type:"T", count:0};
data.each_k {|key| .... }  // 'name', 'type', 'count'
data.each_v {|value| .... } // "A", "T", 0
data.each_kv {|key, value| .... } // key="namae", value="A", key="type",value="T", key="count",value=0
var data2 = [10, 20, 30]
data2.each {|value| ... }  // 10, 20, 30
data2.each_iv {|index, value| .... }  // index=0,value=10, index=1,value=20, index=2,value=30
```

## 3.7 JavaScript Promise 風の例外処理
- SQLite3 に接続するとき、例外が発生する例。
```
sqlite(PATH)
.then {|db| .... }
.catch {|err| error(err)}
```

## 3.8 Action デリゲートを使って for 文をカプセル化
- times は初期値が 0, ステップが 1 固定であるため、それ以外のケースでは for 文を使う必要がある。
- この例は、生の for 文の代わりに for をカプセル化した関数を定義するものである。
```
var @for = void (int begin, int end, int step, Action<int> action)
{
  for (int i = begin; i <= end; i += step)
  {
    action(i);
  }
}

for(1, 10, 2, void (int i) { println(i) });
```
- この例のように最後のパラメータが Action, Func だった場合、次のように書くこともできる。
```
for(1, 10, 2) { |i| println(i) }
```

# ４ dotnet-csp コマンド
## 4.1 機能

- 拡張構文 (2. 参照) を含む csp ソースを dotnet-script コマンドで実行できるような完全な C# スクリプトソースに変換する。
- 変換が成功した場合は、結果を一時ファイルに保存し dotnet-script で実行する。
- 拡張構文が含まれていなければ、単純に一時ファイルにコピーして dotnet-script で実行する。
- 構文エラーがあった場合は、エラーメッセージを表示して実行はしない。
-警告だけの場合は、実行の判断をユーザに任せる。
- \-s <file> オプションが指定された場合、変換結果を指定されたファイルに保存し、dotnet-script による実行は行わない。
- 一時ファイルは、一時フォルダ (%TEMP%:Windows, $TMPDIR:Windows 以外) に作成し、csp_temp_yyyymmdd_hhmmss.csx とする。

実行までのフロー
　.csp (構文糖の変換) ->  .csx (dotnet-script)-> 実行

T.B.D.
# ５ 実装
csp はシェル cssh に組み込まれるとともに dotnet-csp コマンドでも使われる。
よって、１つの独立した DLL とすると便利。

T.B.D.

# ６ マイルストーン
## v0.1.0
  #r なしでcssh 標準関数を実行できる。
