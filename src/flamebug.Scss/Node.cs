using System;
using System.Collections.Generic;

namespace flamebug.Scss
{
    public abstract class Node
	{
        #region Public Properties

        /// <summary>
        /// Environment
        /// </summary>
        
        public Environment Environment
        {
            get;
            set;
        }

        /// <summary>
        /// Parent Node
        /// </summary>
        
        public Node Parent
		{
			get;
			set;
		}

		/// <summary>
		/// List of Sub-Nodes
		/// </summary>
        
		public List<Node> Nodes
		{
			get;
			set;
		}

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>

        public Node()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Clone the node
        /// </summary>
        
        public abstract Node Clone();

        /// <summary>
        /// Compile to the output format
        /// </summary>

        public abstract string Compile();

        /// <summary>
        /// Evaluate the node
        /// </summary>

        public virtual void Evaluate()
        {
            if (Nodes == null)
                return;

            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                Nodes[i].Evaluate();
            }
        }

        /// <summary>
        /// Unstack the node
        /// </summary>
        
        public virtual void Unstack()
		{
            if (Nodes == null)
                return;

			for (int i = Nodes.Count - 1; i >= 0; i--)
			{
                Nodes[i].Unstack();
			}
		}

        /// <summary>
        /// Copy nodes to the node's parent
        /// </summary>

        protected static void CopyNodesToParent(Node node)
		{
			var index = node.Parent.Nodes.IndexOf(node);

			if (node.Nodes != null)
			{
				for (int i = node.Nodes.Count - 1; i >= 0; i--)
				{
					var current = node.Nodes[i];

					current.Parent = node.Parent;

					node.Parent.Nodes.Insert(index, current);
				}
			}

			node.Parent.Nodes.Remove(node);
		}

        /// <summary>
        /// Clone the nodes to the parent
        /// </summary>

        protected static List<Node> CloneNodes(Node node, Node parent)
		{
			List<Node> nodes = new List<Node>();

			if (node.Nodes != null)
			{
				foreach (Node subnode in node.Nodes)
				{
					Node newnode = subnode.Clone();

					newnode.Parent = parent;

					nodes.Add(newnode);
				}
			}

			return nodes;
		}
        #endregion
    }

}
