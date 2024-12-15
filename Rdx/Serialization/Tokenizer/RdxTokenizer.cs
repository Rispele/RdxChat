using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Tokenizer;

public class RdxTokenizer
{
    public static List<RdxToken> Tokenize(string jRdx)
    {
        var tokens = new List<RdxToken>();
        var beginOfNewToken = 0;
        for (var i = 0; i < jRdx.Length; i++)
        {
            if (jRdx[i] == '<' || jRdx[i] == '{' || jRdx[i] == '[')
            {
                AddValueToken(i);
                tokens.Add(RdxToken.OpeningBracket(i));
            }

            if (jRdx[i] == '>' || jRdx[i] == '}' || jRdx[i] == ']')
            {
                AddValueToken(i);
                tokens.Add(RdxToken.ClosingBracket(i));
            }

            if (jRdx[i] == ':')
            {
                AddValueToken(i);
                tokens.Add(RdxToken.Colon(i));
            }

            if (jRdx[i] == ',')
            {
                AddValueToken(i);
                tokens.Add(RdxToken.Comma(i));
            }

            if (jRdx[i] == ' ')
            {
                AddValueToken(i);
            }
        }

        return tokens;

        void AddValueToken(int i)
        {
            if (beginOfNewToken != i)
            {
                tokens.Add(jRdx[beginOfNewToken] == '@'
                    ? RdxToken.Timestamp(beginOfNewToken, i - beginOfNewToken)
                    : RdxToken.Value(beginOfNewToken, i - beginOfNewToken));
            }

            beginOfNewToken = i + 1;
        }
    }
}