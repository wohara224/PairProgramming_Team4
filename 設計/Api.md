# API仕様書

## 処理の流れ

[初期]サーバー開設
1. 通信立上げ
2. クライアントリクエスト待ち受け

[POST] 成績登録
1. JSONから登録情報を受け取る
2. バリデーションOKなら200、NGなら400
    - 正常なリクエスト：200（追加しました）
    - 正常で既存データがあった：200（更新しました）
    - JSONが不正；400エラー（リクエストが不正です）
    - 生徒が存在しない：400エラー（存在しない生徒が指定されました）
    - 科目が存在しない：400エラー（存在しない科目が指定されました）
    - 点数が範囲外：400エラー（点数は0-100の範囲です）

[GET] 個人別スコア取得
1. JSON内から付加情報を取り出す
2. 生徒IDをもらってデータを返す
    - 正常なリクエスト：200（全科目分返す）
    - IDが不正：400エラー（生徒IDが不正です）
    - 生徒が存在しない：404エラー（生徒ID:**は存在しません）

[GET] 科目別ランキング取得
1. JSON内から付加情報を取り出す
2. 科目に従って上位5名分のデータを返す。
    - 正常なリクエスト：200（5名分のその科目の結果を返す）
    - IDが不正：400エラー（科目IDが不正です）
    - 科目が存在しない：404エラー（科目ID:**は存在しません）

その他のURLアクセス -> 403 Forbiddenを返却

## API仕様

- 接続URL：http://xxx.xxx.xxx.xxx:8080/api
- 成績登録：/register
- 個人別スコア取得：/scores?id=*
- 科目別ランキング取得：/ranking?id=*

## リクエストボディ

成績登録

[POST] /api/register
``` json
{
  "studentId": 1,
  "subjectId": 1,
  "score": 94
}
```

個人別スコア取得

[GET] /api/scores?id=1
``` json
{}
```

科目別ランキング取得

[GET] /api/ranking?id=1
``` json
{}
```

## レスポンスボディ (正常系)

成績登録

/api/register
``` json
{}
```

個人別スコア取得

/api/scores?id=1
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

科目別ランキング取得

/api/ranking?id=1
``` json
{
  "subject": "数学",
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

/api/register / POST以外のメソッドを受信した（405）
``` json
{}
```

/api/register / JSONがおかしい、変換できない（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/api/register / 生徒が存在しない（400）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_EXIST" }
  ]
}
```

/api/register / 科目が存在しない（400）
``` json
{
  "errors": [
    { "message": "SUBJECT_NOT_EXIST" }
  ]
}
```

/api/register / 点数が範囲外（400）
``` json
{
  "errors": [
    { "message": "SCORE_OUT_OF_RANGE" }
  ]
}
```

/api/register / パラメータ値異常の複合（400）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_EXIST" },
    { "message": "SCORE_OUT_OF_RANGE" }
  ]
}
``` 

/api/register / POST以外のメソッドを受信した（405）
``` json
{}
```

個人別スコア取得

/api/scores?id=1 / GET以外のメソッドを受信した（405）
``` json
{}
```

/api/scores?id=abc / クエリがない、またはID数値変換失敗（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/api/scores?id=999 / 生徒が存在しない（404）
``` json
{
  "errors": [
    { "message": "STUDENT_NOT_FOUND" }
  ]
}
```

科目別ランキング取得

/api/ranking?id=1 / GET以外のメソッドを受信した（405）
``` json
{}
```

/api/ranking?id=abc / クエリがない、またはID数値変換失敗（400）
``` json
{
  "errors": [
    { "message": "INVALID_REQUEST" }
  ]
}
```

/api/ranking?id=999 / 科目が存在しない（404）
``` json
{
  "errors": [
    { "message": "SUBJECT_NOT_FOUND" }
  ]
}
```

その他

URLが上記以外のもの（403）
``` json
{}
```

