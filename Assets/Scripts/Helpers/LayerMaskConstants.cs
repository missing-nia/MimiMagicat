namespace Magicat.Helpers
{
    // Constant values for layer mask access
    // (This can be a little evil since *techincally* masks can be changed but it can help with readability)
    public static class LayerMaskConstants
    {
        public const int LAYERMASK_ALL = -1;
        public const int LAYERMASK_NONE = 0;
        public const int LAYERMASK_DEFAULT = 1;
        public const int LAYERMASK_UI = 1 << 5;
        public const int LAYERMASK_HUD = 1 << 6;
        public const int LAYERMASK_ENTITY = 1 << 8;
        public const int LAYERMASK_PLAYER = 1 << 9;
        public const int LAYERMASK_BREAKABLE = 1 << 10;
        public const int LAYERMASK_HEALTHBAR = 1 << 12;
    }
}
