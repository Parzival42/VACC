float3x3 img3x3(sampler2D colorSampler, float2 textureCoord, int colorChannel, float texelSize)
{
    float d = texelSize;
    float3x3 c;
    c[0][0] = tex2D(colorSampler, textureCoord + float2(-d, -d))[colorChannel];
    c[0][1] = tex2D(colorSampler, textureCoord + float2( 0, -d))[colorChannel];
    c[0][2] = tex2D(colorSampler, textureCoord + float2( d, -d))[colorChannel];

    c[1][0] = tex2D(colorSampler, textureCoord + float2(-d, 0))[colorChannel];
    c[1][1] = tex2D(colorSampler, textureCoord                )[colorChannel];
    c[1][2] = tex2D(colorSampler, textureCoord + float2( d, 0))[colorChannel];

    c[2][0] = tex2D(colorSampler, textureCoord + float2(-d, d))[colorChannel];
    c[2][1] = tex2D(colorSampler, textureCoord + float2( 0, d))[colorChannel];
    c[2][2] = tex2D(colorSampler, textureCoord + float2( d, d))[colorChannel];
    return c;
}

float3 height2NormalSobel(float3x3 c) {
    float3x3 x = float3x3(1.0, 0.0, -1.0,
                          2.0, 0.0, -2.0,
                          1.0, 0.0, -1.0);

    float3x3 y = float3x3(1.0,  2.0,  1.0,
                          0.0,  0.0,  0.0,
                          -1.0, -2.0, -1.0);
    x = x * c;
    y = y * c;

    float cx =  x[0][0] + x[0][2]
              + x[1][0] + x[1][2]
              + x[2][0] + x[2][2];

    float cy =  y[0][0] + y[0][1] + y[0][2]
              + y[2][0] + y[2][1] + y[2][2];

    float cz =  sqrt(1 - (cx * cx + cy * cy));
    return float3(cx, cy, cz);
}
