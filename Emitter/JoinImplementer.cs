using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Fmacj.Emitter
{
    internal class JoinImplementer
    {
        private readonly TypeBuilder target;

  
        public JoinImplementer(TypeBuilder target)
        {
            this.target = target;
        }

        public void Implement(JoinGroup joinGroup)
        {
            throw new NotImplementedException();
        }
    }
}
