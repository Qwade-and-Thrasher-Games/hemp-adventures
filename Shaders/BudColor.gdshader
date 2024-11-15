shader_type canvas_item;

// Color replacement settings
uniform vec4 replacement_color : source_color;
uniform vec4 target_color : source_color = vec4(1.0, 0.65, 0.0, 1.0); // Default orange
uniform float hue_tolerance : hint_range(0.0, 1.0) = 0.3;
uniform float saturation_tolerance : hint_range(0.0, 1.0) = 0.3;
uniform float value_tolerance : hint_range(0.0, 1.0) = 0.3;

// Advanced features
uniform bool preserve_luminosity = true;
uniform float smoothness : hint_range(0.0, 1.0) = 0.1;
uniform float intensity : hint_range(0.0, 1.0) = 1.0;
uniform bool invert_mask = false;
uniform vec2 color_range = vec2(0.0, 1.0);

// Edge detection
uniform bool edge_detection = false;
uniform float edge_thickness : hint_range(0.0, 5.0) = 1.0;
uniform vec4 edge_color : source_color = vec4(0.0, 0.0, 0.0, 1.0);

// Animation
uniform bool enable_animation = false;
uniform float time_scale : hint_range(0.0, 5.0) = 1.0;

vec3 rgb2hsv(vec3 c) {
    vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
    vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

float getLuminosity(vec3 color) {
    return 0.299 * color.r + 0.587 * color.g + 0.114 * color.b;
}

vec4 detectEdges(sampler2D tex, vec2 uv, vec2 texture_pixel_size) {
    vec2 offsetX = vec2(texture_pixel_size.x * edge_thickness, 0.0);
    vec2 offsetY = vec2(0.0, texture_pixel_size.y * edge_thickness);
    
    float left = getLuminosity(texture(tex, uv - offsetX).rgb);
    float right = getLuminosity(texture(tex, uv + offsetX).rgb);
    float top = getLuminosity(texture(tex, uv - offsetY).rgb);
    float bottom = getLuminosity(texture(tex, uv + offsetY).rgb);
    
    float edge = sqrt((left - right) * (left - right) + (top - bottom) * (top - bottom));
    return edge > 0.1 ? edge_color : vec4(0.0);
}

void fragment() {
    vec4 original_color = texture(TEXTURE, UV);
    vec3 original_hsv = rgb2hsv(original_color.rgb);
    vec3 target_hsv = rgb2hsv(target_color.rgb);
    
    // Calculate color difference in HSV space
    float hue_diff = min(abs(original_hsv.x - target_hsv.x), 
                        1.0 - abs(original_hsv.x - target_hsv.x));
    float sat_diff = abs(original_hsv.y - target_hsv.y);
    float val_diff = abs(original_hsv.z - target_hsv.z);
    
    // Calculate replacement strength
    float strength = smoothstep(hue_tolerance, hue_tolerance * (1.0 - smoothness), hue_diff) *
                    smoothstep(saturation_tolerance, saturation_tolerance * (1.0 - smoothness), sat_diff) *
                    smoothstep(value_tolerance, value_tolerance * (1.0 - smoothness), val_diff);
    
    // Apply intensity and range mapping
    strength *= intensity;
    strength = smoothstep(color_range.x, color_range.y, strength);
    if (invert_mask) strength = 1.0 - strength;
    
    // Animation
    if (enable_animation) {
        float wave = sin(TIME * time_scale) * 0.5 + 0.5;
        strength *= wave;
    }
    
    // Create the replacement color
    vec4 result = original_color;
    if (preserve_luminosity) {
        float original_lum = getLuminosity(original_color.rgb);
        float replacement_lum = getLuminosity(replacement_color.rgb);
        vec3 adjusted_replacement = replacement_color.rgb * (original_lum / replacement_lum);
        result = mix(original_color, vec4(adjusted_replacement, replacement_color.a), strength);
    } else {
        result = mix(original_color, replacement_color, strength);
    }
    
    // Edge detection
    if (edge_detection) {
        vec4 edges = detectEdges(TEXTURE, UV, TEXTURE_PIXEL_SIZE);
        result = mix(result, edges, edges.a);
    }
    
    COLOR = result;
}