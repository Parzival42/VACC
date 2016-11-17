/*
 * Normalizes the given value between the minimum and maximum boundary.
 * Returns a float between 0 and 1.
 */
float normalizeBetween(float min, float max, float value) {
    return (value - min) / (max - min);
}

/**
 * Rim value calculation (Used for rim lighting or other view dependent effects.)
 */
half calculateRim(float3 viewDir, float3 normal, half rimPower) {
    return pow(saturate(dot(normalize(viewDir), normal)), rimPower);
}
