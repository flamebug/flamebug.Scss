using System;
using System.Collections.Generic;
using System.Text;

namespace flamebug.Scss
{
    public class Scanner
    {
        private string _content;
        private int _offset;
        private List<Token> _buffer = new List<Token>();

        public Scanner(string content)
        {
            _content = content;
            _offset = -1;
        }

        private char Read()
        {
            _offset++;

            // Return null character if EOF
            if (_offset >= _content.Length)
                return '\0';

            return _content[_offset];
        }

        private void Unread()
        {
            _offset--;
        }

        private char Peek(int count = 1)
        {
            // Return null character if EOF
            if (_offset + count >= _content.Length)
                return '\0';

            return _content[_offset + count];
        }

        public Token Scan()
        {
            var ch = Read();

            if (isEof())
                return new Token('\0', TokenType.Eof);

            if (isWhitespace(ch))
            {
                Unread();
                return new Token(ScanWhitespace(), TokenType.Whitespace);
            }

            if (isQuotationMark(ch))
            {
                Unread();
                return new Token(ScanString(ch), TokenType.String);
            }

            if (isLineComment(ch, Peek(1)))
            {
                Unread();
                return new Token(ScanLineComment(), TokenType.LineComment);
            }

            if (isBlockComment(ch, Peek(1)))
            {
                Unread();
                return new Token(ScanBlockComment(), TokenType.BlockComment);
            }

            if (IsIdentifier(ch, Peek(1), Peek(2)))
            {
                Unread();
                return new Token(ScanIdentifier(), TokenType.Identifier);
            }

            if (isDigit(ch))
            {
                Unread();
                return new Token(ScanNumber(), TokenType.Number);
            }

            switch (ch)
            {
                case '{':
                    return new Token(ch, TokenType.LeftCurlyBracket);
                case '}':
                    return new Token(ch, TokenType.RightCurlyBracket);
                case '[':
                    return new Token(ch, TokenType.LeftSquareBracket);
                case ']':
                    return new Token(ch, TokenType.RightSquareBracket);
                case '(':
                    return new Token(ch, TokenType.LeftParenthesis);
                case ')':
                    return new Token(ch, TokenType.RightParenthesis);
                case '=':
                    return new Token(ch, TokenType.Equals);
                case ':':
                    return new Token(ch, TokenType.Colon);
                case ';':
                    return new Token(ch, TokenType.SemiColon);
                case '#':
                    return new Token(ch, TokenType.Hash);
                case '.':
                    return new Token(ch, TokenType.Period);
                case '@':
                    return new Token(ch, TokenType.At);
                case '%':
                    return new Token(ch, TokenType.Percent);
                case '<':
                    return new Token(ch, TokenType.LessThan);
                case '>':
                    return new Token(ch, TokenType.GreaterThan);
                case '$':
                    return new Token(ch, TokenType.Dollar);
                case '~':
                    return new Token(ch, TokenType.Tilde);
                case '|':
                    return new Token(ch, TokenType.Pipe);
                case '+':
                    return new Token(ch, TokenType.Plus);
                case '-':
                    return new Token(ch, TokenType.Hyphen);
                case '*':
                    return new Token(ch, TokenType.Asterisk);
                case '/':
                    return new Token(ch, TokenType.Slash);
                case '\\':
                    return new Token(ch, TokenType.Backslash);
                case ',':
                    return new Token(ch, TokenType.Comma);
            }

            return new Token(ch, TokenType.Delimiter);
        }

        public StringBuilder ScanWhitespace()
        {
            var value = new StringBuilder();

            var ch = Read();

            while (!isEof() && isWhitespace(ch))
            {
                value.Append(ch);

                ch = Read();
            }

            Unread();

            return value;
        }

        public StringBuilder ScanString(char quotecharacter)
        {
            var value = new StringBuilder();

            var ch = Read();

            while (!isEof() && ch != quotecharacter)
            {
                value.Append(ch);

                ch = Read();
            }

            Unread();

            return value;
        }

