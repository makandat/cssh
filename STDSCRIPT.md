 cssh 標準関数仕様書

# 1. 出力系関数
## print(object value)
説明：改行なしで値を標準出力へ書き込む
戻り値：なし
シグネチャ: print(object)
例：print("hello")
## println(object value)
説明：値を標準出力へ書き込み、末尾に改行を付ける
戻り値：なし
シグネチャ: println(object)
例：println("hello")
## printf(string format, params object[] args)
説明：フォーマット文字列に従って出力する。フォーマットはCスタイルだが、%d, %s, %f, #書式のみ対応する。
戻り値：なし
例：printf("name=%s age=%d", name, age)
## debug(object value)
説明：デバッグ用出力。出力先は標準エラー出力 (stderr) とする。
戻り値：なし
例：debug("x=" + x)

# 2. 文字列生成系関数
## format(string format, params object[] args)
説明：フォーマット文字列に従って文字列を生成して返す
戻り値：string
例：var msg = format("Hello {0}, age {1}", name, age); println(msg);

# 3. 入力系関数
## input(string prompt)
説明：プロンプトを表示し、ユーザーから 1 行入力を受け取るプロンプトの表示は Console.Write(prompt) で行い、入力文字列には最後の改行文字を含めない。
戻り値：string?
例：var name = input("name > ");
## gets()
説明：プロンプトなしで 1 行入力を受け取る
戻り値：string?
例：var line = gets();

# 4. 日付・時間系関数
# today()
説明：今日の日付を "yyyy-MM-dd" 形式で返す
戻り値：string
例：println(today());
# now()
説明：現在日時を ISO 8601 形式で返す
戻り値：string
例：println(now());

# datetime(DateTime? dt, string format)
説明：指定フォーマットで日時文字列を返す
戻り値：string
シグネチャ: string datetime(DateTime? dt = null, string format = "yyyy-MM-ddTHH:mm:ss");
例：println(date("yyyy/MM/dd HH:mm"));
println(datetime(new DateTime(2000, 1, 1), "yyyy-MM-dd"));

# 5. ファイル操作系関数
## read(string path)
説明：ファイル内容をすべて読み込んで返す。
戻り値：string
例：var text = read("data.txt");
## write(string path, string content)
説明：ファイルに内容を書き込む（上書き）
戻り値：なし
例：write("log.txt", "hello");
## append(string path, string content)
説明：ファイルに内容を追記する
戻り値：なし
例：append("log.txt", "more");
## exists(string path)
説明：ファイルの存在を確認する
戻り値：bool
例：if (exists("config.json")) { ... }

# 6. プロセス実行系関数
## system(string command)
説明：外部コマンドを実行し、標準出力を文字列として返す。(画面に表示されない)
戻り値：string
例：var result = system("ls -l");
## run(string command)
説明：外部コマンドを実行し、標準出力をそのまま流す。(画面に表示される)
戻り値：なし
例：run("echo hello");

# 7. 制御系関数
## exit(int code=0)
説明：指定した終了コードでプロセスを終了する
戻り値：なし（戻らない）
例：exit(1);
## abort(string message=””)
説明：例外を発生させて即座に終了する
戻り値：なし
例：abort("fatal error");

# 8. 正規表現系関数
## match(string text, string pattern)
説明：text が pattern にマッチするかどうかを返す
戻り値：bool
シグネチャ：bool match(string text, string pattern)
  例：if (match(name, "^[A-Z].*")) { ... }
## search(string text, string pattern)
説明：text の中で pattern に最初にマッチした部分文字列を返す。 マッチしない場合は null。
 戻り値：string?
 シグネチャ：string? search(string text, string pattern)
 例：var m = search(line, "[0-9]+");
## search(string text, string pattern)
説明：text の中で pattern に最初にマッチした部分文字列を返す。 マッチしない場合は null。
戻り値：string?
シグネチャ：string? search(string text, string pattern)
 例：var str = search(line, “[0-9]+”);
## replace(string text, string pattern, string replacement)
説明：正規表現に一致した部分を置換する
 戻り値：string
 シグネチャ：string replace(string text, string pattern, string replacement)
 例：var s = replace(line, "[0-9]+", "###");

# 9.コマンドライン引数系関数
## argc()
説明：コマンドライン引数の数を返す
 戻り値：int
 シグネチャ：int argc()
 例：println(argc());
## args(int index)
説明：指定したインデックスのコマンドライン引数を返す
 戻り値：string?（範囲外なら null）
 シグネチャ：string? args(int index)
 例：for (var i = 0; i < argc(); i++) {  println(args(i)); }
