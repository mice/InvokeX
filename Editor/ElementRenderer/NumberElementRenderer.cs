using XInspect;

public class NumberElementRenderer : TypeElementRenderer
{
    private IntXField fieldView;
    public NumberElementRenderer()
    {
        fieldView = new IntXField("");
        element = fieldView;
        this.SetValueAction = _SetValueAction;
    }

    public NumberElementRenderer InitInt(string paramName)
    {
        this.fieldView.label = paramName;
        this.type = typeof(int);

        this.ToValueFunc = (r) =>
        {
            int.TryParse(fieldView.value, out var intValue);
            return intValue;
        };
        return this;
    }

    public NumberElementRenderer InitUInt(string paramName)
    {
        this.fieldView.label = paramName;
        this.type = typeof(uint);

        this.ToValueFunc = (r) =>
        {
            uint.TryParse(fieldView.value, out var intValue);
            return intValue;
        };

        this.SetValueAction = (obj) =>
        {
            fieldView.value = System.Convert.ToUInt32(obj).ToString();
        };
        return this;
    }

    protected void _SetValueAction(System.Object obj)
    {
        fieldView.value = System.Convert.ToInt32(obj).ToString();
    }
}
