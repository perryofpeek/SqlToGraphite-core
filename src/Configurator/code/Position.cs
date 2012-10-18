public class Position
{
    public Position(int left, int width, int top, int height)
    {
        this.Left = left;
        this.Height = height;
        this.Top = top;
        this.Width = width;
    }

    public int Left { get; private set; }
    public int Height { get; private set; }
    public int Top { get; private set; }
    public int Width { get; private set; }
}