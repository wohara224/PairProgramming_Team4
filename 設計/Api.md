# API仕様書

## メンバ

- 大原☆　・・・　クライアント側実装
- 越智孝　・・・　サーバー側実装、データ定義

## 必須要件

非同期処理
- クライアント：リクエスト送信後のレスポンス受信待ち

LINQ利用
- クライアント：科目別成績リストを受信後、成績上位5名を絞って高い順にランキング表示
- サーバー：生データ　→　レスポンス加工時

ロギング
- クライアント：アプリ動作
- サーバー：アプリ動作（システムログ）、通信内容（APIログ）

## ポイント

クライアント：
- 流れない見た目
- メイン画面とサブ画面に分割、ESCで戻る

サーバー：
- エラー処理の共通化
- Ctrl＋Cで正常終了させる ・・・　表示とログに終了を刻む
- JSONシリアライザの活用
- DBを想定したインターフェースの活用

## 処理の流れ

[POST] 成績登録
1. クライアントから登録情報をもらう
2. バリデーションチェック後にデータ登録し、結果を返す
    - 正常なリクエスト：200
    - POST以外のリクエスト：405
    - JSONが不正；400（INVALID_REQUEST）
    - 生徒が存在しない：400（STUDENT_NOT_EXIST）
    - 科目が存在しない：400（SUBJECT_NOT_EXIST）
    - 点数が範囲外：400（SCORE_OUT_OF_RANGE）

[GET] 個人別スコア取得
1. クライアントから生徒ID指定でリクエスト
2. 生徒IDに紐づくデータを返す
    - 正常なリクエスト：200（個人の持つ各科目のスコア配列）
    - POST以外のリクエスト：405
    - クエリ不正：400（INVALID_REQUEST）
    - 生徒が存在しない：404（STUDENT_NOT_FOUND）

[GET] 科目別成績リスト取得
1. クライアントから科目ID指定でリクエスト
2. 科目IDに紐づくデータを返す
    - 正常なリクエスト：200（その科目の全員分の結果を返す）
    - POST以外のリクエスト：405
    - IDが不正：400（INVALID_REQUEST）
    - 科目が存在しない：404（SUBJECT_NOT_FOUND）

その他のURLアクセス -> 404 Not Found

## API仕様

- 接続URL：http://xxx.xxx.xxx.xxx:8080/api
- 成績登録：/register
- 個人別スコア取得：/scores/student?id=*
- 科目別成績リスト取得：/scores/subject?id=*

## リクエストボディ

成績登録

[POST] /register
``` json
{
  "studentId": 1,
  "subjectId": 1,
  "score": 94
}
```

個人別スコア取得

[GET] /scores/student?id=1
``` json
{}
```

科目別成績リスト取得

[GET] /scores/subject?id=1
``` json
{}
```

## レスポンスボディ (正常系)

成績登録

/register
``` json
{}
```

個人別スコア取得

/scores/student?id=1
``` json
{
  "name": "田中",
  "subjects": [
    { "name": "数学", "score": 94 },
    { "name": "英語", "score": 47 },
    { "name": "物理", "score": 83 }
  ]
}
```

科目別成績リスト取得

/scores/subject?id=1
``` json
{
  "name": "数学",
  "students": [
    { "name": "山本", "score": 98 },
    { "name": "田中", "score": 94 },
    { "name": "柏", "score": 93 },
    { "name": "宇田", "score": 89 },
    { "name": "藤田", "score": 87 }
  ]
}
```

## レスポンスボディ （異常系）

成績登録

/register / POST以外のメソッドを受信した（405）
``` json
{}
```

/register / JSONがおかしい、変換できない（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/register / 生徒が存在しない（400）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_EXIST" }
  ]
}
```

/register / 科目が存在しない（400）
``` json
{
  "errors": [
    { "message": "SUBJECT_NOT_EXIST" }
  ]
}
```

/register / 点数が範囲外（400）
``` json
{
  "errors": [
    { "message": "SCORE_OUT_OF_RANGE" }
  ]
}
```

/register / パラメータ値異常の複合（400）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_EXIST" },
    { "message": "SCORE_OUT_OF_RANGE" }
  ]
}
``` 

個人別スコア取得

/scores/student?id=1 / GET以外のメソッドを受信した（405）
``` json
{}
```

/scores/student?id=abc / クエリがない、またはID数値変換失敗（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/scores/student?id=999 / 生徒が存在しない（404）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_FOUND" }
  ]
}
```

科目別成績リスト取得

/scores/subject?id=1 / GET以外のメソッドを受信した（405）
``` json
{}
```

/scores/subject?id=abc / クエリがない、またはID数値変換失敗（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/scores/subject?id=999 / 科目が存在しない（404）
``` json
{
  "errors": [
    { "message": "SUBJECT_NOT_FOUND" }
  ]
}
```

その他

URLが上記以外のもの（404）
``` json
{}
```

