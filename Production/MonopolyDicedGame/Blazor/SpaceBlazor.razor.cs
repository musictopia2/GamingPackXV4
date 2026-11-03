namespace MonopolyDicedGame.Blazor;
public partial class SpaceBlazor
{
    [Parameter]
    public float MarginTop { get; set; }
    [Parameter]
    public float MarginLeft { get; set; }
    [Parameter]
    [EditorRequired]
    public int Column { get; set; }
    [Parameter]
    [EditorRequired]
    public int Row { get; set; }
    [Parameter]
    public EventCallback OnClicked { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    //can try to create a helper.
    private float GetX
    {
        get
        {
            float output = -1;
            float upTo = 0;
            8.Times(x =>
            {
                if (x == Column && output == -1)
                {
                    output = upTo + MarginLeft;
                }
                upTo += 50;
            });
            if (output == -1)
            {
                throw new CustomBasicException("X Not Found");
            }
            return output;
        }
    }
    private float GetY
    {
        get
        {
            float output = -1;
            float upTo = 0;
            8.Times(y =>
            {
                if (y == Row && output == -1)
                {
                    output = upTo + MarginTop;
                }
                upTo += 50;
            });
            if (output == -1)
            {
                throw new CustomBasicException("Y Not Found");
            }
            return output;
        }
    }
    private PointF GetPoint()
    {
        //there are a total of 8 items for both columns and rows.
        float x = GetX;
        float y = GetY;
        return new(x, y);
    }
}