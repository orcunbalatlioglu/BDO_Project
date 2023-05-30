using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;

namespace BDO_Receiver
{
    class ReceiverProgram
    {
        static void Main(string[] args)
        {
            DFA dfa = new DFA();
            OperationManager operationManager = new OperationManager();
            while (dfa.currentState != State.S12)
            {
                bool state = operationManager.Operations(dfa.currentState);
                dfa.currentState = dfa.NextState(dfa.currentState, state);
            }
        }

      
    }
}