namespace Evaluator
{
    /// <summary>
    /// 表达式最小单元
    /// </summary>
    public class EvalItem
    {
        public string Content { get; private set; }
        public EItemType ItemType { get; private set; }
        /// <summary>
        /// 是否为常量
        /// 当ItemType==EItemType.Value 且 Content可以转化为数字的时候 IsConstant为True
        /// 其它情况为flase
        /// </summary>
        public bool IsConstant { get; private set; }
        public decimal Value { get; private set; }
        public EvalItem(EItemType itemType, string content)
        {
            ItemType = itemType;
            Content = content;
            if (itemType == EItemType.Operator)
            {
                IsConstant = false;
            }
            else
            {
                try
                {
                    Value = decimal.Parse(Content);
                    IsConstant = true;
                }
                catch
                {
                    IsConstant = false;
                }
            }
        }
    }
}
