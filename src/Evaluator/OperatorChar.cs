namespace Evaluator
{
    /// <summary>
    /// Operator操作符
    /// </summary>
    public struct OperatorChar
    {
        private char _operator;
        public char Operator
        {
            get { return _operator; }
            set
            {
                _operator = value;
                Level = _operator.GetLevel();
            }
        }
        public int Level { get; private set; }
        public override bool Equals(object obj)
        {
            return _operator.Equals(((OperatorChar)obj)._operator);
        }
        public override int GetHashCode()
        {
            return _operator.GetHashCode();
        }
        public static bool operator >(OperatorChar oc1, OperatorChar oc2)
        {
            return oc1.Level > oc2.Level;
        }
        public static bool operator <(OperatorChar oc1, OperatorChar oc2)
        {
            return oc1.Level < oc2.Level;
        }
        public static bool operator ==(OperatorChar oc1, OperatorChar oc2)
        {
            return oc1.Operator == oc2.Operator;
        }
        public static bool operator !=(OperatorChar oc1, OperatorChar oc2)
        {
            return oc1.Operator != oc2.Operator;
        }
    }
}
