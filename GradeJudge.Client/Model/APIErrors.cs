using System;
using System.Collections.Generic;
using System.Text;

namespace GradeJudge.Client.Model
{
    public class Error
    {
        public string Message { get; set; } = "";
    }

    public class ApiError
    {
        public List<Error> Errors { get; set; } = [];
    }
}
