namespace GradeJudge.Server.Model;

// === リクエスト本体 ===

// 成績登録
public record RegisterRequest(int StudentId, int SubjectId, int Score);