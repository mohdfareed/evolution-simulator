shader_type canvas_item;

uniform sampler2D noise : filter_nearest, repeat_enable;
uniform vec3 color : source_color; // fog color
uniform vec2 speed = vec2(0.05, 0.05); // fog movement speed
uniform float fade_multiplier = 1.0; // fog fade multiplier
uniform float fade_margin = 0.15; // fog fade margin

void fragment() {
    // sample noise with distortion
    vec2 noise_uv = SCREEN_UV + speed * TIME;
    vec3 noise_value = texture(noise, noise_uv).rgb;

    // get least distance from the edge of the screen
    float distance = min(min(UV.x, 1.0 - UV.x), min(UV.y, 1.0 - UV.y));
    // fade the fog out near the edges of the screen
    float edge_fade = smoothstep(0.0, fade_margin, distance);
    // fade faster at darker areas of noise
    float avg_noise = (noise_value.r + noise_value.g + noise_value.b) / 3.0;
    edge_fade /= avg_noise * fade_multiplier;

	// apply the fog effect and color
    COLOR = vec4(noise_value * color, edge_fade);
}
