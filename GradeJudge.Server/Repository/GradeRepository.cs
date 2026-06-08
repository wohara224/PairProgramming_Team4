using GradeJudge.Server.Model;

namespace GradeJudge.Server.Repository;

internal class GradeRepository : IGradeRepository
{
    // === テスト用インメモリデータ ===

    // 科目リスト
    private readonly List<Subject> _testSubjects = [
        new(1,"数学"),
        new(2,"英語"),
        new(3,"物理")];

    // 生徒リスト
    private readonly List<Student> _testStudents = [
        new(1, "氏原 優大"),
        new(2, "大迫 信一"),
        new(3, "松田 純一"),
        new(4, "笠石 一夫"), 
        new(5, "高橋 重治"),
        new(6, "渡部 誠"),
        new(7, "大原 七海"),
        new(8, "江尻 聖士"),
        new(9, "千島 ちひろ"),
        new(10, "佐田 資仁"),
        new(11, "和希 真澄"),
        new(12, "三上 奈央"),
        new(13, "柳澤 弘美"),
        new(14, "真田 基嗣"),
        new(15, "宮本 正夫"),
        new(16, "徳重 瀬奈"),
        new(17, "大﨑 利治"),
        new(18, "小坂 ゆう"),
        new(19, "月足 剛基"),
        new(20, "後藤 健"),
        new(21, "佐々木 小百合"),
        new(22, "名取 達哉"),
        new(23, "三好 春雄"),
        new(24, "中森 健二"),
        new(25, "池野 樹"),
        new(26, "白鳥 慎"),
        new(27, "高江洲 英樹"),
        new(28, "迫丸 修"),
        new(29, "浜田 じゅん子"),
        new(30, "長冨 英之"),
        new(31, "飯沼 英之"),
        new(32, "希美 麗来"),
        new(33, "上野 千尋"),
        new(34, "嶋田 勝喜"),
        new(35, "三木 勝"),
        new(36, "北爪 大己"),
        new(37, "町田 凪"),
        new(38, "大村 部愛菜"),
        new(39, "原田 ルミ子"),
        new(40, "出井 愛")];

    // === 変数 ===

    // テスト成績
    private readonly List<TestResult> _results;


    // === メソッド ===

