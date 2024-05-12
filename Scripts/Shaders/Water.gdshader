shader_type canvas_item;

uniform sampler2D noise : filter_nearest, repeat_enable;
uniform vec4 color : source_color;
uniform vec2 speed = vec2(0.1, 0.1);
uniform float wave_strength = 0.005;
uniform vec2 tile_count = vec2(1, 1); // tileset size in tiles


void fragment(){
    // sample noise with distortion
    vec2 noise_uv = UV + speed * TIME;
    vec2 noise_value = 2.0 * texture(noise, noise_uv).rg - vec2(1.0);
    noise_value = UV + noise_value * wave_strength;

    // apply tiling
    vec2 tile_index = floor(UV * tile_count);
    vec2 tile_min = (tile_index / tile_count);
    vec2 tile_max = (tile_index + 1.0) / tile_count;
    noise_value.x = mod(noise_value.x, tile_max.x - tile_min.x) + tile_min.x;
    noise_value.y = mod(noise_value.y, tile_max.y - tile_min.y) + tile_min.y;

    // apply distortion to UV
    COLOR = texture(TEXTURE, noise_value) * color;
}
