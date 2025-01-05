using Rdx.Extensions;
using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Tokenizer;

public class RdxTokenizer(string jRdx)
{
    private int beginOfLastToken = 0;
    private int endOfLastToken = 0;

    public IEnumerable<RdxToken> Tokenize()
    {
        while (endOfLastToken < jRdx.Length)
        {
            var symbol = jRdx[endOfLastToken];
            foreach (var token in HandleSymbol(symbol))
            {
                yield return token;
            }

            endOfLastToken++;
        }
    }

    private IEnumerable<RdxToken> HandleSymbol(char symbol)
    {
        return symbol switch
        {
            '<' or '{' or '[' => AddValueTokenWithSpecialSymbol(RdxToken.OpeningBracket),
            '>' or '}' or ']' => AddValueTokenWithSpecialSymbol(RdxToken.ClosingBracket),
            ':' => AddValueTokenWithSpecialSymbol(RdxToken.Colon),
            ',' => AddValueTokenWithSpecialSymbol(RdxToken.Comma),
            '@' => AddValueTokenWithSpecialSymbol(RdxToken.TimestampMarker),
            ' ' => HandleSpace(),
            _ => []
        };

        IEnumerable<RdxToken> AddValueTokenWithSpecialSymbol(Func<int, RdxToken> factory)
        {
            var token = AddValueToken();
            if (token is not null)
            {
                yield return token;
            }
            
            yield return factory(endOfLastToken);
            
            beginOfLastToken = endOfLastToken + 1;
        }

        IEnumerable<RdxToken> HandleSpace()
        {
            var token = AddValueToken();
            beginOfLastToken = endOfLastToken + 1;
            return token?.AsEnumerable() ?? [];
        }
    }

    private RdxToken? AddValueToken()
    {
        return beginOfLastToken == endOfLastToken
            ? null
            : RdxToken.Value(beginOfLastToken, endOfLastToken - beginOfLastToken);
    }
}