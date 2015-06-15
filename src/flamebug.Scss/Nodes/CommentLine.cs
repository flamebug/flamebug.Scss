using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace flamebug.Scss
{
    public class CommentLine : Node
    {
        #region Public Properties

		/// <summary>
		/// Comment for the RuleSet
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		public int Length
		{
			get
			{
				return Value.Length;
			}
		}

        #endregion

		#region StyleNode

		public override Node Clone()
		{
			var comment = new CommentLine();

            comment.Environment = Environment;
            comment.Parent = Parent;
			comment.Nodes = Node.CloneNodes(this, comment);

			comment.Value = Value;

			return comment;
		}

		public static bool IsMatch(string buffer)
		{
			return new Regex(@"^//.*[\n]").IsMatch(buffer);
		}

		public override string Compile()
		{
			return string.Empty;
		}

		#endregion

		#region Parse

		public static CommentLine Parse(string buffer, Node parent, Environment env)
		{
			var comment = new CommentLine();

            comment.Environment = env;
            comment.Parent = parent;

			comment.Value = "";

			var match = new Regex(@"^//.*[\n]").Match(buffer);

			if(match.Success)
				comment.Value = match.Value;

			return comment;
		}

		#endregion
	}

}
