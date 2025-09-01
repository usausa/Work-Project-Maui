namespace WorkDesign;

[Flags]
public enum JoyStickMode
{
    Horizontal = 1,
    Vertical = 2,
    Both = Horizontal | Vertical
}
