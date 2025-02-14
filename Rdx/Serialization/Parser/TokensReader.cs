﻿using Rdx.Serialization.Tokenizer.Tokens;

namespace Rdx.Serialization.Parser;

public class TokensReader : IDisposable
{
    private readonly string source;
    private readonly List<RdxToken> tokens = [];
    private readonly IEnumerator<RdxToken> tokenSource;

    private int position;
    private Lazy<string> value = null!;

    public TokensReader(IEnumerable<RdxToken> tokenSource, string source)
    {
        this.tokenSource = tokenSource.GetEnumerator();
        this.source = source;

        RecreateLazy();
    }

    public void Dispose()
    {
        tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    public TokenType GetTokenType(int offset = 0)
    {
        return GetToken(position + offset)?.TokenType ?? throw new InvalidOperationException("Unable to read tokens.");
    }

    public bool HasToken(int offset = 0)
    {
        return GetToken(position + offset) != null;
    }

    public string GetValue(int offset = 0)
    {
        return offset == 0
            ? value.Value
            : GetToken(position + offset)?.GetValue(source)
         ?? throw new InvalidOperationException("Unable to read tokens.");
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

    private RdxToken? GetToken(int tokenPosition)
    {
        while (tokenPosition >= tokens.Count)
        {
            if (!tokenSource.MoveNext()) return null;

            tokens.Add(tokenSource.Current);
        }

        return tokens[tokenPosition];
    }

    private void RecreateLazy()
    {
        value = new Lazy<string>(() => GetToken(position).GetValue(source));
    }
}