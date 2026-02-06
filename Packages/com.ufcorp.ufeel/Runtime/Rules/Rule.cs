using System;

namespace UFeel
{
    public class Rule
    {
        public Func<bool> Condition;
        public Action Action;
        public bool IsUnique;
        public int Id;

        public Rule(int id, Func<bool> condition, Action action, bool isUnique)
        {
            Id = id;
            Condition = condition;
            Action = action;
            IsUnique = isUnique;
        }
    }
}