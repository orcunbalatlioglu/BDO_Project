using BDO_Sender;

namespace IEC62351_3_Encrypted_Data_Transfer
{

    class Program
    {   
        static void Main(string[] args)
        {   
            DFA dfa = new DFA();
            OperationManager operationManager = new OperationManager();
            while(dfa.currentState != State.S11) { 
                bool state = operationManager.Operations(dfa.currentState);
                dfa.currentState = dfa.NextState(dfa.currentState, state);
            }
        }
    }
}
