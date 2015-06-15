using System;
using System.Collections.Generic;

namespace flamebug.Scss
{
    public class Environment
    {
        /// <summary>
        /// Paths to search for imports
        /// </summary>
        
        public List<string> Paths
        {
            get;
            set;
        }

        /// <summary>
        /// Minify Output
        /// </summary>

        public bool Minify
        {
            get;
            set;
        }

        /// <summary>
        /// Contructor
        /// </summary>

        public Environment()
        {
            Paths = new List<string>();
            Minify = false;
        }

    }
}
