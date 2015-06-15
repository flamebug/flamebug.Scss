using System;
using System.Text;
using System.Collections.Generic;

namespace flamebug.Scss
{
	public class Root : Node
    {
		#region Node

		public override Node Clone()
		{
			throw new NotImplementedException();
		}

		public override string Compile()
		{
			var output = new StringBuilder();

			foreach (var node in Nodes)
			{
				output.Append(node.Compile());
			}

			return output.ToString().Trim();
		}
        
		#endregion

		#region Parse

		public static Root Parse(string content, Node parent, Environment env)
		{
			var root = new Root();

            root.Environment = env;
			root.Parent = parent;

			var buffer = new StringBuilder();

			var offset = 0;

			while (offset < content.Length)
			{
				switch (content[offset])
				{
                    // Beginning of double quote (find the end)
                    case '"':
                    	var dquote = Parser.FindEndOfQuote(content, offset, '"');
                    	buffer.Append(content.Substring(offset, dquote - offset + 1));
                    	offset = dquote + 1;
                    	continue;

                    // Beginning of single quote (find the end)
                    case '\'':
                    	var squote = Parser.FindEndOfQuote(content, offset, '\'');
                    	buffer.Append(content.Substring(offset, squote - offset + 1));
                    	offset = squote + 1;
                    	continue;

                    // RuleSet (MixinDefinition, RuleSet)
                    case '{':
                        var endblock = Parser.FindClosingCharacter(content, offset, '{', '}');
                        buffer.Append(content.Substring(offset, endblock - offset + 1));

                        ProcessRuleSet(buffer, root, env);
                        buffer.Clear();
                        offset = endblock + 1;
                        continue;

                    // Possible comment
                    case '/':

                        if ((offset + 1) < content.Length)
                        { 
                            // Line Comment
                            if (content[offset + 1] == '/')
                            {
                                ///
                                ///TODO: Fix this to call a different function, if comment begins with //// it gobbles up 2 newlines
                                /// 
                                var endcommentline = Parser.FindClosingString(content, offset, "//", "\n");
                                buffer.Append(content.Substring(offset, endcommentline - offset + 2));

                                ProcessCommentLine(buffer, root, env);
                                buffer.Clear();
                                offset = endcommentline + 1;
                            }
                            else if (content[offset + 1] == '*')
                            {
                                var endcommentblock = Parser.FindClosingString(content, offset, "/*", "*/");
                                buffer.Append(content.Substring(offset, endcommentblock - offset + 2));
                                
                                ProcessCommentBlock(buffer, root, env);
                                buffer.Clear();
                                offset = endcommentblock + 2;
                            }
                            else { 
                                buffer.Append(content[offset]);
                                offset++;
                            }
                        }
                        else
                        {
                            buffer.Append(content[offset]);
                            offset++;
                        }
                        continue;

                    // Declaration (Import, Mixin, Variable, Rule, Declaration)
                    case ';':
                        ProcessDeclaration(content, offset, buffer, root, env);
                        buffer.Clear();
                        offset++;
						continue;

                    default:
						buffer.Append(content[offset]);
                        offset++;
						break;
				}
			}

			return root;
		}

        #endregion

        #region Private Methods
        
		public static void ProcessDeclaration(string content, int offset, StringBuilder buffer, Node parent, Environment env)
		{
			var buf = buffer.ToString().Trim();

            //if (Import.IsMatch(buf))
            //{
            //	var import = Import.Parse(buf.Substring(7).Trim(), parent, path);

            //	parent.Nodes.Add(import);

            //	Node.CopyNodesToParent(import);
            //}

            //else if (Include.IsMatch(buf))
            //	parent.Nodes.Add(Include.Parse(buf, parent));

            //else if (VariableDefinition.IsMatch(buf))
            //	parent.Nodes.Add(VariableDefinition.Parse(buf, parent));

            /*else*/
            if (Rule.IsMatch(buf))
                parent.Nodes.Add(Rule.Parse(buf, parent, env));

            else
                throw new Exception("Declaration Not Found");
		}
        
		public static void ProcessRuleSet(StringBuilder buffer, Node parent, Environment env)
		{
            var buf = buffer.ToString().Trim();

            //if(Media.IsMatch(buf))
            //	parent.Nodes.Add(Media.Parse(buf, parent, path));

            //else if(Mixin.IsMatch(buf))
            //	parent.Nodes.Add(Mixin.Parse(buf, parent, path));

            //else
                parent.Nodes.Add(RuleSet.Parse(buf, parent, env));
		}

        public static void ProcessCommentBlock(StringBuilder buffer, Node parent, Environment env)
        {
            var buf = buffer.ToString().Trim();

            parent.Nodes.Add(CommentBlock.Parse(buf, parent, env));
        }

        public static void ProcessCommentLine(StringBuilder buffer, Node parent, Environment env)
        {
            var buf = buffer.ToString().Trim();

            parent.Nodes.Add(CommentLine.Parse(buf, parent, env));
        }
                
        #endregion
    }

}
