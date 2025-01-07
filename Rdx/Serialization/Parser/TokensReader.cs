using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class TokensReader : IDisposable
{
    private readonly IEnumerator<RdxToken> tokenSource;
    private readonly List<RdxToken> tokens = [];
    private readonly string source;

    private int position;
    private Lazy<string> value = null!;
    
    public TokensReader(IEnumerable<RdxToken> tokenSource, string source)
    {
        this.tokenSource = tokenSource.GetEnumerator();
        this.source = source;
        
        RecreateLazy();
    }

    public TokenType GetTokenType(int offset = 0)
    {
        return GetToken(position + offset).TokenType;
    }
    
    public string GetValue(int offset = 0)
    {
        return offset == 0 
            ? value.Value 
            : GetToken(position + offset).GetValue(source);
    }

    public string GetValueAndMoveNext()
    {
        var innerValue = GetValue();
        MoveNext();
        return innerValue;
    }

    public void MoveNext(int offset = 1)
    {
        position += offset;
        RecreateLazy();
    }

    private RdxToken GetToken(int tokenPosition)
    {
        while (tokenPosition >= tokens.Count)
        {
            if (!tokenSource.MoveNext())
            {
                throw new InvalidOperationException("Unable to read tokens.");
            }
            
            tokens.Add(tokenSource.Current);
        }

        return tokens[tokenPosition];
    }
    
    private void RecreateLazy() =>
        value = new Lazy<string>(() => GetToken(position).GetValue(source));

    public void Dispose()
    {
        tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}