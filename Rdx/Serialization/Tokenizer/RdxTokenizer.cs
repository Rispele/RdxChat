using Rdx.Extensions;
using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Tokenizer;

public class RdxTokenizer(string jRdx)
{
    private const char EndOfFileCharacter = (char)0x1A;
    
    private readonly string jRdxY = jRdx.Trim();
    private int beginOfLastToken;
    private int endOfLastToken;

    public IEnumerable<RdxToken> Tokenize()
    {
        var inString = false;
        var isPreviousSlash = false;
        for (;endOfLastToken < jRdx.Length; endOfLastToken++)
        {
            var symbol = jRdx[endOfLastToken];
            if (symbol == '\\')
            {
                isPreviousSlash = true;
                continue;
            }
            if (symbol == '\"')
            {
                if (isPreviousSlash)
                {
                    isPreviousSlash = false;
                    continue;
                }
                
                inString = !inString;
                continue;
            }
            if (inString)
            {
                continue;
            }
            
            foreach (var token in HandleSymbol(symbol)) yield return token;
        }

        if (beginOfLastToken != endOfLastToken)
        {
            foreach (var token in HandleSymbol(EndOfFileCharacter)) yield return token;
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
            EndOfFileCharacter => [AddValueToken()!],
            _ => []
        };

        IEnumerable<RdxToken> AddValueTokenWithSpecialSymbol(Func<int, RdxToken> factory)
        {
            var token = AddValueToken();
            if (token is not null) yield return token;

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