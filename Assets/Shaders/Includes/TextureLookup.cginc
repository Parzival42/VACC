/**
* Performs two texture lookups, were the second texture is tiled differently
* and brightened up a bit to reduce the optical tiling.
*/
fixed4 SampleMultiUvMix(sampler2D tex, float2 uv, float tiling, float secondTexTiling, float brightFactor) {
    return tex2D(tex, uv * tiling) * tex2D(tex, uv * secondTexTiling) * brightFactor;
}
