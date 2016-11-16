/*
 * Normalizes the given value between the minimum and maximum boundary.
 * Returns a float between 0 and 1.
 */
float normalizeBetween(float min, float max, float value) {
    return (value - min) / (max - min);
}