    // コンストラクタ
    public GradeRepository()
    {
        // 仮テスト成績の作成
        _results = [
            new(37, _testStudents[0],  _testSubjects[0], 94),
            new(12, _testStudents[1],  _testSubjects[1], 88),
            new(58, _testStudents[2],  _testSubjects[2], 91),
            new(4,  _testStudents[3],  _testSubjects[0], 85),
            new(29, _testStudents[4],  _testSubjects[1], 97),
            new(63, _testStudents[5],  _testSubjects[2], 83),
            new(18, _testStudents[6],  _testSubjects[0], 89),
            new(45, _testStudents[7],  _testSubjects[1], 92),
            new(7,  _testStudents[8],  _testSubjects[2], 86),
            new(52, _testStudents[9],  _testSubjects[0], 81),
            new(24, _testStudents[10], _testSubjects[1], 95),
            new(61, _testStudents[11], _testSubjects[2], 84),
            new(33, _testStudents[12], _testSubjects[0], 90),
            
            new(9,  _testStudents[13], _testSubjects[1], 78),
            new(55, _testStudents[14], _testSubjects[2], 74),
            new(20, _testStudents[15], _testSubjects[0], 71),
            new(43, _testStudents[16], _testSubjects[1], 76),
            new(2,  _testStudents[17], _testSubjects[2], 79),
            new(49, _testStudents[18], _testSubjects[0], 73),
            new(14, _testStudents[19], _testSubjects[1], 75),
            new(60, _testStudents[20], _testSubjects[2], 72),
            new(26, _testStudents[21], _testSubjects[0], 77),
            new(41, _testStudents[22], _testSubjects[1], 74),
            new(5,  _testStudents[23], _testSubjects[2], 70),
            new(54, _testStudents[24], _testSubjects[0], 78),
            new(17, _testStudents[25], _testSubjects[1], 76),
            new(47, _testStudents[26], _testSubjects[2], 73),
            new(11, _testStudents[27], _testSubjects[0], 79),
            new(57, _testStudents[28], _testSubjects[1], 71),
            new(22, _testStudents[29], _testSubjects[2], 75),
            new(64, _testStudents[30], _testSubjects[0], 77),
            new(31, _testStudents[31], _testSubjects[1], 72),
            new(50, _testStudents[32], _testSubjects[2], 74),
            new(15, _testStudents[33], _testSubjects[0], 78),
            new(44, _testStudents[34], _testSubjects[1], 73),
            new(8,  _testStudents[35], _testSubjects[2], 76),
            new(53, _testStudents[36], _testSubjects[0], 70),
            new(27, _testStudents[37], _testSubjects[1], 79),
            new(62, _testStudents[38], _testSubjects[2], 74),
            
            new(3,  _testStudents[39], _testSubjects[0], 68),
            new(40, _testStudents[0],  _testSubjects[1], 65),
            new(16, _testStudents[1],  _testSubjects[2], 62),
            new(59, _testStudents[2],  _testSubjects[0], 67),
            new(23, _testStudents[3],  _testSubjects[1], 64),
            new(48, _testStudents[4],  _testSubjects[2], 69),
            new(10, _testStudents[5],  _testSubjects[0], 61),
            new(51, _testStudents[6],  _testSubjects[1], 66),
            new(19, _testStudents[7],  _testSubjects[2], 63),
            new(65, _testStudents[8],  _testSubjects[0], 68),
            new(28, _testStudents[9],  _testSubjects[1], 60),
            new(42, _testStudents[10], _testSubjects[2], 65),
            new(13, _testStudents[11], _testSubjects[0], 67),
            new(56, _testStudents[12], _testSubjects[1], 62),
            new(21, _testStudents[13], _testSubjects[2], 69),
            new(46, _testStudents[14], _testSubjects[0], 64),
            new(6,  _testStudents[15], _testSubjects[1], 66),
            new(39, _testStudents[16], _testSubjects[2], 61),
            new(25, _testStudents[17], _testSubjects[0], 68),
            
            new(1,  _testStudents[18], _testSubjects[1], 55),
            new(34, _testStudents[19], _testSubjects[2], 48),
            new(30, _testStudents[20], _testSubjects[0], 59),
            new(38, _testStudents[21], _testSubjects[1], 42),
            new(35, _testStudents[22], _testSubjects[2], 57),
            new(32, _testStudents[23], _testSubjects[0], 51),
            new(36, _testStudents[24], _testSubjects[1], 38)
            ];


    }

    // 生徒存在確認
    public bool StudentExists(int studentId) =>
        _testStudents.Any(x => x.Id == studentId);

    // 科目存在確認
    public bool SubjectExists(int subjectId) =>
        _testSubjects.Any(x => x.Id == subjectId);

    // スコア確認
    public bool IsValidScore(int score) =>
        score is >= 0 and <= 100;

    // 成績登録
    public void Register(int studentId, int subjectId, int score)
    {
        var student = _testStudents.FirstOrDefault(x => x.Id == studentId);
        var subject = _testSubjects.FirstOrDefault(x => x.Id == subjectId);

        // バリデーションチェック
        if (student is null || subject is null || !IsValidScore(score))
            throw new ArgumentException($"引数異常 student={studentId} subject={subjectId} score={score}");

        int index = _results.FindIndex(r => r.Student == student && r.Subject == subject);
        if (index < 0)
        {
            int nextId = _results.Count > 0 ? _results.Max(x => x.Id) + 1 : 1;
            TestResult newResult = new(nextId, student, subject, score);
            _results.Add(newResult);
        }
        else
        {
            int nextId = _results[index].Id;
            TestResult newResult = new(nextId, student, subject, score);
            _results[index] = newResult;
        }
    }

    // 個人成績取得
    public PerformanceResponse? GetPersonalPerformance(int id)
    {
        // 指定IDの生徒は存在するか？
        var target = _testStudents.FirstOrDefault(x => x.Id == id);
        if (target is null)
        {
            return null;
        }

        var scores =
            _results
                .Where(x => x.Student == target) // 指定生徒に絞った成績リスト
                .Select(x=>new SubjectScore(x.Subject.Name, x.Score)); // 科目別成績に加工

        // 成績0件なら空リストを返す（nullではない）
        return new(target.Name, [.. scores]);
    }
}
