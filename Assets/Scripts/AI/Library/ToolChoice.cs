namespace AILibrary
{
    using System;

    [Serializable]
    public class ToolChoice
    {
        public string type;
        public ToolChoiceFunction function;
    }

    [Serializable]
    public class ToolChoiceFunction
    {
        public string name;
    }
}