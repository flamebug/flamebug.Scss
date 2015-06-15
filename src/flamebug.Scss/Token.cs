using System;
using System.Collections.Generic;
using System.Text;

namespace flamebug.Scss
{
    public class Token
	{
        public TokenType Type
        {
            get;
        }

        public string Value
        {
            get;
        }

        public Token(char value, TokenType type)
        {
            Value = value.ToString();
            Type = type;
        }

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        public Token(StringBuilder value, TokenType type)
        {
            Value = value.ToString();
            Type = type;
        }
    }

    public enum TokenType
    {
        Delimiter,
        Whitespace,
        String,
        Number,
        Hash,
        LineComment,
        BlockComment,
        Identifier,
        Dollar,
        LeftCurlyBracket,
        RightCurlyBracket,
        LeftSquareBracket,
        RightSquareBracket,
        LeftParenthesis,
        RightParenthesis,
        Equals,
        Plus,
        Hyphen,
        Asterisk,
        Comma,
        Period,
        SemiColon,
        Colon,
        Slash,
        Backslash,
        At,
        Percent,
        LessThan,
        GreaterThan,
        Tilde,
        Pipe,

        Eof

    }

}
