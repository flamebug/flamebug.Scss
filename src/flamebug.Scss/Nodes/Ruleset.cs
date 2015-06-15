using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace flamebug.Scss
{
    //
    // Ruleset:
    //
    // selector,
    // selector {
    //      display: none;          <<-- Rule
    // }
    //
    //
    //

	public class RuleSet : Node, IComparable<RuleSet>
	{
        #region Public Properties

        /// <summary>
        /// Selectors
        /// </summary>

        public List<string> Selectors
		{
			get;
			set;
		}

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>

        public RuleSet()
        {
            Selectors = new List<string>();
        }

		#region StyleNode

		public override Node Clone()
		{
			RuleSet ruleset = new RuleSet();

            ruleset.Environment = Environment;
			ruleset.Parent = Parent;
			ruleset.Nodes = Node.CloneNodes(this, ruleset);

            ruleset.Selectors = Selectors;

			return ruleset;
		}

		public override void Unstack()
		{
			if (Parent != null && Parent.Parent != null)
			{
				int index = Parent.Nodes.IndexOf(this);
				int parentindex = Parent.Parent.Nodes.IndexOf(Parent);

				//only copy the node upwards if the parent is a ruleset
				if (Parent is RuleSet)
				{
					RuleSet parentruleset = Parent as RuleSet;

					Selectors = CombineSelectors(parentruleset.Selectors, Selectors);
			
					Parent.Parent.Nodes.Insert(parentindex + 1, this);
					Parent.Nodes.RemoveAt(index);
					Parent = Parent.Parent;
				}
			}

			base.Unstack();
		}

		public override string Compile()
		{
            if (Nodes.Count == 0)
                return string.Empty;

            var output = new StringBuilder();

            if (Environment.Minify) {

				output.Append(string.Join(",", Selectors));
				output.Append("{");

				foreach (Node node in Nodes)
				{
					output.Append(node.Compile());
				}

                output.Append("}");

            }
            else
            {

                output.Append(System.Environment.NewLine);
                output.Append(string.Join("," + System.Environment.NewLine, Selectors));
                output.Append(" {");

                foreach (Node node in Nodes)
                {
                    output.Append(node.Compile());
                }

                output.Append(System.Environment.NewLine);
                output.Append("}");

            }

			return output.ToString();
		}
        
		#endregion

		#region Parse

		public static RuleSet Parse(string buffer, Node parent, Environment env)
		{
			var ruleset = new RuleSet();

            ruleset.Environment = env;
			ruleset.Parent = parent;

			int brace = buffer.IndexOf('{');
			string selector = buffer.Remove(brace).Trim();

			ruleset.Selectors = new List<string>();

			foreach (var sel in selector.Split(','))
			{
				ruleset.Selectors.Add(sel.Trim());
			}

			ruleset.Nodes = (Root.Parse(buffer.Substring(brace + 1, buffer.Length - brace - 2), ruleset, env)).Nodes;

			foreach (var node in ruleset.Nodes)
			{
				node.Parent = ruleset;
			}

			return ruleset;
		}

		private static List<string> CombineSelectors(List<string> parent, List<string> child)
		{
			var combined = new List<string>();

			foreach (var parentSelector in parent)
			{
				foreach (var childSelector in child)
				{
					if (childSelector.Contains("&"))
					{
						combined.Add(childSelector.Replace("&", parentSelector));
					}
					else
					{
						combined.Add(parentSelector + " " + childSelector);
					}
				}
			}

			return combined;
		}

		#endregion

		#region IComparable

		public int CompareTo(RuleSet other)
        {
            return 0;
			//return this.Name.CompareTo(other.Name);  ///TODO: FIX THIS TO USE SELECTOR LIST
        }

		public bool Equals(RuleSet other)
        {
            return false;
			//return (this.Name == other.Name);        ///TODO: FIX THIS TO USE SELECTOR LIST
        }

        #endregion
    }

}
