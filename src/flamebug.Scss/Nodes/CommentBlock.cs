using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace flamebug.Scss
{
    public class CommentBlock : Node
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
			var comment = new CommentBlock();

            comment.Environment = Environment;
            comment.Parent = Parent;
			comment.Nodes = Node.CloneNodes(this, comment);

			comment.Value = Value;

			return comment;
		}

		public static bool IsMatch(string buffer)
		{
			return new Regex(@"^/\*[\s\S]*?\*/").IsMatch(buffer);
		}

		public override string Compile()
		{
			var output = new StringBuilder();

            if(!Environment.Minify)
            { 
			    output.Append(System.Environment.NewLine);

			    //if comment is part of a ruleset
			    if (Parent != null && Parent.Parent != null)
				    output.Append("\t");

			    output.Append(Value);
            }

            return output.ToString();
		}

		#endregion

		#region Parse

		public static CommentBlock Parse(string buffer, Node parent, Environment env)
		{
			CommentBlock comment = new CommentBlock();

            comment.Environment = env;
            comment.Parent = parent;

			comment.Value = "";

			var match = new Regex(@"^/\*[\s\S]*?\*/").Match(buffer);

			if(match.Success)
				comment.Value = match.Value;

			return comment;
		}

		#endregion
	}

}
