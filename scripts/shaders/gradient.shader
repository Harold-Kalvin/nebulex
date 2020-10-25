shader_type canvas_item;

render_mode blend_mix;

uniform sampler2D gradient: hint_black;

const float PI = 3.14159265358979323846;

vec2 rotateUV(vec2 uv, vec2 pivot, float rotation) {
    float sine = sin(rotation);
    float cosine = cos(rotation);

    uv -= pivot;
    uv.x = uv.x * cosine - uv.y * sine;
    uv.y = uv.x * sine + uv.y * cosine;
    uv += pivot;

    return uv;
}

void fragment() {
    // gradient
    vec4 sampled_color = texture(gradient, rotateUV(UV, vec2(0.5), PI/2f)); // change angle
    vec4 main_texture = vec4(sampled_color.rgb, texture(TEXTURE, UV).a * 1.0);

    // apply gradient
    COLOR = main_texture;
}
