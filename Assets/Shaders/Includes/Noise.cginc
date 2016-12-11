//textureless 3D simplex noise function converted to cg from Ian McEwan, Ashima Arts

half4 permute( half4 x ) {
    return ( ( x * 34.0 ) + 1.0 ) * x - 289.0 * floor(( ( x * 34.0 ) + 1.0 ) * x/289.0);
}

		
half4 taylorInvSqrt( half4 r ) {
    return 1.79284291400159 - 0.85373472095314 * r;
}

		
half snoise( half3 v ) {
    const half2 C = half2( 1.0 / 6.0, 1.0 / 3.0 );
    const half4 D = half4( 0.0, 0.5, 1.0, 2.0 );
    // First corner
			half3 i  = floor( v + dot( v, C.yyy ) );
    half3 x0 = v - i + dot( i, C.xxx );
    // Other corners
			half3 g = step( x0.yzx, x0.xyz );
    half3 l = 1.0 - g;
    half3 i1 = min( g.xyz, l.zxy );
    half3 i2 = max( g.xyz, l.zxy );
    //  x0 = x0 - 0. + 0.0 * C
			half3 x1 = x0 - i1 + 1.0 * C.xxx;
    half3 x2 = x0 - i2 + 2.0 * C.xxx;
    half3 x3 = x0 - 1. + 3.0 * C.xxx;
    // Permutations
			i = i - 289.0 * floor(i/289.0);
    half4 p = permute( permute( permute(
						 i.z + half4( 0.0, i1.z, i2.z, 1.0 ) )
					   + i.y + half4( 0.0, i1.y, i2.y, 1.0 ) )
					   + i.x + half4( 0.0, i1.x, i2.x, 1.0 ) );
    // Gradients
			// ( N*N points uniformly over a square, mapped onto an octahedron.)
			
			half n_ = 1.0 / 7.0;
    // N=7
			half3 ns = n_ * D.wyz - D.xzx;
    half4 j = p - 49.0 * floor( p * ns.z *ns.z );
    //  mod(p,N*N)
			
			half4 x_ = floor( j * ns.z );
    half4 y_ = floor( j - 7.0 * x_ );
    // mod(j,N)
			
			half4 x = x_ *ns.x + ns.yyyy;
    half4 y = y_ *ns.x + ns.yyyy;
    half4 h = 1.0 - abs( x ) - abs( y );
    half4 b0 = half4( x.xy, y.xy );
    half4 b1 = half4( x.zw, y.zw );
    half4 s0 = floor( b0 ) * 2.0 + 1.0;
    half4 s1 = floor( b1 ) * 2.0 + 1.0;
    half4 sh = -step( h, half4(0.0,0.0,0.0,0.0));
    half4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
    half4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
    half3 p0 = half3( a0.xy, h.x );
    half3 p1 = half3( a0.zw, h.y );
    half3 p2 = half3( a1.xy, h.z );
    half3 p3 = half3( a1.zw, h.w );
    // Normalise gradients
			half4 norm = taylorInvSqrt( half4( dot(p0,p0), dot(p1,p1), dot(p2, p2), dot(p3,p3) ) );
    p0 *= norm.x;
    p1 *= norm.y;
    p2 *= norm.z;
    p3 *= norm.w;
    // Mix final noise value
			half4 m = max(0.6 - half4(dot(x0,x0), dot(x1,x1), dot(x2,x2), dot(x3,x3) ), 0.0 );
    m = m * m;
    return 50.0 * dot( m*m, half4( dot(p0,x0), dot(p1,x1), dot(p2,x2), dot(p3,x3) ) );
    //42
}
			
half heightMap( half3 coord ) {
    half n = abs( snoise( coord ) );
    n += 0.25   * abs( snoise( coord * 2.0 ) );
    n += 0.25   * abs( snoise( coord * 4.0 ) );
    n += 0.125  * abs( snoise( coord * 8.0 ) );
    n += 0.0625 * abs( snoise( coord * 16.0 ) );
    return n;
}

