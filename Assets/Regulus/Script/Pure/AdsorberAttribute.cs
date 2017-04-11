using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Regulus.Remoting.Unity
{
    public class ExpressionAdsorberAttribute
    {
        
    }
    public class TExpressionAdsorberAttribute<T> : ExpressionAdsorberAttribute
    {
        public TExpressionAdsorberAttribute(Expression<Action<T>> exp)
        {
            
        }
    }

    public class AdsorberAttribute : System.Attribute
    {
        public AdsorberAttribute(Type adsorber, string target)
        {
            Adsorber = adsorber;
            
            Target = target;
        }

        public AdsorberAttribute(ExpressionAdsorberAttribute a )
        {
            
        }


        public readonly Type Adsorber;        
        public readonly string Target;



        public enum MATCH
        {
            OK,
            LOST_METHOD,
            DIFFERENT_METHOD_PARAMS_LENGTH,
            DIFFERENT_METHOD_PARAMS_TYPE
        }
        public MATCH Match(MethodInfo method)
        {
            var field = Adsorber.GetField(Target);
            if (field == null)
            {
                return MATCH.LOST_METHOD;
            }            

            var adsbuterParams = field.FieldType.GetMethod("Invoke").GetParameters();
            var scriptParams = method.GetParameters();

            if (adsbuterParams.Length != scriptParams.Length)
            {
                return MATCH.DIFFERENT_METHOD_PARAMS_LENGTH;
            }

            for (int i = 0; i < adsbuterParams.Length; i++)
            {
                if(adsbuterParams[i].ParameterType != scriptParams[i].ParameterType)
                    return MATCH.DIFFERENT_METHOD_PARAMS_TYPE;
            }
            
            return MATCH.OK;
        }
    }
}
