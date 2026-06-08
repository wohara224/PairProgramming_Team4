namespace GradeJudge.Server.Model;

// 科目
public record Subject(int Id, string Name);

// 生徒
public record Student(int Id, string Name);

// 成績
public record TestResult(int Id, Student Student, Subject Subject, int Score);