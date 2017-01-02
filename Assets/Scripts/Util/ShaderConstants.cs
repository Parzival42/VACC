/// <summary>
/// Data class for constant values which are used for shaders.
/// </summary>
public class ShaderConstants
{
    public static readonly string PARAM_WATER_HEIGHT = "_WaterHeight";
    public static readonly string PARAM_TERRAIN_HEIGHT = "_TerrainHeight";
    public static readonly string PARAM_COMBINED_TERRAIN_WATER = "_CombinedHeight";
    public static readonly string PARAM_VELOCITY_X = "_VelocityX";
    public static readonly string PARAM_VELOCITY_Y = "_VelocityY";

    public static readonly string PARAM_TEXTURE_SIZE = "_TextureSize";
    public static readonly string PARAM_UV_HIT = "_UvHit";
    public static readonly string PARAM_RADIUS = "_Radius";
    public static readonly string PARAM_BRUSH_STRENGTH = "_BrushStrength";
    public static readonly string PARAM_BRUSH_FALLOFF = "_Falloff";
    public static readonly string PARAM_RESULT = "Result";
    public static readonly string PARAM_MASK = "_Mask";
    public static readonly string PARAM_DELTA_TIME = "_DeltaTime";
    public static readonly string PARAM_DAMPING_FACTOR = "_DampingFactor";
    public static readonly string PARAM_HEIGHT_TO_FLUX_FACTOR = "_HeightToFluxFactor";
    public static readonly string PARAM_SEGMENT_SIZE_SQUARED = "_SegmentSizeSquared";
    public static readonly string PARAM_SEGMENT_SIZE = "_SegmentSize";
    public static readonly string PARAM_MIN_WATER_HEIGHT = "_MinWaterHeight";
    public static readonly string PARAM_HEIGHTMAP = "_Heightmap";
    public static readonly string PARAM_FLUX_LEFT = "_FluxLeft";
    public static readonly string PARAM_FLUX_RIGHT = "_FluxRight";
    public static readonly string PARAM_FLUX_TOP = "_FluxTop";
    public static readonly string PARAM_FLUX_BOTTOM = "_FluxBottom";
    public static readonly string PARAM_MASK_OFFSET = "_MaskOffset";

    public static readonly string INPUT_WATER_HEIGHT = "WaterHeight";
    public static readonly string INPUT_TERRAIN_HEIGHT = "TerrainHeight";
    public static readonly string INPUT_COMBINED_TERRAIN_WATER = "CombinedTerrainWater";
    public static readonly string INPUT_FLUX_LEFT = "FluxLeft";
    public static readonly string INPUT_FLUX_RIGHT = "FluxRight";
    public static readonly string INPUT_FLUX_TOP = "FluxTop";
    public static readonly string INPUT_FLUX_BOTTOM = "FluxBottom";
    public static readonly string INPUT_VELOCITY_X = "VelocityX";
    public static readonly string INPUT_VELOCITY_Y = "Velocityy";
    public static readonly string INPUT_BOUNDARY_TEXTURE = "BoundaryTexture";
    public static readonly string INPUT_WATER_SOURCES = "WaterSources";

}
