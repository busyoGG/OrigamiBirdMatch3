namespace ReflectionUI
{
    public enum UIType
    {
        Comp,
        TextField,
        TextInput,
        Loader,
        List,
        Slider,
        Image,
        ComboBox
    }

    public enum UIAction
    {
        ListRender,
        ListProvider,
        ListClick,
        Click,
        DragStart,
        DragHold,
        DragEnd,
        Drop,
        Hover,
        Slider,
        ComboBox
    }

    public enum TweenTarget
    {
        None,
        X,
        Y,
        Position,
        Size,
        ScaleX,
        ScaleY,
        Scale,
        Rotation,
        Alpha,
        Heihgt,
        Width
    }

    public enum UIClass
    {
        Model,
        Drag
    }

}