namespace AILibrary
{
    using System;

    [Serializable]
    public class ToolRoot
    {
        public string type;
        public ToolRootFunction function;
    }

    [Serializable]
    public class ToolRootFunction
    {
        public string name;
        public string description;
        public Parameter parameters;
    }

    [Serializable]
    public class Parameter
    {
        public string type;
        public ContentProperty properties;
        public string[] required;
    }

    [Serializable]
    public class ContentProperty
    {
        public Stage stage;
        public Description description;
        public StatusOption options;
    }

    [Serializable]
    public class Stage
    {
        public string type;
    }

    [Serializable]
    public class Description
    {
        public string type;
        public int minLength;
    }

    [Serializable]
    public class StatusOption
    {
        public string type;
        public int minItems;
        public Item items;
    }

    [Serializable]
    public class Item
    {
        public string type;
        public StatusOptionProperty properties;
        public string[] required;
    }

    [Serializable]
    public class StatusOptionProperty
    {
        public Title title;
        public Result result;
    }

    [Serializable]
    public class Title
    {
        public string type;
    }

    [Serializable]
    public class Result
    {
        public string type;
    }
}