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
- 生徒：Student (Student)
- 科目：Subject (Subject)
- 点数：Score (int)

## リクエスト用DTO

成績登録：RegisterRequest
- 生徒ID：StudentId (int)
- 科目ID：SubjectId (int)
- 点数：Score (int)

個人別スコア：なし

科目別成績リスト：なし

## レスポンス用DTO

個人別スコア：StudentScoresResponse
- 生徒名：Name (string)
- 成績リスト：Subjects (List:SubjectScore)

科目個別の成績：SubjectScore
- 科目名：Name (string)
- 点数：Score (int)

科目別成績リスト：SubjectScoresResponse
- 科目名:Name (string)
- 生徒リスト：Students (List:StudentScore)

生徒別の成績：StudentScore
- 生徒名：Name (string)
- 点数：Score (int)

汎用エラー：ErrorResponse
  - エラーアイテム：Errors (ErrorItem)

エラーアイテム：ErrorItem
- エラー内容：Message (string)

## 画面構成

### 起動時

通常起動

``` bash
========================================
【成績管理スタブ】が起動しました
   ポート番号: 8080
========================================
ペアのPC（クライアント）からの通信を待っています...
```

localhost起動

``` bash
========================================
【成績管理スタブ】が起動しました
   ポート番号: 8080 (localhost)
========================================
ペアのPC（クライアント）からの通信を待っています...
```

### エラー時

起動失敗時

``` bash
サーバーが起動できませんでした。：Error=～～～～～～～～
アプリケーションを終了します．．．
```

### リクエスト受信時

通信待機中

``` bash
2026-06-05 16:18:26 192.168.100.24  GET     /api/scores/student?id=1        200 OK
2026-06-05 16:18:28 192.168.100.24  GET     /api/scores/student?id=43       404 NotFound
2026-06-05 16:18:18 192.168.100.24  GET     /api/scores/student?id=abc      400 BadRequest
2026-06-05 16:18:18 192.168.100.24  POST    /api/scores/student?id=abc      405 MethodNotAllowed
```

## ログ定義

- GradeJudge.Server.System：通信ログ
- GradeJudge.Server.Api：システムログ
- Microsoft.* / System.* は除外し、アプリケーション固有のみを対象とする

システムログ
``` text:system-20260605.log
2026-06-05 16:18:28.882|INFO|GradeJudge.Server.System|アプリケーション起動|
2026-06-05 16:18:29.591|FITAL|GradeJudge.Server.System|エラー: サーバー起動失敗|アクセスが拒否されました。
2026-06-05 16:18:29.689|INFO|GradeJudge.Server.System|アプリケーション終了：Exit=1|
2026-06-05 16:29:47.904|INFO|GradeJudge.Server.System|アプリケーション起動|
2026-06-05 16:29:47.992|INFO|GradeJudge.Server.System|リスナー起動：Port 8080|
```

通信ログ
``` text:api-20260608.log
2026-06-08 09:12:17.1020|INFO|GradeJudge.Server.Api|リクエスト受信：API=/api/ Method=GET|
2026-06-08 09:12:17.2183|WARN|GradeJudge.Server.Api|リクエスト異常：URL不正|
2026-06-08 09:12:17.2591|INFO|GradeJudge.Server.Api|レスポンス送信：Status=405|
2026-06-08 09:13:17.7008|INFO|GradeJudge.Server.Api|リクエスト受信：API=/api/performance Method=GET|
2026-06-08 09:13:17.7255|WARN|GradeJudge.Server.Api|リクエスト異常：クエリ不正 Query=|
2026-06-08 09:13:17.7255|INFO|GradeJudge.Server.Api|レスポンス送信：Status=400|
2026-06-08 09:13:34.2450|INFO|GradeJudge.Server.Api|リクエスト受信：API=/api/performance?student=1 Method=GET|
2026-06-08 09:13:34.2450|WARN|GradeJudge.Server.Api|リクエスト異常：クエリ不正 Query=?student=1|
2026-06-08 09:13:34.2450|INFO|GradeJudge.Server.Api|レスポンス送信：Status=400|
2026-06-08 09:13:43.8865|INFO|GradeJudge.Server.Api|リクエスト受信：API=/api/performance?studentId=1 Method=GET|
2026-06-08 09:13:43.8865|WARN|GradeJudge.Server.Api|リクエスト異常：クエリ不正 Query=?studentId=1|
2026-06-08 09:13:43.8865|INFO|GradeJudge.Server.Api|レスポンス送信：Status=400|
2026-06-08 09:14:08.3664|INFO|GradeJudge.Server.Api|リクエスト受信：API=/api/performance?id=1 Method=GET|
2026-06-08 09:14:08.4014|INFO|GradeJudge.Server.Api|リクエスト正常|
2026-06-08 09:14:08.4014|INFO|GradeJudge.Server.Api|レスポンス送信：Status=200|
```


