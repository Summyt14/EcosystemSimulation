namespace _Scripts.Utils
{
    public static class HelperMethods
    {
        public static float Map (float x, float x1, float x2, float y1,  float y2)
        {
            float m = (y2 - y1) / (x2 - x1);
            float c = y1 - m * x1;
            return m * x + c;
        }
    }
}