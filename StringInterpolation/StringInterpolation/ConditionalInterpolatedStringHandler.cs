using System.Runtime.CompilerServices;

namespace StringInterpolation
{
    [InterpolatedStringHandler]
    public ref struct ConditionalInterpolatedStringHandler
    {
        private DefaultInterpolatedStringHandler _inner;

        public ConditionalInterpolatedStringHandler(
            int literalLength,
            int formattedCount,
            bool condition,
            out bool shouldAppend)
        {
            if (condition)
            {
                _inner = new(literalLength, formattedCount);
                shouldAppend = true;
            }

            _inner = default;
            shouldAppend = false;
        }

        public override string ToString()
        {
            return _inner.ToString();
        }

        public string ToStringAndClear()
        {
            return _inner.ToStringAndClear();
        }

        public void AppendLiteral(string literal)
        {
            _inner.AppendLiteral(literal);
        }

        public void AppendFormatted<T>(T message)
        {
            _inner.AppendFormatted(message);
        }
    }
}