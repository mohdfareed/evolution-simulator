shader_type canvas_item;

uniform sampler2D noise : filter_nearest, repeat_enable;
uniform vec4 water_color : source_color;
uniform vec2 speed = vec2(0.5, 0.25);
uniform float wave_strength = 0.01;
uniform vec2 tile_index = vec2(0, 0); // tile index in tileset
uniform vec2 tile_size = vec2(0, 0); // tile size in pixels
uniform vec2 tileset_size = vec2(0, 0); // tileset size in tiles


void fragment(){
    // tile uv limits
    vec2 tileset_size_pixels = tileset_size * tile_size;
    vec2 tile_index_pixels = tile_index * tile_size + 1.0;
    vec2 next_tile_index_pixels = tile_index_pixels + tile_size - 2.0;
    vec2 tile_min = tile_index_pixels / tileset_size_pixels;
    vec2 tile_max = next_tile_index_pixels / tileset_size_pixels;
    // sample noise with distortion
    vec2 noise_uv = SCREEN_UV + speed * TIME;
    vec2 noise_value = 2.0 * texture(noise, noise_uv).rg - vec2(1.0);
    // clamp UV to tile limits
    vec2 uv = clamp(UV + noise_value * wave_strength, tile_min, tile_max);
    // apply distortion to UV
    COLOR = texture(TEXTURE, uv) * water_color;
}
