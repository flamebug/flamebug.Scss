using System;
using System.IO;

namespace flamebug.Scss
{
    public static class Parser
    {
        #region Public Methods

        public static string Parse(string file)
        {
            var env = new Environment();

            if (!File.Exists(file))
                throw new FileNotFoundException("Filename does not exist: " + file);

            env.Paths.Add(new FileInfo(file).DirectoryName);

            Console.WriteLine("Parsing File: " + file);

            return Parse(File.ReadAllText(file), env);
        }

        public static string Parse(string content, string path)
		{
            var env = new Environment();

            env.Paths.Add(path);

            return Parse(content, env);
        }

        public static string Parse(string content, Environment env)
        {
            var scanner = new Scanner(content);

            var token = scanner.Scan();

            while (token.Type != TokenType.Eof)
            {
                Console.WriteLine(token.Type + " : " + token.Value);
                //Console.Write(token.Value);

                token = scanner.Scan();
            }

            return "testing";
            //var root = Root.Parse(content, null, env);

            //root.Evaluate();
            //root.Unstack();

            //return root.Compile();
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

        public static int FindClosingString(string content, int offset, string open, string close)
        {
            int i = offset;
            int length = content.Length;
            int olength = open.Length;
            int clength = close.Length;

            int level = 0;

            while (i < length)
            {
                if (((i + olength) < length) && content.Substring(i, olength) == open)
                {
                    level++;
                    i += olength;
                    continue;
                }

                if (((i + clength) < length) && content.Substring(i, clength) == close)
                {
                    level--;

                    if (level == 0)
                        return i;

                    i += clength;
                    continue;
                }

                i++;
            }

            return -1;
        }

        public static int FindEndOfQuote(string content, int offset, char quotecharacter)
		{
			int quote = content.IndexOf(quotecharacter, offset + 1);

			if (quote < 0)
				throw new Exception("Missing end of quote");

			return quote;
		}

		#endregion
	}
}
