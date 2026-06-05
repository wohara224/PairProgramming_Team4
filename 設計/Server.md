# サーバー設計書

## 環境

Nugetパッケージ

- NLog : ログ出力
- NLog.Schema : ログ設定時のインテリセンス（候補入力）

## データ定義

科目：Subject

- 科目番号：Id (int)
- 名前：Name (string)

生徒：Student

- 生徒番号：Id (int)
- 名前：Name (string)

成績：TestResult

- 管理番号：Id (int)
- 生徒：StudentInfo (Student)
- 科目：Subject (Subject)
- 点数：Score (int)

## リクエスト用DTO

/api/register -> RegisterRequest
- 生徒ID：Student (int)
- 科目ID：Subject (int)
- 点数：Score (int)

## レスポンス用DTO

/api/register -> なし

/api/performance?student=1 -> PerformanceResponse
- 生徒名：Name (string)
- 成績リスト：Subjects (List:SubjectScore)
  - SubjectScore
    - 科目名：Name (string)
    - 点数：Score (int)

/api/dropout -> DropoutResponse
- 落第者リスト：DropoutStudents (List:DropoutStudent)
  - DropoutStudent
    - 名前：Name (string)
    - 落第科目：Subjects <List<SubjectScore>>

/api/ranking?subject=1 -> RankingResponse
- 科目名:Subject (string)
- 生徒リスト：Students (List:StudentScore)
  - StudentScore
    - 名前：Name (string)
    - 点数：Score (int)

汎用エラーオブジェクト -> ErrorResponse
- エラーの種類：Error (string)
- エラーコード：Code (string)

## 画面構成

### ヘッダー

起動時

``` bash
========================================
  Server : Starting ...
  Port   : -
========================================
```

サーバー起動成功

``` bash
========================================
  Server : [ OK ] Started
  Port   : 50080  
========================================
```

ローカルホスト起動

``` bash
========================================
  Server : [WARN] Local only
  Port   : 50080 
========================================
```

サーバー起動失敗

``` bash
========================================
  Server : [FAIL] Unavailable
  Port   : -
========================================
```

### ビジネス中

通信待機中

``` bash
========================================
  Server : [ OK ] Started
  Port   : 50080  
========================================
Wait for client request...





- History -
No connection logs found.
```

通信確立 → 200 レスポンス送信

``` bash
========================================
  Server : [ OK ] Started
  Port   : 50080  
========================================
Connected.
Connection  : 192.168.100.24
GET request : register
HTTP Status : 200 OK
Disconnected.

- History -
2026-06-05 16:18:18 192.168.100.24  register                200 OK
```

通信確立 → 400 レスポンス送信

``` bash
========================================
  Server : [ OK ] Started
  Port   : 50080  
========================================
Connected.
Connection  : 192.168.100.24
GET request : register
HTTP Status : 400 Bad Request
Disconnected.

- History -
2026-06-05 19:23:45 192.168.100.24  register                400 Bad Request
```

1件以上通信履歴があるとき

``` bash
========================================
  Server : [ OK ] Started
  Port   : 50080  
========================================
Wait for client request...





- History -
2026-06-05 16:18:18 192.168.100.24  register                200 OK
2026-06-05 16:18:26 192.168.100.24  performance?student=2   404 Not Found
```

## ログ定義

- System.*、Microsoft.*系は除外する
- 通信ログは、*Api.*を対象に出力する
- システムログは、上記以外を対象とする

通信ログ
``` text:api-20260605.log
2026-06-05 16:18:29.123|Info|GradeJudge.Server.Api.ResponseCreator|Listener起動 PORT:12380
2026-06-05 16:18:29.349|Error|GradeJudge.Server.Api.ResponseCreator|エラー: アクセスが拒否されました。
2026-06-05 16:18:29.443|Info|GradeJudge.Server.Api.ResponseCreator|Listener終了
2026-06-05 16:29:48.333|Info|GradeJudge.Server.Api.ResponseCreator|Listener起動 PORT:50080
2026-06-05 16:29:48.587|Info|GradeJudge.Server.Api.ResponseCreator|Listener起動完了
2026-06-05 16:30:28.912|Info|GradeJudge.Server.Api.ResponseCreator|通信確立 192.168.100.24
2026-06-05 16:30:29.123|Info|GradeJudge.Server.Api.ResponseCreator|GETリクエスト受信 register
2026-06-05 16:30:29.578|Warn|GradeJudge.Server.Api.ResponseCreator|リクエスト不正: 生徒ID:999は存在しません。
2026-06-05 16:30:29.843|Info|GradeJudge.Server.Api.ResponseCreator|レスポンス送信 400 Bad Request
```

システムログ
``` text:system-20260605.log
2026-06-05 16:18:28.882|Info|GradeJudge.Server.Program|アプリケーション起動
2026-06-05 16:18:29.591|Error|GradeJudge.Server.Program|エラー: サーバー確立失敗
2026-06-05 16:18:29.689|Info|GradeJudge.Server.Program|アプリケーション終了 ExitCode=1
2026-06-05 16:29:47.904|Info|GradeJudge.Server.Program|アプリケーション起動
```

