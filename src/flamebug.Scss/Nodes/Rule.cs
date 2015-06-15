using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace flamebug.Scss
{
    public class Rule : Node
    {
        #region Public Properties

        /// <summary>
        /// Property for the rule
        /// </summary>
        public string Property
        {
			get;
			set;
        }

        /// <summary>
        /// Value for the rule
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        #endregion

        #region StyleNode

        public override Node Clone()
		{
			Rule rule = new Rule();

            rule.Environment = Environment;
			rule.Parent = Parent;
			//rule.Nodes = Node.CloneNodes(this, rule);

			rule.Property = Property;
            rule.Value = Value;

			return rule;
		}

		public static bool IsMatch(string buffer)
		{
			return new Regex(@"[\w].*:[\s\S]*?").IsMatch(buffer);
		}

		public override string Compile()
		{
			StringBuilder output = new StringBuilder();

            if (Environment.Minify)
            {
			    output.Append(Property);
			    output.Append(":");
                output.Append(Value);
                output.Append(";");
            }
            else
            {
                output.Append(System.Environment.NewLine);
                output.Append("\t");
                output.Append(Property);
                output.Append(":");
                output.Append(Value);
                output.Append(";");
            }

            return output.ToString();
		}

		#endregion

		#region Parse

		public static Rule Parse(string buffer, Node parent, Environment env)
		{
			Rule rule = new Rule();

            rule.Environment = env;
			rule.Parent = parent;

			int colon = buffer.IndexOf(':');

			rule.Property = buffer.Substring(0, colon).Trim();

            rule.Value = buffer.Substring(colon + 1).Trim();

			return rule;
		}

		#endregion
	}

}
