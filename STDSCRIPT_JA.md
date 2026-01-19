# cssh 標準関数仕様書 (Rev.3)

# １．概要
## 1.1 目的
- cssh では C# スクリプトを標準スクリプトとして使用する。
- しかし、C# スクリプトの構文は C# そのものに非常に近いので、スクリプトらしい簡単な関数がない。
- ここでは、その問題を軽減するための cssh 標準関数を定義する。

（例)
```
println(“Hello World!”);
```

## 1.2 実装方法
- ソリューション scch にプロジェクト cssh.Std/cssh.Std.csproj を作る。
- ソースファイルは ScriptStd.cs
- DLLファイルは bin の最下層に cssh.Std.dll ができる。

## 1.3 使用方法
- using static ScriptStd; で参照できる。
- プロジェクトファイル (.csproj) の ItemGroup に以下の１行を追加する必要がある。
```
<ProjectReference Include="..\cssh.Std\cssh.Std.csproj" />
```

# 2. 出力系関数
## print(object value)
- 説明：改行なしで値を標準出力へ書き込む
- 戻り値：なし
- シグネチャ: print(object)
- 例：print("hello")

## println(object value)
- 説明：値を標準出力へ書き込み、末尾に改行を付ける
- 戻り値：なし
- シグネチャ: println(object)
例：println("hello")

## printf(string format, params object[] args)
- 説明：フォーマット文字列に従って出力する。フォーマットはCスタイルだが、%d, %s, %f, #書式のみ対応する。
- 戻り値：なし
- 例：printf("name=%s age=%d", name, age)

## debug(object value) または error(object value)
- 説明：デバッグ用出力。出力先は標準エラー出力 (stderr) とする。
- 戻り値：なし
- 例：debug("x=" + x)

# 3. 文字列系関数

## format(string format, params object[] args)
- 説明：フォーマット文字列に従って文字列を生成して返す
- 戻り値：string
- 例：var msg = format("Hello {0}, age {1}", name, age); println(msg);

## merge(string[] arr, string separator="")
- 説明: 文字列配列の要素をすべて結合して１つの文字列にする。separator が指定されている場合は、結合する文字列間にそれを挟む。
- 戻り値：string
- 例: string[] arr = ["two", "dogs"]; var merged = merge(arr);

## split(string str, string separator=",")
- 説明: 文字列を separator で指定した文字列で分割して、文字列配列を返す。
- 戻り値: string[]
- 例: var arr = split(input);

## index(string input, string str)
- 説明: 文字列 input に str が含まれていれば、その位置を返す。含まれない場合は、-1 を返す。
- 戻り値: int
- 例: var p = index("C#;C++", ";")

## substr(string input, int start, int length)
- 説明: 文字列 input の位置 start から長さ(文字数) length の部分文字列を取得する。(String.Substring()のラッパー)
- 戻り値: string
- 例: var s = substr("0123456789", 3, 2);

## startsWith(string input, string str)
- 説明: 文字列 input が str で始まっていたら true, そうでないなら false を返す。(case sensitive, String::StartsWih()のラッパー)
- 戻り値: bool
- 例: var b = startsWith("C#;C++", "C")

## endsWith(string input, string str)
- 説明: 文字列 input が str で終わっていたら true, そうでないなら false を返す。(case sensitive, String::EndsWih()のラッパー)
- 戻り値: bool
- 例: var b = endsWith("C#;C++", "++")

## trim(string input)
- 説明: 文字列 input の前後の空白文字列を削除した文字列を返す。(String.Trim()のラッパー)
- 戻り値: string
- 例: var str = trim("\tABC\n")

# 4. 入力系関数

## input(string prompt)
- 説明：プロンプトを表示し、ユーザーから 1 行入力を受け取るプロンプトの表示は Console.Write(prompt) で行い、入力文字列には最後の改行文字を含めない。
- 戻り値：string?
- 例：var name = input("name > ");

## gets()
- 説明：プロンプトなしで 1 行入力を受け取る
- 戻り値：string?
- 例：var line = gets();

# 4. 日付・時間系関数

## today()
- 説明：今日の日付を "yyyy-MM-dd" 形式で返す
- 戻り値：string
- 例：println(today());

## now()
- 説明：現在日時を ISO 8601 形式で返す
- 戻り値：string
- 例：println(now());

## datetime(DateTime? dt, string format)
- 説明：指定フォーマットで日時文字列を返す
- 戻り値：string
- シグネチャ: string datetime(DateTime? dt = null, string format = "yyyy-MM-ddTHH:mm:ss");
- 例：println(datetime("yyyy/MM/dd HH:mm"));
println(datetime(new DateTime(2000, 1, 1), "yyyy-MM-dd"));

