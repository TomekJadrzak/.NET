﻿using Microsoft.CodeAnalysis;

namespace MonacoRoslynCompletionProvider.Api
{
    public class CodeCheckResult : IResponse
    {
        public CodeCheckResult() { }

        public virtual string Message { get; set; }

        public virtual int OffsetFrom { get; set; }

        public virtual int OffsetTo { get; set; }

        public virtual CodeCheckSeverity Severity { get; set; }

        public virtual int SeverityNumeric { get; set; }
    }
}