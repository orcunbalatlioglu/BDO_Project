namespace BDO_Receiver
{   
    public enum State
    {
        S0,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6,
        S7,
        S8,
        S9,
        S10,
        S11,
        S12
    }
    internal class DFA
    {           
        public State currentState { get; set; }

        public DFA()
        {
            currentState = State.S0;
        }
        public State NextState(State currentState, bool state)
        {
            return GetNextState(currentState, state);
        }

        private State GetNextState(State currentState, bool state)
        {
            switch (currentState)
            {
                case State.S0:
                    return state == false ? State.S0 : State.S1;
                case State.S1:
                    return state == false ? State.S0 : State.S2;
                case State.S2:
                    return state == false ? State.S0 : State.S3;
                case State.S3:
                    return state == false ? State.S0 : State.S4;
                case State.S4:
                    return state == false ? State.S0 : State.S5;
                case State.S5:
                    return state == false ? State.S0 : State.S6;
                case State.S6:
                    return state == false ? State.S6 : State.S7;
                case State.S7:
                    return state == false ? State.S7 : State.S8;
                case State.S8:
                    return state == false ? State.S0 : State.S9;
                case State.S9:
                    return state == false ? State.S12 : State.S10;
                case State.S10:
                    return state == false ? State.S12 : State.S11;
                case State.S11:
                    return state == false ? State.S12 : State.S9;
                case State.S12:
                    return State.S12;
                default:
                    return State.S0;
            }
        }
    }
}
