namespace R8EOX.Menu.Internal
{
    using UnityEngine;

    internal readonly struct ButtonStateColors
    {
        internal readonly Color BorderColor;
        internal readonly Color FillColor;
        internal readonly Color TextColor;

        internal ButtonStateColors(Color border, Color fill, Color text)
        {
            BorderColor = border;
            FillColor = fill;
            TextColor = text;
        }
    }
}
