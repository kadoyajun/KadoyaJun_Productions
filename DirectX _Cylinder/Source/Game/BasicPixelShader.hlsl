#include "BasicShader.hlsli"

float4 main(GSOutput input) : SV_TARGET
{
	// ïΩçsåıåπ(w=0) ì_åıåπ(w=1)
    float3 light = normalize(LightPosition.xyz - LightPosition.w * input.worldPosition.xyz);

	// ägéUîΩéÀ
    float diffuse = max(dot(light, input.worldNormal), 0);
    float3 diffuseColor = diffuse * MaterialDiffuse.rgb;

	// ãæñ îΩéÀ
    float3 reflect = 2 * input.worldNormal * dot(input.worldNormal, light) - light;
    float3 viewDir = normalize(ViewPosition - input.worldPosition).xyz;
    float specular = pow(saturate(dot(reflect, viewDir)), MaterialSpecularPower);
    float3 specularColor = specular * MaterialSpecularColor.rgb;

    return float4(diffuseColor + specularColor, MaterialDiffuse.a);
}
