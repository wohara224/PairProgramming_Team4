using GradeJudge.Server.Model;

namespace GradeJudge.Server.Repository;

public interface IGradeRepository
{
    // 生徒存在確認
    bool StudentExists(int studentId);

    // 科目存在確認
    bool SubjectExists(int subjectId);

    // スコア確認
    bool IsValidScore(int score);

    // 成績登録
    void Register(int studentId, int subjectId, int score);

    // 個人別スコア取得
    StudentScoresResponse? GetStudentScores(int id);

    // 科目別成績リスト取得
    SubjectScoresResponse? GetSubjectScores(int id);
}