        public StringBuilder ScanNumber()
        {
            var value = new StringBuilder();

            var ch = Read();

            while (!isEof() && (isDigit(ch) || (ch == '.' && isDigit(Peek()))))
            {
                value.Append(ch);

                ch = Read();
            }

            Unread();

            return value;
        }

        public StringBuilder ScanIdentifier()
        {
            // CSS name must begin with underscore, hyphen, or letter
            // CSS name may contain letters, digits, underscores, or hyphens
            // if the first letter is a hyphen the second letter must be a letter or underscore and must be at least 2 characters

            var value = new StringBuilder();

            var ch = Read();

            while (!isEof() && (isLetter(ch) || isDigit(ch) || ch == '_' || ch == '-'))
            {
                value.Append(ch);

                ch = Read();
            }

            Unread();

            return value;
        }

        public StringBuilder ScanLineComment()
        {
            var value = new StringBuilder();

            var ch = Read();

            while (!isEof() && !isNewLine(ch))
            {
                value.Append(ch);

                ch = Read();
            }

            Unread();

            return value;
        }

        public StringBuilder ScanBlockComment()
        {
            var value = new StringBuilder();
            var level = 0;

            var ch = Read();

            while (!isEof())
            {
                value.Append(ch);

                if (ch == '/' && Peek() == '*')
                    level++;

                if (ch == '*' && Peek() == '/')
                    level--;

                if (level == 0)
                    break;

                ch = Read();
            }

            ch = Read();    //read once more so we don't leak ending slashes

            value.Append(ch);

            return value;
        }

        public static int FindClosingCharacter(string content, int offset, char opencharacter, char closecharacter)
        {
            int i = offset;
            int length = content.Length;

            int level = 0;

            while (i < length)
            {
                char c = content[i];

                if (c == opencharacter) level++;

                if (c == closecharacter) level--;

                if (level == 0)
                {
                    break;
                }

                i++;
            }

            return i;
        }

        // 
        // Using token logic from http://www.w3.org/TR/css-syntax-3/
        // 

        private bool isEof()
        {
            return _offset > _content.Length;
        }

        public bool isDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private bool isHexDigit(char ch)
        {
            return isDigit(ch) || (ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F');
        }

        public bool isUpperCaseLetter(char ch)
        {
            return ch >= 'A' && ch <= 'Z';
        }

        public bool isLowerCaseLetter(char ch)
        {
            return ch >= 'a' && ch <= 'z';
        }

        public bool isLetter(char ch)
        {
            return isUpperCaseLetter(ch) || isLowerCaseLetter(ch);
        }

        public bool isNewLine(char ch)
        {
            return ch == '\n' || ch == '\r' || ch == '\f' || (ch == '\r' && Peek() == '\n');
        }

        public bool isQuotationMark(char ch)
        {
            return ch == '"' || ch == '\'';
        }

        public bool isWhitespace(char ch)
        {
            return ch == ' ' || ch == '\t' || isNewLine(ch);
        }

        public bool isLineComment(char ch1, char ch2)
        {
            return ch1 == '/' && ch2 == '/';
        }

        public bool isBlockComment(char ch1, char ch2)
        {
            return ch1 == '/' && ch2 == '*';
        }

        public bool isNameStart(char ch)
        {
            return isLetter(ch) || ch == '_';
        }

        public bool isEscape(char ch1, char ch2)
        {
            return ch1 == '\\' && ch2 != '\\';
        }

        public bool IsIdentifier(char ch1, char ch2, char ch3)
        {
            // CSS name must begin with underscore, hyphen, or letter
            // CSS name may contain letters, digits, underscores, or hyphens
            // if the first letter is a hyphen the second letter must be a letter or underscore and must be at least 2 characters

            if (ch1 == '-' && (isNameStart(ch2) || isEscape(ch2, ch3)))
                return true;

            if (isNameStart(ch1))
                return true;

            if (isEscape(ch1, ch2))
                return true;

            return false;
        }




    }
}