# 6. ファイル操作系関数
## read(string path)
- 説明：ファイル内容をすべて読み込んで返す。
- 戻り値：string
- 例：var text = read("data.txt");

## write(string path, string content)
- 説明：ファイルに内容を書き込む（上書き）
- 戻り値：なし
- 例：write("log.txt", "hello");

## append(string path, string content)
- 説明：ファイルに内容を追記する
- 戻り値：なし
- 例：append("log.txt", "more");

## exists(string path)
- 説明：ファイルの存在を確認する
- 戻り値：bool
- 例：if (exists("config.json")) { ... }

# 7. プロセス実行系関数

## system(string command)
- 説明：外部コマンドを実行し、標準出力を文字列として返す。(画面に表示されない)
- 戻り値：string
- 例：var result = system("ls -l");

## run(string command)
- 説明：外部コマンドを実行し、標準出力をそのまま流す。(画面に表示される)
- 戻り値：なし
- 例：run("echo hello");

# 8. 制御系関数

## exit(int code=0)
- 説明：指定した終了コードでプロセスを終了する
- 戻り値：なし（戻らない）
- 例：exit(1);

## abort(string message="")
- 説明：プロセスを即座に終了する
- 戻り値：なし
- 例：abort("fatal error");

# 9. 正規表現系関数

## match(string text, string pattern)
- 説明：text が pattern にマッチするかどうかを返す
- 戻り値：bool
- シグネチャ：bool match(string text, string pattern)
- 例：if (match(name, "^[A-Z].*")) { ... }

## search(string text, string pattern)
- 説明：text の中で pattern に最初にマッチした部分文字列を返す。 マッチしない場合は null。
- 戻り値：string?
- シグネチャ：string? search(string text, string pattern)
- 例：var str = search(line, “[0-9]+”);

## replace(string text, string pattern, string replacement)
- 説明：正規表現に一致した部分を置換する
- 戻り値：string
- シグネチャ：string replace(string text, string pattern, string replacement)
- 例：var s = replace(line, "[0-9]+", "###");

# 10.コマンドライン引数系関数
## argc()
- 説明：コマンドライン引数の数を返す
- 戻り値：int
- シグネチャ：int argc()
- 例：println(argc());

## args(int index)
- 説明：指定したインデックスのコマンドライン引数を返す
- 戻り値：string?（範囲外なら null）
- シグネチャ：string? args(int index)
- 例：for (var i = 0; i < argc(); i++) {  println(args(i)); }

# 11.JSON 関数 (Rev.3)
## from_json(json)
- 説明： JSON 文字列からオブジェクトを作成する。
- 戻り値：オブジェクト
- シグネチャ： object from_json(string)
- 例： var obj = from_json("{\"name\":\"Tom\", \"age\":8}");

## to_json(obj)
- 説明：オブジェクトを JSON 文字列に変換する。
- 戻り値：文字列
- シグネチャ：string to_json(object)
- 例： var json = to_json(obj);

# 12. Python 組み込み関数 (Rev.3)
Python 組み込み関数のうち、よく使われるものと思われる以下の関数を実装する。
- dynamic abs(dynamic)
- dynamic ascii(dynamic)
- dynamic boolean(dynamic) (注意) Python では bool(x) であるが、C# は bool は予約されているため。
- dynamic chr(dynamic)
- dynamic hex(dynamic)
- dynamic len(dynamic)
- dynamic max(dynamic, dynamic)
- dynamic min(dynamic, dynamic)
- dynamic oct(dynamic)
- dynamic ord(dynamic)
- dynamic pow(dynamic, dynamic)
- dynamic range(dynamic, dynamic, dynamic)
- dynamic round(dynamic)
- dynamic sorted(dynamic)
- dynamic sum(dynamic)
- dynamic tuple(dynamic)
- dynamic type(dynamic)

# 13. Python 数学関数 (Rev.3)
Python math モジュールの関数のうち、よく使われるものと思われる以下の関数を実装する。
使う場合は、math. を付ける必要はない。
- dynamic ceil(dynamic)
- dynamic floor(dynamic)
- dynamic round(dynamic)
- dynamic fmod(dynamic, dynamic)
- dynamic sqrt(dynamic)
- dynamic isnan(dynamic)
- dynamic exp(dynamic)
- dynamic log(dynamic)
- dynamic log10(dynamic)
- dynamic degrees(dynamic)
- dynamic radians(dynamic)
- dynamic acos(dynamic)
- dynamic asin(dynamic)
- dynamic atan(dynamic)
- dynamic cos(dynamic)
- dynamic sin(dynamic)
- dynamic tan(dynamic)
- dynamic PI (定数)
- dynamic E (定数)


