using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Regulus.Remoting.Unity
{
    public class AdsorberListenerAttribute : System.Attribute
    {
        public AdsorberListenerAttribute(Type adsorber, string target)
        {
            Adsorber = adsorber;
            
            Target = target;
        }

        
        public readonly Type Adsorber;        
        public readonly string Target;
    }
}
