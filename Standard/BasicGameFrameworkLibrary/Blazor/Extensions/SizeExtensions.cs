namespace BasicGameFrameworkLibrary.Blazor.Extensions;
public static class SizeExtensions
{
    extension (int request)
    {
        public int HeightPixels => ll1.GetHeight(request);
        //public string HeightString() => $"{request}vh"; //to be consistent with the width one.
        public string WidthString<D>()
            where D : IDeckObject, new()
        {
            D obj = new();
            return obj.WidthString(request);
        }

    }
    extension (double request)
    {
        public string HeightPixels => $"{ll1.GetHeight(request)}px";
    }

    extension (float request)
    {
        public string HeightPixels => $"{ll1.GetHeight(request)}px";
    }

    //extension (SizeF size)
    //{
    //    public string WidthString(int heightRequest)
    //    {
    //        double starts = heightRequest * size.Width / size.Height;
    //        return $"{starts}vh"; //i think
    //    }
    //}
    extension<D>(D deck)
        where D : IDeckObject
    {
        public string WidthString(int heightRequest)
        {
            double starts = heightRequest * deck.DefaultSize.Width / deck.DefaultSize.Height;
            return $"{starts}px"; //i think
        }
    }
}